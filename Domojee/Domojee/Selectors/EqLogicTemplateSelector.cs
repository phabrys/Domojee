using Domojee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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