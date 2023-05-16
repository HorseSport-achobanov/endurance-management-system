﻿using EnduranceJudge.Domain.AggregateRoots.Manager;
using EnduranceJudge.Gateways.Desktop.Core.Services;
using EnduranceJudge.Gateways.Desktop.Core.ViewModels;
using EnduranceJudge.Gateways.Desktop.Services;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace EnduranceJudge.Gateways.Desktop.Views.Dialogs.Startlists;

public class StartlistDialogModel : DialogBase
{
    private readonly IExecutor<ManagerRoot> contestExecutor;
    private readonly IInputHandler input;

    public StartlistDialogModel(IExecutor<ManagerRoot> contestExecutor, ISimplePrinter simplePrinter, IInputHandler input)
    {
        this.contestExecutor = contestExecutor;
        this.input = input;
        this.GetList = new DelegateCommand(this.RenderList);
        this.Print = new DelegateCommand<Visual>(simplePrinter.Print);
    }

    public ObservableCollection<StartTemplateModel> List { get; } = new();
    public DelegateCommand GetList { get; }
    public DelegateCommand<Visual> Print { get; }
    public bool IncludePast { get; set; } = false;

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        this.RenderList();
    }

    private void RenderList()
    {
        var participants = this.contestExecutor
            .Execute(x => x.GetStartList(this.IncludePast), false)
            .Select(x => new StartTemplateModel(x));
        this.List.Clear();
        this.List.AddRange(participants);
    }

    public void HandleScroll(object sender, MouseWheelEventArgs mouseEvent)
    {
        this.input.HandleScroll(sender, mouseEvent);
    }
}
