using NTS.SignalR.Server.ACL;

namespace NTS.Application;

public static class ApplicationConstants
{
    // TODO: change JUDGE_HUB value to ;judge-hub'. It's currently named like that because the
    // legacy Witness app is setup to connect on 'judge-hub', which makes no sense once HUB is separated
    // from Judge app.
    public const string JUDGE_HUB = "judge-judge-hub";
    public const string WITNESS_HUB = Constants.RPC_ENDPOINT; //TODO: change to "witness-hub";
    public const int RPC_PORT = 11337;
}
