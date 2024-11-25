using Not.Localization;

namespace Not.Blazor.Components;
public partial class NTextBase
{
    string AbsoluteCenterStyle => IsAbsoluteCenter //TODO: fix naming rule here should be_absoluteCenterStyle
        ? "position: absolute; left: 0; width: 100%"
        : "";

    [Inject]
    ILocalizer Localizer { get; set; } = default!;

    [Parameter, EditorRequired]
    public string Content { get; set; } = default!;
    [Parameter]
    public bool IsAbsoluteCenter { get; set; }
}
