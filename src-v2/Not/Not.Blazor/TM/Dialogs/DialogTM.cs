﻿using Not.Services;
using MudBlazor;
using Not.Blazor.TM.Forms.Components;
using Not.Reflection;

namespace Not.Blazor.TM.Dialogs;

public class DialogTM<T, TForm>
    where T : new()
    where TForm : FormTM<T>
{
    private readonly IDialogService _mudDialogService;
    private readonly ILocalizer _localizer;
    private readonly DialogOptions _options = new DialogOptions { BackdropClick = false };

    public DialogTM(IDialogService mudDialogService, ILocalizer localizer)
    {
        _mudDialogService = mudDialogService;
        _localizer = localizer;
    }

    public async Task RenderCreate()
    {
        var title = _localizer.Get("Create", " ", ReflectionHelper.GetName<T>());
        var dialog = await _mudDialogService.ShowAsync<CreateFormDialog<T, TForm>>(title, _options);
        await dialog.Result;
    }
}