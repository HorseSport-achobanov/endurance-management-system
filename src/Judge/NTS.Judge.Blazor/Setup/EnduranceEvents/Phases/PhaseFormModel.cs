﻿using Not.Blazor.CRUD.Forms.Ports;
using NTS.Domain.Setup.Aggregates;

namespace NTS.Judge.Blazor.Setup.EnduranceEvents.Phases;

public class PhaseFormModel : IFormModel<Phase>
{
    public PhaseFormModel()
    {
#if DEBUG
        Recovery = 15;
        Rest = 40;
#endif
    }

    public int Id { get; set; }
    public Loop? Loop { get; set; }
    public int Recovery { get; set; }
    public int? Rest { get; set; }

    public void FromEntity(Phase phase)
    {
        Id = phase.Id;
        Loop = phase.Loop;
        Recovery = phase.Recovery!;
        Rest = phase.Rest!;
    }
}
