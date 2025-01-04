﻿using Not.Application.Behinds.Adapters;
using Not.Application.CRUD.Ports;
using Not.Blazor.CRUD.Ports;
using Not.Exceptions;
using Not.Safe;
using NTS.Application.RPC;
using NTS.Domain.Core.Aggregates;
using NTS.Domain.Core.Aggregates.Participations;
using NTS.Domain.Enums;
using NTS.Domain.Objects;
using NTS.Judge.Blazor.Core.Dashboards;
using NTS.Judge.Blazor.Core.Dashboards.Actions.Eliminations;
using NTS.Judge.Blazor.Core.Dashboards.Actions.Inspections;
using NTS.Judge.Blazor.Core.Dashboards.Actions.Snapshots;
using NTS.Judge.Blazor.Core.Dashboards.Component;
using NTS.Judge.Blazor.Core.Dashboards.Phases;
using NTS.Judge.RPC;

namespace NTS.Judge.Core.Behinds.Adapters;

public class ParticipationBehind
    : ObservableBehind,
        IParticipationContext,
        IInspections,
        IEliminations,
        IDashboardBehind,
        IUpdateBehind<PhaseUpdateModel>,
        ISnapshotProcessor,
        IManualProcessor
{
    readonly List<int> _recentlyProcessed = [];
    readonly IJudgeRpcClient _judgeRpcClient;
    readonly IRepository<Participation> _participationRepository;
    readonly IRepository<SnapshotResult> _snapshotResultRepository;
    Participation? _selectedParticipation;

    public ParticipationBehind(
        IJudgeRpcClient judgeRpcClient,
        IRepository<Participation> participationRepository,
        IRepository<SnapshotResult> snapshotResultRepository
    )
    {
        _judgeRpcClient = judgeRpcClient;
        _participationRepository = participationRepository;
        _snapshotResultRepository = snapshotResultRepository;
    }

    public IReadOnlyList<int> RecentlyProcessed => _recentlyProcessed;
    public IEnumerable<Participation> Participations { get; private set; } = [];
    public Participation? SelectedParticipation
    {
        get => _selectedParticipation;
        set
        {
            _selectedParticipation = value;
            var number = _selectedParticipation?.Combination.Number;
            if (number != null)
            {
                _recentlyProcessed.Remove(number.Value);
            }
            EmitChange();
        }
    }

    protected override async Task<bool> PerformInitialization(params IEnumerable<object> arguments)
    {
        var log = arguments.FirstOrDefault() as Action<string>;
        log?.Invoke("------- RPC ------- Registering events...");
        Participation.PHASE_COMPLETED_EVENT.Subscribe(_judgeRpcClient.SendStartCreated); //TODO: figure out where to subscribe?
        Participation.ELIMINATED_EVENT.Subscribe(
            (x) => _judgeRpcClient.SendParticipationEliminated(x, log!)
        ); //TODO: figure out where to subscribe?
        Participation.RESTORED_EVENT.Subscribe(_judgeRpcClient.SendParticipationRestored); //TODO: figure out where to subscribe?

        Participations = await _participationRepository.ReadAll();
        SelectedParticipation = Participations.FirstOrDefault();
        return Participations.Any();
    }

    public async Task Update(PhaseUpdateModel model)
    {
        Task action() => SafeUpdate(model);
        await SafeHelper.Run(action);
    }

    public async Task Process(Snapshot snapshot, Action<string> log)
    {
        Task action() => SafeProcess(snapshot, log);
        await SafeHelper.Run(action);
    }

    public async Task RequestRepresent(bool isRequested)
    {
        Task action() => SafeRequestReinspection(isRequested);
        await SafeHelper.Run(action);
    }

    public async Task RequireInspection(bool isRequested)
    {
        Task action() => SafeRequestRequiredInspection(isRequested);
        await SafeHelper.Run(action);
    }

    public async Task Withdraw()
    {
        await SafeHelper.Run(SafeWithdraw);
    }

    public async Task Retire()
    {
        await SafeHelper.Run(SafeRetire);
    }

    public async Task FinishNotRanked(string reason)
    {
        Task action() => SafeFinishNotRanked(reason);
        await SafeHelper.Run(action);
    }

    public async Task Disqualify(string reason)
    {
        Task action() => SafeDisqualify(reason);
        await SafeHelper.Run(action);
    }

    public async Task FailToQualify(FtqCode[] ftqCodes, string? reason)
    {
        Task action() => SafeFailToQualify(ftqCodes, reason);
        await SafeHelper.Run(action);
    }

    public async Task RestoreQualification()
    {
        await SafeHelper.Run(SafeRestoreQualification);
    }

    public async Task Process(Timestamp timestamp)
    {
        Task action() => SafeProcess(timestamp, x => { });
        await SafeHelper.Run(action);
    }

    async Task SafeUpdate(PhaseUpdateModel model)
    {
        var participation = Participations.FirstOrDefault(x => x.Phases.Any(y => y.Id == model.Id));
        GuardHelper.ThrowIfDefault(participation);

        participation.Update(model);
        await _participationRepository.Update(participation);
        EmitChange();
    }

    async Task SafeRequestReinspection(bool requestFlag)
    {
        SelectedParticipation!.ChangeReinspection(requestFlag);
        await _participationRepository.Update(SelectedParticipation);

        EmitChange();
    }

    async Task SafeRequestRequiredInspection(bool requestFlag)
    {
        SelectedParticipation!.ChangeRequiredInspection(requestFlag);
        await _participationRepository.Update(SelectedParticipation);

        EmitChange();
    }

    async Task SafeProcess(Timestamp timestamp, Action<string> log)
    {
        var snapshot = new Snapshot(
            SelectedParticipation!.Combination.Number,
            SnapshotType.Automatic,
            SnapshotMethod.Manual,
            timestamp
        );
        await SafeProcess(snapshot, log);
    }

    async Task SafeProcess(Snapshot snapshot, Action<string> log)
    {
        var participation = Participations.FirstOrDefault(x =>
            x.Combination.Number == snapshot.Number
        );
        if (participation == null)
        {
            return;
        }
        var result = participation.Process(snapshot, log);
        if (result.Type == SnapshotResultType.Applied)
        {
            await _participationRepository.Update(participation);
        }
        await _snapshotResultRepository.Create(result);
        _recentlyProcessed.Add(participation.Combination.Number);
        EmitChange();
    }

    async Task SafeWithdraw()
    {
        GuardHelper.ThrowIfDefault(SelectedParticipation);
        SelectedParticipation.Withdraw();
        await _participationRepository.Update(SelectedParticipation);

        EmitChange();
    }

    async Task SafeRetire()
    {
        GuardHelper.ThrowIfDefault(SelectedParticipation);
        SelectedParticipation.Retire();
        await _participationRepository.Update(SelectedParticipation);

        EmitChange();
    }

    async Task SafeFinishNotRanked(string reason)
    {
        GuardHelper.ThrowIfDefault(SelectedParticipation);
        SelectedParticipation.FinishNotRanked(reason);
        await _participationRepository.Update(SelectedParticipation);

        EmitChange();
    }

    async Task SafeDisqualify(string reason)
    {
        GuardHelper.ThrowIfDefault(SelectedParticipation);
        SelectedParticipation.Disqualify(reason);
        await _participationRepository.Update(SelectedParticipation);

        EmitChange();
    }

    async Task SafeFailToQualify(FtqCode[] ftqCodes, string? reason)
    {
        GuardHelper.ThrowIfDefault(SelectedParticipation);

        SelectedParticipation.FailToQualify(ftqCodes, reason);
        await _participationRepository.Update(SelectedParticipation);

        EmitChange();
    }

    async Task SafeRestoreQualification()
    {
        GuardHelper.ThrowIfDefault(SelectedParticipation);
        SelectedParticipation.Restore();
        await _participationRepository.Update(SelectedParticipation);

        EmitChange();
    }
}
