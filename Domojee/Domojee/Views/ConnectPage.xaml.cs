using Jeedom;
using System;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Controls;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
//TODO: Gérer l'adresse sur le dns jeedom

namespace Domojee.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConnectPage : UserControl
    {
        public ConnectPage()
        {
            this.InitializeComponent();
        }

        private async void bConnect_Click(object sender, RoutedEventArgs e)
        {
            // Tentative de connexion à Jeedom
            var error = await RequestViewModel.Instance.PingJeedom();
            if (error != null)
            {
                ErrorMsg.Text = error.message;
                return; // Problème de connexion
            }

            // Lance le rapatriement des données de Jeedom
            var taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
            await taskFactory.StartNew(async () =>
            {
                await RequestViewModel.Instance.FirstLaunch();
            });

            // Masque le dialogue de connection
            ConnectPage.HideConnectDialog();
        }

        /// <summary>
        /// Affiche le dialogue de Connection
        /// </summary>
        public static void ShowConnectDialog()
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                var modal = Window.Current.Content as ModalDialog;
                var view = modal.ModalContent as ConnectPage;
                if (view == null)
                    modal.ModalContent = view = new ConnectPage();
                modal.IsModal = true;
                view.Logo.Begin();
            });
        }

        /// <summary>
        /// Masque le dialogue de Connection
        /// </summary>
        public static void HideConnectDialog()
        {
            WindowWrapper.Current().Dispatcher.Dispatch(() =>
            {
                var modal = Window.Current.Content as ModalDialog;
                if (modal != null)
                    modal.IsModal = false;
            });
        }
    }
}