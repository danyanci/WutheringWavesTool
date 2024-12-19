using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace WutheringWavesTool.Converter;

public partial class BoolToVisibilityConverter : IValueConverter
{
    public bool Reversal { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool result = false;
        if (value is bool v)
        {
            if (Reversal)
                result = !v;
            else
                result = v;
        }
        return result ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
