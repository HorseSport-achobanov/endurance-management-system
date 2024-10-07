﻿using Not.Services;
using MudBlazor;
using Not.Blazor.Dialogs;
using Not.Blazor.TM.Forms.Components;
using Not.Reflection;

namespace Not.Blazor.TM.Dialogs;

public class NotDialogs<T, TForm> : IDialogs<T, TForm>
    where TForm : FormTM<T>
{
    private readonly IDialogService _mudDialogService;
    private readonly ILocalizer _localizer;

    public NotDialogs(IDialogService mudDialogService, ILocalizer localizer)
    {
        _mudDialogService = mudDialogService;
        _localizer = localizer;
    }

    public async Task RenderCreate()
    {
        var title = _localizer.Get("Create", " ", ReflectionHelper.GetName<T>());
        var dialog = await _mudDialogService.ShowAsync<NotFormCreate<T, TForm>>(title);
        await dialog.Result;
    }
}

public class DialogTM<T, TForm>
    where T : new()
    where TForm : FormTM<T>
{
    private readonly IDialogService _mudDialogService;
    private readonly ILocalizer _localizer;

    public DialogTM(IDialogService mudDialogService, ILocalizer localizer)
    {
        _mudDialogService = mudDialogService;
        _localizer = localizer;
    }

    public async Task RenderCreate()
    {
        var title = _localizer.Get("Create", " ", ReflectionHelper.GetName<T>());
        var dialog = await _mudDialogService.ShowAsync<FormCreateTM<T, TForm>>(title);
        await dialog.Result;
    }
}