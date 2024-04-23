﻿using NTS.Domain.Setup.Entities;
using Not.Domain;
using NTS.Persistence.Setup;

namespace NTS.Persistence.Adapters;

public class SetupCompetitionRepository : BranchRepository<Competition, SetupState>
{
    public SetupCompetitionRepository(IStore<SetupState> store) : base(store)
    {
    }

    protected override Competition? Get(SetupState context, int id)
    {
        return context.Root?.Competitions.FirstOrDefault(x => x.Id == id);
    }

    protected override IParent<Competition>? GetParent(SetupState context, int childId)
    {
        return context.Root;
    }
}
