using MudBlazor;
using Not.Blazor.Components;
using NTS.Domain.Objects;
using NTS.Domain.Setup.Aggregates;
using NTS.Judge.Blazor.Shared.Contexts;

namespace NTS.Judge.Blazor.Setup.EnduranceEvents;
public partial class EnduranceEventForm
{
    MudTextField<string?> _placeField = default!;
    NAutocomplete<Country?> _countryField = default!;
    [Inject]
    ICountriesContext Countries { get; set; } = default!;

    public override void RegisterValidationInjectors()
    {
        RegisterInjector(nameof(EnduranceEvent.Place), () => _placeField);
        RegisterInjector(nameof(EnduranceEvent.Country), () => _countryField);
    }
}
