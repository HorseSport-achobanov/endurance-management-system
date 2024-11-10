﻿using NTS.Domain.Setup.Entities;
using NTS.Persistence.States;

namespace NTS.Persistence.Adapters;

public class HorseRepository : SetRepository<Horse, SetupState>
{
    public HorseRepository(IStore<SetupState> store)
        : base(store) { }
}
