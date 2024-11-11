﻿using Not.Blazor.CRUD.Forms.Ports;
using NTS.Domain.Setup.Entities;

namespace NTS.Judge.Blazor.Pages.Setup.Contestants;

public class ParticipationFormModel : IFormModel<Participation>
{
    public int Id { get; set; }
    public TimeSpan? StartTimeOverride { get; set; }
    public bool IsNotRanked { get; set; }
    public Combination? Combination { get; set; }
    public double? MaxSpeedOverride { get; set; }

    public void FromEntity(Participation participation)
    {
        Id = participation.Id;
        StartTimeOverride = participation.StartTimeOverride?.LocalDateTime.TimeOfDay;
        IsNotRanked = participation.IsNotRanked;
        Combination = participation.Combination;
        MaxSpeedOverride = participation.MaxSpeedOverride;
    }
}
