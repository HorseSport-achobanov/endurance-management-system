﻿using EnduranceJudge.Application.Core;
using EnduranceJudge.Domain.AggregateRoots.Configuration;
using EnduranceJudge.Domain.Core.Models;
using EnduranceJudge.Domain.Enums;
using EnduranceJudge.Domain.State.Personnels;
using EnduranceJudge.Gateways.Desktop.Core.Components.Templates.SimpleListItem;
using EnduranceJudge.Gateways.Desktop.Services;
using EnduranceJudge.Gateways.Desktop.Views.Content.Configuration.Core;
using System.Collections.ObjectModel;
using System.Linq;

namespace EnduranceJudge.Gateways.Desktop.Views.Content.Configuration.Children.Personnel;

public class PersonnelViewModel : NestedConfigurationBase<PersonnelView, Domain.State.Personnels.Personnel>,
    IPersonnelState
{
    private readonly IExecutor<ConfigurationRoot> executor;
    private PersonnelViewModel() : this(null, null) {}
    public PersonnelViewModel(
        IExecutor<ConfigurationRoot> executor,
        IQueries<Domain.State.Personnels.Personnel> personnel) : base(personnel)
    {
        this.executor = executor;
    }

    public ObservableCollection<SimpleListItemViewModel> RoleItems { get; }
        = new(SimpleListItemViewModel.FromEnum<PersonnelRole>());

    private string name;
    private int roleId;

    protected override IDomain Persist()
    {
        var result = this.executor.Execute(
            config => config.Save(this),
            true);
        return result;
    }

    public string Name
    {
        get => this.name;
        set => this.SetProperty(ref this.name, value);
    }
    public int RoleId
    {
        get => this.roleId;
        set => this.SetProperty(ref this.roleId, value);
    }

    public PersonnelRole Role => (PersonnelRole)this.RoleId;
    public string RoleName => this.RoleItems.First(x => x.Id == this.RoleId).Name;
}
