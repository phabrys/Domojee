using Domojee.Helpers;
using Domojee.Services;
using Jeedom;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (RequestViewModel.config.Populated && RequestViewModel.config.ConnexionAuto)
            {
                if (RequestViewModel.config.IsDemoEnabled)
                    RequestViewModel.Instance.LaunchDemo();
                else
                    await RequestViewModel.Instance.FirstLaunch();

                NavigationService.Navigate(typeof(FavoritePage));
            }
            else
                NavigationService.Navigate(typeof(SettingPage));

            //Lancer le dispatchertimer toutes les 10 secondes
            var _dispatcher = new DispatcherTimer();
            _dispatcher.Interval = TimeSpan.FromSeconds(10);
            _dispatcher.Tick += _dispatcher_Tick;
            _dispatcher.Start();
            base.OnNavigatedTo(e);
        }

        private async void _dispatcher_Tick(object sender, object e)
        {
            await RequestViewModel.Instance.UpdateTask();
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

        private void Shell_BackRequested(object sender, BackRequestedEventArgs e)
        {
            NavigationService.GoBack();
            e.Handled = true;
        }
    }
}