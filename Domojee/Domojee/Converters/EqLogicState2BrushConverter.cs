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
            if (state == "1" || state.ToLower() == "on")
                brush = GetColorFromHexa("#8CC152");
            else
                brush = GetColorFromHexa("#434A54");
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Convertie une couleur hexa
        /// </summary>
        /// <param name="hexaColor"></param>
        /// <returns></returns>
        public static SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            return new SolidColorBrush(
                    Color.FromArgb(
                        255,
                        System.Convert.ToByte(hexaColor.Substring(1, 2), 16),
                        System.Convert.ToByte(hexaColor.Substring(3, 2), 16),
                        System.Convert.ToByte(hexaColor.Substring(5, 2), 16)
                )
            );
        }
    }
}