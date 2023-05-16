using EnduranceJudge.Domain.Core.Models;
using EnduranceJudge.Domain.Validation;

namespace EnduranceJudge.Domain.State.Countries;

public class Country : DomainBase<CountryException>, ICountryState
{
    private Country() {}
    public Country(string isoCode, string name, int id) : base(GENERATE_ID)
    {
        this.Id = id;
        this.IsoCode = this.Validator.IsRequired(isoCode, nameof(isoCode));
        this.Name = this.Validator.IsRequired(name, nameof(name));
    }

    public string IsoCode { get; private set; }
    public string Name { get; private set; }
}