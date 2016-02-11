using Jeedom;
using Jeedom.Model;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ObjectPage : Page
    {
        public ObservableCollection<EqLogic> EqLogicList;
        public bool Updating;
        public string ObjectName;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        public JdObject Object;
        public string ImagePath;

        public ObjectPage()
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

            this.Object = e.Parameter as JdObject;

            EqLogicList = this.Object.eqLogics;
            ObjectName = this.Object.Name;
            ImagePath = this.Object.Image;

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
            //TODO: DoWork ne s'arrête pas dans certaines conditions
            while (!tokenSource.IsCancellationRequested)
            {
                Updating = true;
                Bindings.Update();
                await RequestViewModel.GetInstance().UpdateObject(this.Object);
                Updating = false;
                Bindings.Update();

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(15), tokenSource.Token);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        private void eqlogicview_ItemClick(object sender, ItemClickEventArgs e)
        {
            var eq = e.ClickedItem as EqLogic;
            Frame.Navigate(typeof(EqLogicPage), eq);
        }

        private async void btnON_Click(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            var id = element.Tag as string;
            var eq = RequestViewModel.EqLogicList.Where(eqlogic => eqlogic.id == id).First();
            string cmdName;
            if (!eq.Updating)
            {
                eq.Updating = true;
                if (eq.State == "1")
                    cmdName = "Off";
                else
                    cmdName = "On";

                if (eq != null)
                {
                    var cmd = eq.cmds?.Where(command => command.name == cmdName)?.First();
                    if (cmd != null)
                    {
                        await RequestViewModel.GetInstance().ExecuteCommand(cmd);
                        await Task.Delay(TimeSpan.FromSeconds(3));
                        await RequestViewModel.GetInstance().UpdateEqLogic(cmd.Parent);
                    }
                }
                eq.Updating = false;
            }
        }
    }
}