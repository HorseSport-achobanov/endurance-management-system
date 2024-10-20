﻿using NTS.Domain.Core.Entities.ParticipationAggregate;
using NTS.Domain.Core.Entities;

namespace NTS.Domain.Core.Objects;

public record HandoutDocument : DomainObject
{
    public HandoutDocument(Handout handout, EnduranceEvent enduranceEvent, IEnumerable<Official> officials)
    {
        var participation = handout.Participation;
        Header = new DocumentHeader(participation.Competition.Name, enduranceEvent.PopulatedPlace, enduranceEvent.EventSpan, officials);
        Tandem = participation.Tandem;
        Phases = participation.Phases;
        HandoutId = handout.Id;
    }

    public int HandoutId { get; }
    public DocumentHeader Header { get; }
    public Tandem Tandem { get; }
    public PhaseCollection Phases { get; }
}
