using Domojee.Models;
using Domojee.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace Domojee.Views
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class EqLogicPage : Page
    {
        ObservableCollection<Command> ActionList;
        ObservableCollection<Command> InformationList;
        private EqLogic eqLogic;
        string EqLogicName;
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        public bool Updating;

        public EqLogicPage()
        {
            this.InitializeComponent();
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

            eqLogic = e.Parameter as EqLogic;
            ActionList = eqLogic.GetActionsCmds();
            InformationList = eqLogic.GetInformationsCmds();
            EqLogicName = eqLogic.name;
            if (ActionList.Count == 0)
                actionview.Visibility = Visibility.Collapsed;

            if (InformationList.Count == 0)
                infoview.Visibility = Visibility.Collapsed;

            var taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
            await taskFactory.StartNew(() => DoWork(tokenSource), tokenSource.Token);

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            tokenSource.Cancel();
            tokenSource.Dispose();
            base.OnNavigatedFrom(e);
        }

        private async Task DoWork(CancellationTokenSource tokenSource)
        {
            while (!tokenSource.IsCancellationRequested)
            {
                Updating = true;
                Bindings.Update();
                await RequestViewModel.GetInstance().UpdateEqLogic(eqLogic);
                Updating = false;
                Bindings.Update();

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(2), tokenSource.Token);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        private async void commandview_ItemClick(object sender, ItemClickEventArgs e)
        {
            Command cmd = e.ClickedItem as Command;
            cmd.Updating = true;
            await RequestViewModel.GetInstance().ExecuteCommand(cmd);
            cmd.Updating = false;
        }
    }
}
