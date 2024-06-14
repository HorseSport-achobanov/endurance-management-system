﻿
using MudBlazor;
using Not.Injection;

namespace NTS.Judge.Ports;

public interface IHelloWorldInterop : ITransientService
{
    Task Hello();
    Task Print(MudThemeProvider provider);
}