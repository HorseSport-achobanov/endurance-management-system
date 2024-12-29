﻿using Microsoft.Extensions.DependencyInjection;
using Not.Application.CRUD.Ports;
using NTS.Domain.Core.Aggregates;
using NTS.Domain.Enums;
using NTS.Domain.Objects;
using NTS.Judge.Core;
using NTS.Judge.Tests.Helpers;
using NTS.Storage.Core;
using static NTS.Judge.Tests.Constants;

namespace NTS.Judge.Tests.Core;

public class ProcessSnapshotTests : JudgeIntegrationTest
{
    public ProcessSnapshotTests() : base(nameof(CoreState))
    {
    }

    IRepository<Participation> Participations => Provider.GetRequiredService<IRepository<Participation>>();

    [Theory]
    [InlineData(SnapshotMethod.Manual)] //Manual is OK here, because if no times are recorded Manual will record ArriveTime first
    [InlineData(SnapshotMethod.RFID)]
    [InlineData(SnapshotMethod.EmsIntegration)]
    public async Task WhenParticipantArrives_ShouldRecordArrivalTime(SnapshotMethod method)
    {
        await Seed();
        var arriveAt = TimestampHelper.Create(19, 30);
        var snapshot = new Snapshot(DEFAULT_COMBINATION_NUMBER, SnapshotType.Stage, method, arriveAt);
        var behind = await GetBehind<ISnapshotProcessor>();

        await behind.Process(snapshot);
        
        var participation = await Participations.Read(x => x.Combination.Number == DEFAULT_COMBINATION_NUMBER);
        Assert.NotNull(participation);
        Assert.Equal(participation.Phases.First().ArriveTime, arriveAt);
    }

    [Theory]
    [InlineData(SnapshotMethod.RFID)]
    [InlineData(SnapshotMethod.EmsIntegration)]
    public async Task WhenParticipantPresents_ShouldRecordPresentTime(SnapshotMethod method)
    {
        await Seed();
        var presentAt = TimestampHelper.Create(19, 35);
        var snapshot = new Snapshot(DEFAULT_COMBINATION_NUMBER, SnapshotType.Vet, method, presentAt);
        var behind = await GetBehind<ISnapshotProcessor>();

        await behind.Process(snapshot);
        
        var participation = await Participations.Read(x => x.Combination.Number == DEFAULT_COMBINATION_NUMBER);
        Assert.NotNull(participation);
        Assert.Equal(participation.Phases.First().PresentTime, presentAt);
    }
}
