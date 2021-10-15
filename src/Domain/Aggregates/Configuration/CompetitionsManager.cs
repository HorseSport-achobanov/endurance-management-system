﻿using EnduranceJudge.Domain.Core.Models;
using EnduranceJudge.Domain.Core.Validation;
using EnduranceJudge.Domain.State;
using EnduranceJudge.Domain.State.Competitions;
using EnduranceJudge.Domain.Validation;
using static EnduranceJudge.Localization.DesktopStrings;

namespace EnduranceJudge.Domain.Aggregates.Configuration
{
    public class CompetitionsManager : ManagerObjectBase
    {
        private readonly IState state;

        internal CompetitionsManager(IState state)
        {
            this.state = state;
        }

        public void Save(ICompetitionState state) => this.Validate<CompetitionException>(() =>
        {
            state.Type.IsRequired(TYPE);
            state.Name.IsRequired(NAME);
            state.StartTime.IsRequired(START_TIME).IsFutureDate();

            var competition = new Competition(state.Type, state.Name, state.StartTime)
            {
                Id = state.Id,
            };
            this.state.Event.Save(competition);
        });
    }
}
