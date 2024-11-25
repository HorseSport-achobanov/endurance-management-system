using MudBlazor;
using Not.Blazor.Components;
using Not.Structures;
using NTS.Domain.Objects;
using NTS.Domain.Setup.Aggregates;
using NTS.Judge.Blazor.Shared.Contexts;

namespace NTS.Judge.Blazor.Setup.AthletesHorses.Athletes;

public partial class AthleteForm
{
    // TODO: Introduce NotText variants for consistency
    MudTextField<string?> _nameField = default!;
    MudTextField<string?> _feiIdField = default!;
    NAutocomplete<Country?> _countryField = default!;
    MudTextField<string?> _clubField = default!;
    NSelect<AthleteCategory> _categoryField = default!;
    List<NotListModel<AthleteCategory>> _categories = NotListModel
        .FromEnum<AthleteCategory>()
        .ToList();

    [Inject]
    ICountriesContext Countries { get; set; } = default!;

    public override void RegisterValidationInjectors()
    {
        RegisterInjector(nameof(Athlete.Person), () => _nameField);
        RegisterInjector(nameof(Athlete.Country), () => _countryField);
        RegisterInjector(nameof(Athlete.Club), () => _clubField);
        RegisterInjector(nameof(Athlete.FeiId), () => _feiIdField);
        RegisterInjector(nameof(Athlete.Category), () => _categoryField);
    }
}
