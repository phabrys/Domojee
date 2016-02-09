using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Domojee.Converters
{
    internal class EqLogicUpdatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var updating = (bool)value;
            if (updating)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}