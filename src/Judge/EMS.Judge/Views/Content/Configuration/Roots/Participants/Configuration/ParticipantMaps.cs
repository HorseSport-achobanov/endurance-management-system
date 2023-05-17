﻿using AutoMapper;
using EMS.Judge.Core.Components.Templates.SimpleListItem;
using EMS.Judge.Application.Core.Models;
using Core.Mappings;
using Core.Domain.State.Athletes;
using Core.Domain.State.Horses;
using Core.Domain.State.Participants;

namespace EMS.Judge.Views.Content.Configuration.Roots.Participants.Configuration;

public class ParticipantMaps : ICustomMapConfiguration
{
    public void AddFromMaps(IProfileExpression profile)
    {
        profile.CreateMap<Participant, ListItemModel>();
        profile.CreateMap<Athlete, SimpleListItemViewModel>();
        profile.CreateMap<Horse, SimpleListItemViewModel>();
    }

    public void AddToMaps(IProfileExpression profile)
    {
    }
}
