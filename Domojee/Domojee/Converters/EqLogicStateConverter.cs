using System;
using Windows.UI.Xaml.Data;

namespace Domojee.Converters
{
    internal class EqLogicStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = value as string;
            if (state == "1")
                return "\uEB4F";
            else
                return "\uEA80";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}