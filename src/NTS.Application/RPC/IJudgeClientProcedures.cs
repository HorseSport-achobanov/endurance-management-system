﻿using NTS.Domain.Objects;

namespace NTS.Application.RPC;

public interface IJudgeClientProcedures
{
    Task Process(IEnumerable<Snapshot> snapshots);
}
