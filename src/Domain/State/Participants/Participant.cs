using EnduranceJudge.Domain.Core.Models;
using EnduranceJudge.Domain.State.Athletes;
using EnduranceJudge.Domain.State.Horses;
using EnduranceJudge.Domain.State.LapRecords;
using System.Collections.ObjectModel;
using System.Linq;

namespace EnduranceJudge.Domain.State.Participants;

public class Participant : DomainBase<ParticipantException>, IParticipantState
{
    public const int DEFAULT_MAX_AVERAGE_SPEED = 16;
    private const string NAME_FORMAT = "{0} - {1} with {2}";

    private ObservableCollection<LapRecord> lapRecords = new();
    private readonly ReadOnlyObservableCollection<LapRecord> lapRecordsReadonly;

    private Participant()
    {
        this.lapRecordsReadonly = new(this.lapRecords);
    }
    public Participant(Athlete athlete, Horse horse, IParticipantState state = null) : base(GENERATE_ID)
    {
        this.lapRecordsReadonly = new ReadOnlyObservableCollection<LapRecord>(this.lapRecords);
        this.Athlete = athlete;
        this.Horse = horse;
        this.RfIdHead = state?.RfIdHead;
        this.RfIdNeck = state?.RfIdNeck;
        this.MaxAverageSpeedInKmPh = state?.MaxAverageSpeedInKmPh;
        this.Number = state?.Number;
    }

    public string RfIdHead { get; internal set; }
    public string RfIdNeck { get; internal set; }
    public string Number { get; internal set; }
    public int? MaxAverageSpeedInKmPh { get; internal set; }
    public Horse Horse { get; internal set; }
    public Athlete Athlete { get; internal set; }
    public ObservableCollection<LapRecord> LapRecords
    {
        get => this.lapRecords;
        private set => this.lapRecords = new ObservableCollection<LapRecord>(value.ToList());
    }

    public void Add(LapRecord record)
        => this.lapRecords.Add(record);

    public string Name => FormatName(this.Number, this.Athlete.Name, this.Horse.Name);

    public static string FormatName(string number, string athleteName, string horseName)
    {
        return string.Format(NAME_FORMAT, number, athleteName, horseName);
    }

    public void __REMOVE_RECORDS__()
    {
        this.lapRecords.Clear();
    }
}
