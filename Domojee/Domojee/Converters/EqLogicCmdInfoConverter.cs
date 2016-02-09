using Jeedom.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace Domojee.Converters
{
    internal class EqLogicCmdInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cmds = (ObservableCollection<Command>)value;
            var searchName = parameter.ToString();
            var searchcmd = cmds.Where(c => c.name.ToLower() == searchName.ToLower()).First();

            return searchcmd._value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}