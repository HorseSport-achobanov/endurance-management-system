﻿using Not.Application.RPC;
using Not.Application.RPC.SignalR;
using Not.Tests.RPC;
using NTS.Application;
using Xunit.Abstractions;

namespace NTS.Judge.Tests;

public class WitnessRpcFixture : HubFixture<WitnessTestClient>
{
    public WitnessRpcFixture(ITestOutputHelper testOutputHelper)
        : base(
            RpcProtocol.Http,
            ApplicationConstants.RPC_PORT,
            ApplicationConstants.WITNESS_HUB,
            "NTS.Judge.MAUI.Server"
        ) { }

    protected override WitnessTestClient CreateClient(
        SignalRSocket socket,
        ITestOutputHelper testOutputHelper
    )
    {
        return new WitnessTestClient(socket, testOutputHelper);
    }
}

[CollectionDefinition(nameof(WitnessRpcFixture))]
public class HubFixtureCollection : ICollectionFixture<WitnessRpcFixture> { }
