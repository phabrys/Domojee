using Jeedom.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Domojee.Selectors
{
    internal class EqLogicTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ZWaveEqLogicTemplate { get; set; }
        public DataTemplate SonosEqLogicTemplate { get; set; }
        public DataTemplate OnOffEqLogicTemplate { get; set; }
        public DataTemplate EqLogicTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var eq = item as EqLogic;
            var element = container as FrameworkElement;

            // Cherche si on a spécifié un Template dans les customParameters
            if (eq.display != null)
                if (eq.display.customParameters != null)
                    if (eq.display.customParameters.DomojeeTemplate != null)
                        switch (eq.display.customParameters.DomojeeTemplate.ToLower())
                        {
                            case "sonos":
                                return SonosEqLogicTemplate;

                            case "onoff":
                                return OnOffEqLogicTemplate;

                            default:
                                return EqLogicTemplate;
                        }

            // Cherche par rapport aux commandes
            //TODO : Voir "generic_type" : https://www.jeedom.com/forum/viewtopic.php?f=112&t=15155#p278226

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