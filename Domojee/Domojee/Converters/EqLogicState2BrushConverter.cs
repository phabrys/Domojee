using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Domojee.Converters
{
    internal class EqLogicState2BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = value as string;
            var brush = new SolidColorBrush();
            if (state == "1")
                brush.Color = Colors.LightGreen;
            else
                brush.Color = Colors.LightCoral;
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}