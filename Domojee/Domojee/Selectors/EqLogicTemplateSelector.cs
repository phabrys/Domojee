using Jeedom.Model;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Domojee.Selectors
{
    internal class EqLogicTemplateSelector : DataTemplateSelector
    {
        #region Public Properties

        public DataTemplate EqLogicTemplate { get; set; }
        public DataTemplate OnOffEqLogicTemplate { get; set; }
        public DataTemplate SonosEqLogicTemplate { get; set; }
        public DataTemplate ZWaveEqLogicTemplate { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var eq = item as EqLogic;
            //var element = container as FrameworkElement;

            // Cherche si on a spécifié un Template dans les customParameters de l'équipement
            /*if (eq.display != null)
                if (eq.display.customParameters != null)
                    if (eq.display.customParameters.DomojeeTemplate != null)
                        switch (eq.display.customParameters.DomojeeTemplate.ToLower())
                        {
                            case "sonos":
                                return SonosEqLogicTemplate;

                            case "onoff":
                                return OnOffEqLogicTemplate;
                        }*/

            // Cherche par rapport aux commandes de l'équipement
            //TODO : Voir "generic_type" : https://www.jeedom.com/forum/viewtopic.php?f=112&t=15155#p278226

            // Lumière OnOff
            if (ContainCmd(eq, new[] { "LIGHT_STATE", "LIGHT_ON", "LIGHT_OFF" }))
            {
                container.SetValue(VariableSizedWrapGrid.RowSpanProperty, 1);
                container.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 1);
                return OnOffEqLogicTemplate;
            }
            // Cherche par rapport au plugin
            /* switch (eq.eqType_name)
            {
                case "openzwave":
                    return ZWaveEqLogicTemplate;

                case "sonos":
                    return SonosEqLogicTemplate;
            } */
            return EqLogicTemplate;
        }

        /// <summary>
        /// Renvoie vrai si l'équipement a les commandes recherchées
        /// </summary>
        /// <param name="eq">L'équipement Jeedom</param>
        /// <param name="types">Les generic_type recherchés</param>
        /// <returns></returns>
        private static bool ContainCmd(EqLogic eq, string[] types)
        {
            int _find = 0;
            foreach (var type in types)
            {
                if (eq.Cmds != null)
                {
                    var search = eq.Cmds.Where(c => c.Display.generic_type == type);
                    if (search.Count() > 0)
                        _find += 1;
                }
            }
            return _find == types.Count();
        }

        #endregion Protected Methods
    }
}