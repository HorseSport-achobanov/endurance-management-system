﻿using Not.Blazor.Components;
using Not.Events;

namespace Not.Blazor.Print;

public abstract class PrintableComponent : NotComponent, IDisposable
{
    public delegate void ToggleVisibility();

    public static void OnToggle(Action handler)
    {
        TOGGLE_EVENT.Subscribe(handler);
    }

    static Event TOGGLE_EVENT = new();

    [Inject]
    IPrintInterop PrintInterop { get; set; } = default!;

    protected bool IsButtonVisible { get; private set; }

    protected override void OnInitialized()
    {
        TOGGLE_EVENT.Subscribe(VisibilityToggleHook);
    }

    protected async Task OpenPrintDialog()
    {
        InvokeToggle();
        await PrintInterop.OpenPrintDialog();
        InvokeToggle();
    }

    /// <summary>
    /// Make sure to Rerender when overriding this method otherwise changes might not be reflected
    /// </summary>
    protected virtual void VisibilityToggleHook() { }

    public void Dispose()
    {
        TOGGLE_EVENT.UnsubscribeAll();
    }

    void InvokeToggle()
    {
        TOGGLE_EVENT.Emit();
    }
}
