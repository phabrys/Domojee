using Domojee.Services;
using Jeedom;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Domojee.Views
{
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gère le click sur le bouton "Se connecter"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Connexion_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            // désactive le bouton pour ne pas se connecter plusieurs fois de suite
            button.IsEnabled = false;

            if (RequestViewModel.config.IsDemoEnabled)
            {
                RequestViewModel.Instance.LaunchDemo();
            }
            else
            {
                // Connexion à Jeedom
                var error = await RequestViewModel.Instance.ConnectJeedomByLogin();
                if (error != null)
                {
                    var dialog = new MessageDialog(error.message);
                    await dialog.ShowAsync();
                    button.IsEnabled = true;
                    return;
                }

                // Création du mobile dans le plugin
                error = await RequestViewModel.Instance.CreateEqLogicMobile();
                if (error != null)
                {
                    var dialog = new MessageDialog(error.message);
                    await dialog.ShowAsync();
                    button.IsEnabled = true;
                    return;
                }

                await RequestViewModel.Instance.FirstLaunch();
            }

            // Réactive le bouton
            button.IsEnabled = true;

            // Aller au dashboard si tout s'est bien terminé
            NavigationService.Navigate(typeof(DashboardPage));
        }

        private void Demo_Click(object sender, RoutedEventArgs e)
        {
            // Désactive le bouton le temps de mettre à jour la configuration
            var button = sender as Button;
            button.IsEnabled = false;
            RequestViewModel.Instance.LaunchDemo();

            //Réactive le bouton
            button.IsEnabled = true;
            NavigationService.Navigate(typeof(DashboardPage));
        }
    }
}