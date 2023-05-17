﻿using EMS.Judge.Controls.Manager;
using EMS.Judge.Common;
using EMS.Judge.Common.Services;
using EMS.Judge.Events;
using EMS.Judge.Services;
using EMS.Judge.Application.Core;
using Core.Domain.AggregateRoots.Manager;
using Core.Domain.State.Participations;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EMS.Judge.Views.Content.Manager;
public class ManagerViewModel : ViewModelBase
{
    private static readonly DateTime Today = DateTime.Today;
    private readonly IEventAggregator eventAggregator;
    private readonly IExecutor<ManagerRoot> managerExecutor;
    private readonly IQueries<Participation> participations;
    private readonly IRfidWitness rfidWitness;

    public ManagerViewModel(
        IRfidWitness rfidWitness,
        IEventAggregator eventAggregator,
        IPopupService popupService,
        IExecutor<ManagerRoot> managerExecutor,
        IQueries<Participation> participations)
    {
        var type = WitnessEventType.Arrival;
        this.WitnessType = type.ToString();
        rfidWitness.Configure(type);
        this.rfidWitness = rfidWitness;

        this.eventAggregator = eventAggregator;
        this.managerExecutor = managerExecutor;
        this.participations = participations;

        this.Update = new DelegateCommand(this.UpdateAction);
        this.Start = new DelegateCommand(this.StartAction);
        this.Disqualify = new DelegateCommand(this.DisqualifyAction);
        this.FailToQualify = new DelegateCommand(this.FailToQualifyAction);
        this.Resign = new DelegateCommand(this.ResignAction);
        this.ReInspection = new DelegateCommand(this.ReInspectionAction);
        this.RequireInspection = new DelegateCommand(this.RequireInspectionAction);
        this.StartList = new DelegateCommand(popupService.RenderStartList);
        this.ReconnectHardware = new DelegateCommand(this.ReconnectHardwareAction);
        this.ResetDetectedLists = new DelegateCommand(this.ResetDetectedListsAction);
        this.Select = new DelegateCommand<object[]>(list =>
        {
            var participation = list.FirstOrDefault();
            if (participation != null)
            {
                this.SelectBy(participation as ParticipationGridModel);
            }
        });
        Participation.UpdateEvent += (_, participation) => this.HandleParticipationUpdate(participation);
    }

    public string WitnessType { get; }

    public DelegateCommand<object[]> Select { get; }
    public DelegateCommand Start { get; }
    public DelegateCommand Update { get; }
    public DelegateCommand Disqualify { get; }
    public DelegateCommand FailToQualify { get; }
    public DelegateCommand Resign { get; }
    public DelegateCommand ReInspection { get; }
    public DelegateCommand RequireInspection { get; }
    public DelegateCommand StartList { get; }
    public DelegateCommand ReconnectHardware { get; }
    public DelegateCommand ResetDetectedLists { get; }

    private Visibility startVisibility;
    private string inputNumber;
    private int? inputHours;
    private int? inputMinutes;
    private int? inputSeconds;
    private string notQualifiedReason;
    private bool requireInspectionValue = false;
    private bool reInspectionValue = false;

    public ObservableCollection<string> DetectedFinishes { get; } = new();
    public ObservableCollection<string> DetectedVets { get; } = new();
    public ObservableCollection<ParticipationGridModel> Participations { get; } = new();
    public ParticipationGridModel SelectedParticipation { get; set; }
    public override void OnNavigatedTo(NavigationContext context)
    {
        if (this.Participations.Any())
        {
            return;
        }
        var hasStarted = this.managerExecutor.Execute(x => x.HasStarted(), false);
        if (hasStarted)
        {
            this.ReloadParticipations();
        }
    }

    private void ResetDetectedListsAction()
    {
        this.DetectedFinishes.Clear();
        this.DetectedVets.Clear();
    }

