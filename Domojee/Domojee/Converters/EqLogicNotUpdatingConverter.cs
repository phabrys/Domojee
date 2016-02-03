using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Domojee.Converters
{
    internal class EqLogicNotUpdatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var updating = (bool)value;
            if (updating)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}