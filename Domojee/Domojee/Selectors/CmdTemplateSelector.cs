using Jeedom.Model;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Domojee.Selectors
{
    internal class CmdTemplateSelector : DataTemplateSelector
    {
        #region Public Properties

        public DataTemplate NumericCmdTemplate { get; set; }
        public DataTemplate BinaryCmdTemplate { get; set; }
        public DataTemplate StringCmdTemplate { get; set; }
        public DataTemplate OtherCmdTemplate { get; set; }
        public DataTemplate SliderCmdTemplate { get; set; }
        public DataTemplate MessageCmdTemplate { get; set; }
        public DataTemplate ColorCmdTemplate { get; set; }
        public DataTemplate LIGHT_STATE_Template { get; set; }
        public DataTemplate LIGHT_ON_Template { get; set; }
        public DataTemplate LIGHT_OFF_Template { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var cmd = item as Command;
            var element = container as FrameworkElement;

            // Cherche si on a spécifié un Template dans les customParameters de l'équipement
            //TODO : Voir "generic_type" : https://www.jeedom.com/forum/viewtopic.php?f=112&t=15155#p278226
            if (cmd.Display != null)
            {
                /* switch(cmd.display.generic_type)
                {
                    case "LIGHT_STATE":
                        return LIGHT_STATE_Template;

                    case "LIGHT_ON":
                        return LIGHT_ON_Template;

                    case "LIGHT_OFF":
                        return LIGHT_OFF_Template;
                } */
            }
            switch (cmd.SubType)
            {
                case "numeric":
                    return NumericCmdTemplate;

                case "binary":
                    return BinaryCmdTemplate;

                case "string":
                    return StringCmdTemplate;

                case "other":
                    return OtherCmdTemplate;

                case "slider":
                    return SliderCmdTemplate;

                case "message":
                    return MessageCmdTemplate;

                case "color":
                    return ColorCmdTemplate;
            }
            return null;
        }

        #endregion Protected Methods
    }
}