using Domojee.Helpers;
using Domojee.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace Domojee.Views
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        public Shell()
        {
            this.InitializeComponent();
            hamburgerMenu.ItemsSource = NavigationService.MenuItems;
            hamburgerMenu.OptionsItemsSource = NavigationService.OptionMenuItems;
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += Shell_BackRequested;
            NavigationService.ContentFrame = contentFrame;
            NavigationService.Navigate(typeof(FavoritePage));
        }

        private void Shell_BackRequested(object sender, BackRequestedEventArgs e)
        {
            NavigationService.GoBack();
            e.Handled = true;
        }

        private void hamburgerMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as MenuItem;
            NavigationService.Navigate(item.PageType);
        }

        private void hamburgerMenu_OptionsItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as MenuItem;
            NavigationService.Navigate(item.PageType);
        }
    }
}
