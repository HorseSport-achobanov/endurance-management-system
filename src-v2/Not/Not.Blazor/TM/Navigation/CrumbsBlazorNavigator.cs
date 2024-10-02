﻿using Not.Blazor.Navigation;
using Not.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Not.Blazor.TM.Navigation;

// Cannot be singleton as long as it uses NavigationManager: https://github.com/dotnet/maui/issues/8583
public class CrumbsBlazorNavigator : ICrumsNavigator, ILandNavigator
{
    private readonly NavigationManager _blazorNavigationManager;
    private static Parameters? _parameters;
    private static Stack<(string endpoint, Parameters? parameters)>? _breadCrumbs;

    public CrumbsBlazorNavigator(NavigationManager blazorNavigationManager)
    {
        _blazorNavigationManager = blazorNavigationManager;
    }

    public void NavigateTo(string endpoint)
    {
        ValidateCrumbs();

        _breadCrumbs.Push((endpoint, null));
        NavigateForward(endpoint);
    }

    public void NavigateTo<T>(string endpoint, T parameter)
    {
        ValidateCrumbs();

        _parameters = Parameters.Create(parameter);
        _breadCrumbs.Push((endpoint, _parameters));
        NavigateForward(endpoint);
    }

    public void NavigateBack()
    {
        ValidateCrumbs();

        // Pop current
        if (!_breadCrumbs.TryPop(out var _))
        {
            return;
        }
        if (!_breadCrumbs.TryPeek(out var previousCrumb))
        {
            return;
        }
        _parameters = previousCrumb.parameters;
        NavigateForward(previousCrumb.endpoint);
    }

    public void LandTo(string landing)
    {
        _breadCrumbs = [];
        NavigateTo(landing);
    }

    public T ConsumeParameter<T>()
    {
        if (_parameters == null)
        {
            throw GuardHelper.Exception($"Cannot get parameter '{typeof(T)}'. There are no parameters on this landing");
        }
        var result = _parameters.Get<T>();
        _parameters = null;
        return result;
    }

    public void Initialize(string landingEndpoint)
    {
        if (_breadCrumbs != null)
        {
            return;
        }
        LandTo(landingEndpoint);
    }

    [DoesNotReturn]
    void ValidateCrumbs()
    {
        if (_breadCrumbs == null)
        {
            throw GuardHelper.Exception($"Crumbs are not initialized. NavigateBack cannot function without navigating using LandTo");
        }
    }

    void NavigateForward(string endpoint)
    {
        _blazorNavigationManager.NavigateTo(endpoint);
    }

    internal class Parameters
    {
        private readonly object _parameter;

        public static Parameters Create<T>(T parameter)
        {
            if (parameter == null)
            {
                throw GuardHelper.Exception($"Parameter of type '{typeof(T)}' is null. {nameof(CrumbsBlazorNavigator)} parameters cannot be null");
            }
            return new Parameters(parameter);
        }

        private Parameters(object parameter)
        {
            _parameter = parameter;
        }

        public T Get<T>()
        {
            if (_parameter is not T typedParameter)
            {
                throw GuardHelper.Exception(
                    $"Cannot get parameter of type '{typeof(T)}'/ Underlying type of parameter is '{_parameter.GetType()}'");
            }
            return typedParameter;
        }
    }
}