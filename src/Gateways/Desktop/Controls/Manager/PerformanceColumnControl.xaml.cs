﻿using Prism.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static EnduranceJudge.Localization.Strings;

namespace EnduranceJudge.Gateways.Desktop.Controls.Manager;

public class PerformanceColumnControl : StackPanel
{
    private PerformanceColumnModel performance;

    public PerformanceColumnModel Performance
    {
        get => (PerformanceColumnModel)GetValue(PERFORMANCE_PROPERTY);
        set => SetValue(PERFORMANCE_PROPERTY, value);
    }
    public bool IsReadonly
    {
        get => (bool)this.GetValue(IS_READONLY_PROPERTY); 
        set => this.SetValue(IS_READONLY_PROPERTY, value);
    }

    public static readonly DependencyProperty IS_READONLY_PROPERTY =
        DependencyProperty.Register(
            nameof(IsReadonly),
            typeof(bool),
            typeof(PerformanceColumnControl));

    public static readonly DependencyProperty PERFORMANCE_PROPERTY =
        DependencyProperty.Register(
            nameof(Performance),
            typeof(PerformanceColumnModel),
            typeof(PerformanceColumnControl),
            new PropertyMetadata(null, OnPerformanceChanged));

    private static void OnPerformanceChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
        var column = (PerformanceColumnControl)sender;
        var performance = (PerformanceColumnModel) args.NewValue;
        column.Construct(performance);
    }

    public PerformanceColumnControl() {}
    public PerformanceColumnControl(PerformanceColumnModel performance, bool isReadonly) : this()
    {
        this.IsReadonly = isReadonly;
        this.Performance = performance;
        this.Construct(performance);
    }

    private void Construct(PerformanceColumnModel performance)
    {
        this.performance = performance;

        this.AddHeader();
        if (this.IsReadonly)
        {
            this.AddArrivalText();
            this.AddInspectionText();
            this.AddReInspectionText();
            this.AddRequiredInspectionText();
            this.AddCompulsoryInspectionText();
        }
        else
        {
            this.AddArrivalInput();
            this.AddInspectionInput();
            this.AddReInspectionInput();
            this.AddRequiredInspectionInput();
            this.AddCompulsoryInspectionInput();
        }
        this.AddNextStartTime();
        this.AddRecovery();
        this.AddTime();
        this.AddAverageSpeed();
        this.AddAverageSpeedTotal();
        if (!this.IsReadonly)
        {
            this.AddEdit();
        }
    }

    private void AddHeader()
        => this.CreateText(nameof(this.performance.HeaderValue));
    private void AddArrivalInput()
        => this.CreateInput(nameof(this.performance.ArrivalTimeString));
    private void AddInspectionInput()
        => this.CreateInput(nameof(this.performance.InspectionTimeString));
    private void AddReInspectionInput()
        => this.CreateInput(nameof(this.performance.ReInspectionTimeString));
    private void AddRequiredInspectionInput()
        => this.CreateInput(nameof(this.performance.RequiredInspectionTimeString));
    private void AddCompulsoryInspectionInput()
        => this.CreateInput(nameof(this.performance.CompulsoryRequiredInspectionTimeString));
    private void AddArrivalText()
        => this.CreateText(nameof(this.performance.ArrivalTimeString));
    private void AddInspectionText()
        => this.CreateText(nameof(this.performance.InspectionTimeString));
    private void AddReInspectionText()
        => this.CreateText(nameof(this.performance.ReInspectionTimeString));
    private void AddRequiredInspectionText()
        => this.CreateText(nameof(this.performance.RequiredInspectionTimeString));
    private void AddCompulsoryInspectionText()
        => this.CreateText(nameof(this.performance.CompulsoryRequiredInspectionTimeString));
    private void AddNextStartTime()
        => this.CreateText(nameof(this.performance.NextStartTimeString), true);
    private void AddRecovery()
        => this.CreateText(nameof(this.performance.RecoverySpanString));
    private void AddTime()
        => this.CreateText(nameof(this.performance.TimeString));
    private void AddAverageSpeed()
        => this.CreateText(nameof(this.performance.AverageSpeedString));
    private void AddAverageSpeedTotal()
        => this.CreateText(nameof(this.performance.AverageSpeedTotalString));
    private void AddEdit()
    {
        var style = ControlsHelper.GetStyle("Button-Table");
        var button = new Button
        {
            Style = style,
            Content = EDIT,
            Command = new DelegateCommand(this.performance.EditAction),
        };
        var border = this.CreateCell(button);
        this.Children.Add(border);
    }

    private Border CreateCell(UIElement content)
    {
        var style = ControlsHelper.GetStyle("Border-Performance-Cell");
        var border = new Border
        {
            Style = style,
            Child = content,
        };
        return border;
    }

    private void CreateText(string value, bool bold = false)
    {
        var style = ControlsHelper.GetStyle("Text");
        var text = new TextBlock
        {
            Style = style,
        };
        if (bold)
        {
            text.FontWeight = FontWeights.Bold;
        }
        var binding = new Binding(value) { Source = this.performance };
        text.SetBinding(TextBlock.TextProperty, binding);
        var border = this.CreateCell(text);
        this.Children.Add(border);
    }

    private void CreateInput(string propertyName)
    {
        var style = ControlsHelper.GetStyle("TextBox-Table");
        var input = new TextBox
        {
            Style = style,
            DataContext = this.performance,
            BorderThickness = new Thickness(0),
        };
        input.SetBinding(TextBox.TextProperty, new Binding(propertyName));
        var border = this.CreateCell(input);
        this.Children.Add(border);
    }
}
