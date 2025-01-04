﻿using System.Globalization;
using NTS.Domain.Enums;
using NTS.Domain.Objects;
using NTS.Judge.Core;
using NTS.Judge.Tests.Helpers;
using NTS.Storage.Core;
using Xunit.Abstractions;

namespace NTS.Judge.Tests.Sample;

[Collection(nameof(WitnessRpcFixture))]
public class RpcIntegrationTest : JudgeIntegrationTest
{
    private readonly WitnessRpcFixture _witnessFIxture;
    private readonly ITestOutputHelper _testOutputHelper;

    public RpcIntegrationTest(WitnessRpcFixture witnessFixture, ITestOutputHelper testOutputHelper)
        : base(nameof(CoreState), testOutputHelper)
    {
        _witnessFIxture = witnessFixture;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task TestEliminatedOnRpcClient()
    {
        await Seed();

        var timestamp = TimestampHelper.Create(hour: 19, minute: 10);
        var snapshot = new Snapshot(1337, SnapshotType.Vet, SnapshotMethod.Manual, timestamp);

        var processor = await GetBehind<ISnapshotProcessor>(_testOutputHelper.WriteLine);

        await AssertRpcInvoked(
            _witnessFIxture,
            () => processor.Process(snapshot, _testOutputHelper.WriteLine),
            nameof(WitnessTestClient.ReceiveEntryUpdate)
        );
    }
}
