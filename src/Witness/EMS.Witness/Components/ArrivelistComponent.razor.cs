﻿using EMS.Witness.Services;
using Microsoft.AspNetCore.Components;

namespace EMS.Witness.Components;

public partial class ArrivelistComponent
{
    [Inject]
    private IArrivelistService ArrivelistService { get; set; } = null!;

    protected override void OnInitialized()
    {
        this.State = this.ArrivelistService.Arrivelist;
    }
}