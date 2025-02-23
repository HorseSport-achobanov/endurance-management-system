﻿using Not.Domain;
using NTS.Domain.Core.Aggregates;
using NTS.Storage.Documents.Countries;
using NTS.Storage.Documents.EnduranceEvents.Models;
using NTS.Storage.Documents.Officials;

namespace NTS.Storage.Documents.EnduranceEvents;

public class EnduranceEventDocument : Document, IAggregateRoot // TODO: questionmark?
{
    public EnduranceEventDocument(EnduranceEvent enduranceEvent, IEnumerable<Official> officials, IEnumerable<Ranking> rankings) : base(enduranceEvent.Id)
    {
        Country = new CountryDocument(enduranceEvent.PopulatedPlace.Country);
        City = enduranceEvent.PopulatedPlace.City;
        Location = enduranceEvent.PopulatedPlace.Location;
        StartDay = enduranceEvent.EventSpan.StartDay;
        EndDay = enduranceEvent.EventSpan.EndDay;
        Officials = officials.Select(x => new OfficialModel(x)).ToArray();
        Rankings = rankings.Select(x => new RankingModel(x)).ToArray();
    }

    public CountryDocument Country { get; init; }
    public string City { get; init; }
    public string? Location { get; init; }
    public DateTimeOffset StartDay { get; init; }
    public DateTimeOffset EndDay { get; init; }
    public OfficialModel[] Officials { get; init; }
    public RankingModel[] Rankings { get; init; }
}
