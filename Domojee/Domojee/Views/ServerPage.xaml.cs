using Jeedom;
using Jeedom.Model;
using System;
using System.Collections.ObjectModel;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace Domojee.Views
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ServerPage : Page
    {
        public ObservableCollection<Message> MessageList = RequestViewModel.MessageList;

        public ServerPage()
        {
            this.InitializeComponent();
            menu.NavigateToPage += Menu_NavigateToPage;
            if (MessageList.Count == 0)
                messageText.Visibility = Visibility.Collapsed;
        }

        private void Menu_NavigateToPage(object sender, Controls.NavigateEventArgs e)
        {
            Frame.Navigate(e.Page);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }

            if (Frame.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            base.OnNavigatedTo(e);
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Grid item = e.ClickedItem as Grid;
            String tag = item.Tag as String;
            string title = "Mise à jour";
            string text = "Voulez-vous vraiment mettre à jour JEEDOM ?";

            switch (tag)
            {
                case "shutdown":
                    title = "Eteindre";
                    text = "Voulez-vous vraiment éteindre JEEDOM ?";
                    break;

                case "reboot":
                    title = "Redémarrer";
                    text = "Voulez-vous vraiment redémarrer JEEDOM ?";
                    break;
            }

            ContentDialog diag = new ContentDialog()
            {
                Title = title,
                Content = text,
                PrimaryButtonText = title,
                SecondaryButtonText = "Annuler"
            };

            ContentDialogResult r = await diag.ShowAsync();
            if (r == ContentDialogResult.Primary)
            {
                var instance = RequestViewModel.GetInstance();
                switch (tag)
                {
                    case "update":
                        await instance.Upgrade();
                        break;

                    case "reboot":
                        await instance.Reboot();
                        break;

                    case "shutdown":
                        await instance.Shutdown();
                        break;
                }
            }
        }
    }
}