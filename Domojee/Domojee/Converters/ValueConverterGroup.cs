using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace Domojee.Converters
{
    internal class ValueConverterGroup : List<IValueConverter>, IValueConverter
    {
        #region Public Methods

        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return this.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}