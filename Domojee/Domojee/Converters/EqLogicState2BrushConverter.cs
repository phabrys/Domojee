using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Domojee.Converters
{
    class EqLogicState2BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = value as string;
            var brush = new SolidColorBrush();
            if (state == "1")
                brush.Color = Colors.Green;
            else
                brush.Color = Colors.Red;
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
