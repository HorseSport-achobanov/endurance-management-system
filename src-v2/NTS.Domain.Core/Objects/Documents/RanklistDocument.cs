﻿using NTS.Domain.Core.Entities;
using NTS.Domain.Core.Objects.Documents.Base;

namespace NTS.Domain.Core.Objects.Documents;

public record RanklistDocument : Document
{
    public RanklistDocument(
        Ranklist ranklist,
        EnduranceEvent enduranceEvent,
        IEnumerable<Official> officials
    )
        : base(
            new DocumentHeader(
                ranklist.Name,
                enduranceEvent.PopulatedPlace,
                enduranceEvent.EventSpan,
                officials
            )
        )
    {
        Ranklist = ranklist;
    }

    public Ranklist Ranklist { get; }
}
