using NTS.Compabitility.EMS.Abstractions;

namespace NTS.Compatibility.EMS.Entities.Laps;

public interface ILapState : IIdentifiable
{
    double LengthInKm { get; }
    bool IsFinal { get; }
    int OrderBy { get; }
    int MaxRecoveryTimeInMins { get; }
    int RestTimeInMins { get; }
    bool IsCompulsoryInspectionRequired { get; }
}
