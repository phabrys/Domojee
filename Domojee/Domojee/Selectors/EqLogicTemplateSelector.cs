using Jeedom.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Domojee.Selectors
{
    internal class EqLogicTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ZWaveEqLogicTemplate { get; set; }
        public DataTemplate EqLogicTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var eq = item as EqLogic;
            var element = container as FrameworkElement;
            switch (eq.eqType_name)
            {
                case "openzwave":
                    return ZWaveEqLogicTemplate;

                default:
                    return EqLogicTemplate;
            }
        }
    }
}