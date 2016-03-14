using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Domojee.Converters
{
    internal class StateToFontAwesomeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
         {
             var state = value as string;
            if (state != null && (state == "1" || state.ToLower() == "on"))
                return "\uf00c";
            return "\uf00d";
         }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}