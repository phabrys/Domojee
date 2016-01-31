﻿using Domojee.Models;
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
        }

        private delegate Task<Error> ActionFunction();

        private struct ActionItem
        {
            public string message;
            public ActionFunction function;
        }

        private ActionItem[] Actions = new ActionItem[] {
            new ActionItem { message = "Objets",        function = RequestViewModel.GetInstance().DownloadObjects },
            new ActionItem { message = "Equipements",   function = RequestViewModel.GetInstance().DownloadEqLogics },
            new ActionItem { message = "Commandes",     function = RequestViewModel.GetInstance().DownloadCommands },
            new ActionItem { message = "Scénarios",     function = RequestViewModel.GetInstance().DownloadScenes },
            new ActionItem { message = "Messages",      function = RequestViewModel.GetInstance().DownloadMessages }
        };

        private async Task<bool> Action(ActionItem item, NavigationEventArgs e)
        {
            tbInformation.Text = item.message;
            Error error = await item.function();
            if (error != null)
            {
                tbInformation.Text = error.message;
                await Task.Delay(new TimeSpan(0, 0, 3));
                Frame.Navigate(typeof(ConnectPage));
                base.OnNavigatedTo(e);
                return false;
            }
            else
                return true;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            prProgress.IsActive = true;

            foreach (ActionItem item in Actions)
            {
                if (!await Action(item, e))
                    return;
            }

            tbInformation.Text = "";
            tbLoading.Text = "Connecté";

            await Task.Delay(new TimeSpan(0, 0, 2));
            Frame.Navigate(typeof(DashboardPage));
            base.OnNavigatedTo(e);
        }
    }
}