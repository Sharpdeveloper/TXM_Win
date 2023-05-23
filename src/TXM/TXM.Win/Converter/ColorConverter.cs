using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using TXM.Core.Global;

namespace TXM.Win.Converter;

[System.Windows.Localizability(System.Windows.LocalizationCategory.NeverLocalize)]
public sealed class ColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string svalue)
        {
            return svalue switch
            {
                Literals.Red => Brushes.Red
                , Literals.Blue => Brushes.Blue
                , Literals.Green => Brushes.Green
                , Literals.Yellow => Brushes.Yellow
                , Literals.White => Brushes.White
                , Literals.Purple => Brushes.Purple
                , Literals.Orange => Brushes.Orange
                , _ => Brushes.Black
            };
        }

        return Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush svalue)
        {
            return Literals.Black;
        }

        if (svalue == Brushes.Red)
        {
            return Literals.Red;
        }

        if (svalue == Brushes.Blue)
        {
            return Literals.Blue;
        }

        if (svalue == Brushes.Green)
        {
            return Literals.Green;
        }

        if (svalue == Brushes.Yellow)
        {
            return Literals.Yellow;
        }

        if (svalue == Brushes.White)
        {
            return Literals.White;
        }

        if (svalue == Brushes.Purple)
        {
            return Literals.Purple;
        }

        if (svalue == Brushes.Orange)
        {
            return Literals.Orange;
        }

        return Literals.Black;

    }
}