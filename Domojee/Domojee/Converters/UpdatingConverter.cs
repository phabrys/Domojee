using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
namespace Domojee.Converters
{
    internal class UpdatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if ((bool)value)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            catch
            {
                try
                {
                    if ((double)value == 1 )
                        return Visibility.Visible;
                    else
                        return Visibility.Collapsed;
                }
                catch
                {
                    try
                    {
                        if ((string)value == "1")
                            return Visibility.Visible;
                        else
                            return Visibility.Collapsed;
                    }
                    catch
                    {
                        return Visibility.Collapsed;
                    }
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}