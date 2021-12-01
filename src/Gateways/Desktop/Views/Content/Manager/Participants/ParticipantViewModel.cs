﻿using EnduranceJudge.Core.Mappings;
using EnduranceJudge.Domain.State.Participants;
using EnduranceJudge.Domain.State.Participations;
using EnduranceJudge.Gateways.Desktop.Core;
using EnduranceJudge.Gateways.Desktop.Views.Content.Manager.PhasePerformances;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace EnduranceJudge.Gateways.Desktop.Views.Content.Manager.Participants
{
    public class ParticipantViewModel : ViewModelBase
    {
        public ParticipantViewModel(Participant participant)
        {
            this.Number = participant.Number;
            this.UpdatePhases(participant.Participation);
        }

        private readonly int number;
        private Visibility visibility = Visibility.Visible;
        public ObservableCollection<PhasePerformanceViewModel> PhasePerformances { get; } = new();
        public ObservableCollection<double> PhaseLengths { get; } = new();

        private void UpdatePhases(Participation participation)
        {
            var lengths = participation.Performances.Select(x => x.Phase.LengthInKm);
            var viewModels = participation.Performances.MapEnumerable<PhasePerformanceViewModel>();

            this.PhasePerformances.AddRange(viewModels);
            this.PhaseLengths.AddRange(lengths);
        }

        public int Number
        {
            get => this.number;
            private init => this.SetProperty(ref this.number, value);
        }
        public Visibility Visibility
        {
            get => this.visibility;
            set => this.SetProperty(ref this.visibility, value);
        }
    }
}
