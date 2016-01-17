using Domojee.Models;
using Domojee.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Domojee.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            this.InitializeComponent();
            //SystemNavigationManager.GetForCurrentView().BackRequested += LoadingPage_BackRequested;
        }

        private void LoadingPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled.
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            prProgress.IsActive = true;

            tbInformation.Text = "Objets";
            Error error = await RequestViewModel.GetInstance().DownloadObjects();
            if (error != null)
            {
                tbInformation.Text = error.message;
                await Task.Delay(new TimeSpan(0, 0, 3));
                Frame.Navigate(typeof(ConnectPage));
                base.OnNavigatedTo(e);
                return;
            }

            tbInformation.Text = "Equipements";
            error = await RequestViewModel.GetInstance().DownloadEqLogics();
            if (error != null)
            {
                tbInformation.Text = error.message;
                await Task.Delay(new TimeSpan(0, 0, 3));
                Frame.Navigate(typeof(ConnectPage));
                base.OnNavigatedTo(e);
                return;
            }

            tbInformation.Text = "Commandes";
            error = await RequestViewModel.GetInstance().DownloadCommands();
            if (error != null)
            {
                tbInformation.Text = error.message;
                await Task.Delay(new TimeSpan(0, 0, 3));
                Frame.Navigate(typeof(ConnectPage));
                base.OnNavigatedTo(e);
                return;
            }

            tbInformation.Text = "Scénarios";
            error = await RequestViewModel.GetInstance().DownloadScenes();
            if (error != null)
            {
                tbInformation.Text = error.message;
                await Task.Delay(new TimeSpan(0, 0, 3));
                Frame.Navigate(typeof(ConnectPage));
                base.OnNavigatedTo(e);
                return;
            }

            tbInformation.Text = "Messages";
            error = await RequestViewModel.GetInstance().DownloadMessages();
            if (error != null)
            {
                tbInformation.Text = error.message;
                await Task.Delay(new TimeSpan(0, 0, 3));
                Frame.Navigate(typeof(ConnectPage));
                base.OnNavigatedTo(e);
                return;
            }

            tbInformation.Text = "Connecté";

            var instance = RequestViewModel.GetInstance();
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var taskFactory = new TaskFactory(taskScheduler);
            var tokenSource = new CancellationTokenSource();
            await taskFactory.StartNew(() => instance.UpdateAllAsyncThread(tokenSource), tokenSource.Token);

            Frame.Navigate(typeof(DashboardPage));
            base.OnNavigatedTo(e);
        }
    }
}
