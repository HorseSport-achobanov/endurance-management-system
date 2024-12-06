using Core.Domain.State.Athletes;
using NTS.ACL.Abstractions;
using NTS.ACL.Entities.Countries;
using NTS.ACL.Enums;

namespace NTS.ACL.Entities.Athletes;

public class EmsAthlete : EmsDomainBase<EmsAthleteException>
{
    const int ADULT_AGE_IN_YEARS = 18;

    [Newtonsoft.Json.JsonConstructor]
    EmsAthlete() { }

    public EmsAthlete(
        string feiId,
        string firstName,
        string lastName,
        EmsCountry country,
        DateTime birthDate
    )
        : base(GENERATE_ID)
    {
        FeiId = feiId;
        FirstName = firstName;
        LastName = lastName;
        Country = country;
        Category =
            birthDate.AddYears(ADULT_AGE_IN_YEARS) <= DateTime.Now
                ? EmsCategory.Seniors
                : EmsCategory.Children;
    }

    public EmsAthlete(IEmsAthleteState state, EmsCountry country)
        : base(GENERATE_ID)
    {
        FeiId = state.FeiId;
        Club = state.Club;
        FirstName = state.FirstName;
        LastName = state.LastName;
        Category = state.Category;
        Country = country;
    }

    public string FeiId { get; internal set; }
    public string FirstName { get; internal set; }
    public string LastName { get; internal set; }
    public string Club { get; internal set; }
    public EmsCategory Category { get; internal set; }
    public EmsCountry Country { get; internal set; }
    public string Name => $"{FirstName} {LastName}";
}