    private void HandleParticipationUpdate(Participation participation)
    {
        ThreadPool.QueueUserWorkItem(delegate
        {
            SynchronizationContext.SetSynchronizationContext(new
                DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));

            SynchronizationContext.Current!.Post(pl =>
            {
                if (participation.UpdateType == WitnessEventType.Arrival)
                {
                    this.DetectedFinishes.Add(participation.Participant.Number);
                    Task.Run(() => this.ExpireParticipationUpdate(participation.UpdateType, participation.Participant.Number));
                }
                else if (participation.UpdateType == WitnessEventType.VetIn)
                {
                    this.DetectedVets.Add(participation.Participant.Number);
                    Task.Run(() => this.ExpireParticipationUpdate(participation.UpdateType, participation.Participant.Number));
                }
            }, null);
        });
    }

    private async Task ExpireParticipationUpdate(WitnessEventType type, string number)
    {
        await Task.Delay(TimeSpan.FromSeconds(300));
        // TODO: probably extract as utility? Also see RFID handlers
        ThreadPool.QueueUserWorkItem(delegate
        {
            SynchronizationContext.SetSynchronizationContext(new
                DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));

            SynchronizationContext.Current!.Post(pl =>
            {
                if (type == WitnessEventType.Arrival)
                {
                    this.DetectedFinishes.Remove(number);
                }
                else if (type == WitnessEventType.VetIn)
                {
                    this.DetectedVets.Remove(number);
                }
            }, null);
        });
    }

    private void StartAction()
    {
        this.managerExecutor.Execute(manager =>
        {
            manager.Start();
            this.rfidWitness.Start();
            this.ReloadParticipations();
        }, true);
    }
    private void UpdateAction()
        => this.ExecuteAndRender((manager, number) => manager.UpdateRecord(number, this.InputTime));
    private void DisqualifyAction()
        => this.ExecuteAndRender((manager, number) => manager.Disqualify(number, this.NotQualifiedReason));
    private void FailToQualifyAction()
        => this.ExecuteAndRender((manager, number) => manager.FailToQualify(number, this.NotQualifiedReason));
    private void ResignAction()
        => this.ExecuteAndRender((manager, number) => manager.Resign(number, this.NotQualifiedReason));
    private void ReInspectionAction()
        => this.ExecuteAndRender((manager, number) => manager.RequireReInspection(number, this.ReInspectionValue));
    private void RequireInspectionAction()
        => this.ExecuteAndRender((manager, number) => manager.RequireCompulsoryInspection(number, this.RequireInspectionValue));
    private void ExecuteAndRender(Action<ManagerRoot, string> action)
    {
        if (string.IsNullOrWhiteSpace(this.InputNumber))
        {
            return;
        }
        var number = this.InputNumber;
        this.managerExecutor.Execute(manager =>
        {
            action(manager, number);
            this.ReloadParticipations();
        }, true);
        this.SelectBy(number);
    }

    private void ReconnectHardwareAction()
    {
        this.rfidWitness.Reconnect();
    }

    private void SelectBy(string number)
    {
        var participation = this.Participations.FirstOrDefault(x => x.Number == number);
        if (participation != null)
        {
            this.SelectBy(participation);
            this.eventAggregator.GetEvent<SelectTabEvent>().Publish(participation);
        }
    }

    private void SelectBy(ParticipationGridModel participation)
    {
        this.SelectedParticipation = participation;
        var performance = this.SelectedParticipation.Performances.LastOrDefault();
        if (performance != null)
        {
            this.ReInspectionValue = performance.IsReinspectionRequired;
            this.RequireInspectionValue = performance.IsRequiredInspectionRequired;
            this.NotQualifiedReason = participation.DisqualifyCode;
        }
        this.InputNumber = participation.Number;
    }

    private void ReloadParticipations()
    {
        this.Participations.Clear();
        this.StartVisibility = Visibility.Collapsed;
        var participations = this.participations.GetAll();
        if (participations.Any())
        {
            var models = new List<ParticipationGridModel>();
            foreach (var participation in participations.OrderBy(x => int.Parse(x.Participant.Number)))
            {
                var viewModel = new ParticipationGridModel(participation, false);
                models.Add(viewModel);
            }
            models = models
                .OrderBy(x => x.IsComplete)
                .ThenBy(x => x.Distance)
                .ToList();
            this.Participations.AddRange(models);
            this.SelectBy(this.Participations.First());
        }
    }

#region setters
    public Visibility StartVisibility
    {
        get => this.startVisibility;
        set => this.SetProperty(ref this.startVisibility, value);
    }
    public string InputNumber
    {
        get => this.inputNumber;
        set => this.SetProperty(ref this.inputNumber, value);
    }
    public string NotQualifiedReason
    {
        get => this.notQualifiedReason;
        set => this.SetProperty(ref this.notQualifiedReason, value);
    }
    public int? InputHours
    {
        get => this.inputHours;
        set => this.SetProperty(ref this.inputHours, value);
    }
    public int? InputMinutes
    {
        get => this.inputMinutes;
        set => this.SetProperty(ref this.inputMinutes, value);
    }
    public int? InputSeconds
    {
        get => this.inputSeconds;
        set => this.SetProperty(ref this.inputSeconds, value);
    }
    public bool ReInspectionValue
    {
        get => this.reInspectionValue;
        set => this.SetProperty(ref this.reInspectionValue, value);
    }
    public bool RequireInspectionValue
    {
        get => this.requireInspectionValue;
        set => this.SetProperty(ref this.requireInspectionValue, value);
    }
#endregion

    private DateTime InputTime
    {
        get
        {
            var time = Today;
            if (this.InputHours.HasValue)
            {
                time = time.AddHours(this.InputHours.Value);
            }
            if (this.InputMinutes.HasValue)
            {
                time = time.AddMinutes(this.InputMinutes.Value);
            }
            if (this.InputSeconds.HasValue)
            {
                time = time.AddSeconds(this.InputSeconds.Value);
            }
            return time;
        }
    }
}
