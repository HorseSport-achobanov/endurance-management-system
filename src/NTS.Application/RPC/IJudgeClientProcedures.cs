using NTS.Domain.Objects;

namespace NTS.Application.RPC;

public interface IJudgeClientProcedures
{
    Task ReceiveSnapshots(IEnumerable<Snapshot> snapshots);
    Task IncrementConnectionCount(string  connectionId);
    Task DecrementConnectionCount(string connectionId);
}
