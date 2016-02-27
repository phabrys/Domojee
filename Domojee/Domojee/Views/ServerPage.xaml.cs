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
        public ObservableCollection<Message> MessageList = RequestViewModel.Instance.MessageList;

        public ServerPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            if (MessageList.Count == 0)
                messageText.Visibility = Visibility.Collapsed;
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
                var instance = RequestViewModel.Instance;
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