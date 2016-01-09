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
        string EqLogicName;

        public EqLogicPage()
        {
            this.InitializeComponent();
            //SystemNavigationManager.GetForCurrentView().BackRequested += EqLogicPage_BackRequested;
        }

        private void EqLogicPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled.
            if (rootFrame.CanGoBack && e.Handled == false && Frame.BackStack.Last()?.SourcePageType.Name != "LoadingPage")
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

            if (Frame.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            var eq = e.Parameter as EqLogic;
            ActionList = eq.GetActionsCmds();
            InformationList = eq.GetInformationsCmds();
            EqLogicName = eq.name;
            if (ActionList.Count == 0)
                actionview.Visibility = Visibility.Collapsed;

            if (InformationList.Count == 0)
                infoview.Visibility = Visibility.Collapsed;

            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var taskFactory = new TaskFactory(taskScheduler);
            base.OnNavigatedTo(e);
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
