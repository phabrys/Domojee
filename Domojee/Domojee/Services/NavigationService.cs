using Domojee.Helpers;
using Domojee.Views;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace Domojee.Services
{
    internal class NavigationService
    {
        static private Frame _content;

        static private List<MenuItem> _menuItems = new List<MenuItem>()
        {
            new MenuItem() { Icon = Symbol.Favorite,    Text = "Favoris",           PageType = typeof(FavoritePage),     Pos = 0 },
            new MenuItem() { Icon = Symbol.Home,        Text = "Tableau de bord",   PageType = typeof(DashboardPage),     Pos = 1 },
            new MenuItem() { Icon = Symbol.Clock,        Text = "Scénarios",         PageType = typeof(ScenePage),    Pos = 2 }
        };

        static private List<MenuItem> _optionMenuItems = new List<MenuItem>()
        {
            new MenuItem() {Icon = Symbol.Setting,      Text = "Paramètres",    PageType = typeof(SettingPage),   Pos = 0 },
            new MenuItem() {Icon = Symbol.Message,    Text = "Logs",          PageType = typeof(LogPage),         Pos = 1 },
            new MenuItem() {Icon = Symbol.Help,    Text = "A propos",          PageType = typeof(AboutPage),         Pos = 2 }
        };

        static private Page _shell;

        private NavigationService()
        {
        }

        public static Frame ContentFrame
        {
            get
            {
                return _content;
            }

            set
            {
                _content = value;
            }
        }

        public static List<MenuItem> MenuItems
        {
            get
            {
                return _menuItems;
            }

            set
            {
                _menuItems = value;
            }
        }

        public static List<MenuItem> OptionMenuItems
        {
            get
            {
                return _optionMenuItems;
            }

            set
            {
                _optionMenuItems = value;
            }
        }

        public static Page Shell
        {
            get
            {
                return _shell;
            }

            set
            {
                _shell = value;
            }
        }

        static public void GoBack()
        {
            //TODO: Ajouter la prise en charge du changement d'élément sélectionné dans le menuhamburger quand on appuie sur le bouton back
            if (ContentFrame != null)
            {
                if (ContentFrame.CanGoBack)
                {
                    ContentFrame.GoBack();
                }
            }
        }

        static public bool Navigate(Type sourcePageType)
        {
            return _content.Navigate(sourcePageType);
        }

        static public bool Navigate(Type sourcePageType, object parameter)
        {
            return _content.Navigate(sourcePageType, parameter);
        }
    }
}