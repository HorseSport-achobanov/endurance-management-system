﻿using EnduranceJudge.Gateways.Desktop.Core.Objects;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace EnduranceJudge.Gateways.Desktop.Converters;

public class SeverityToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not MessageSeverity severity)
        {
            throw new ArgumentException("Invalid value", nameof(value));
        }

        var color = new SolidColorBrush(Colors.Green);
        switch (severity)
        {
            case MessageSeverity.Warning:
                color = new SolidColorBrush(Colors.Goldenrod);
                break;
            case MessageSeverity.Error:
                color = new SolidColorBrush(Colors.DarkRed);
                break;
        }

        return color;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
