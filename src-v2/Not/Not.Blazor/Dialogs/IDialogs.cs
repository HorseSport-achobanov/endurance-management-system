﻿using Not.Injection;
using Not.Blazor.TM.Forms.Components;

namespace Not.Blazor.Dialogs;

public interface IDialogs<T, TForm> : ITransientService
    where TForm : NotForm<T>
{
    Task RenderCreate();
}
