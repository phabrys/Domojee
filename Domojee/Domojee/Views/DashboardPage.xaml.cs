using Jeedom;
using Jeedom.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace Domojee.Views
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        public ObservableCollection<JdObject> ObjectList = RequestViewModel.ObjectList;
        public RequestViewModel RqViewModel = RequestViewModel.GetInstance();
        public bool Updating;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public DashboardPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }

            if (Frame.CanGoBack && Frame.BackStack.Last()?.SourcePageType.Name != "LoadingPage")
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

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
                await RequestViewModel.GetInstance().UpdateObjectList();
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

        private void objectview_ItemClick(object sender, ItemClickEventArgs e)
        {
            App.Current.NavigationService.Navigate(typeof(ObjectPage), e.ClickedItem);
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                var item = sender as MenuFlyoutItem;
                var id = item.Tag as string;

                RequestViewModel.UpdateObjectImage(id, null);
                var onedrivefile = await RequestViewModel.ImageFolder.CreateFileAsync("dmj" + id, CreationCollisionOption.ReplaceExisting);
                await file.CopyAndReplaceAsync(onedrivefile);
                RequestViewModel.UpdateObjectImage(id, onedrivefile.DisplayName);
            }
        }

        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = sender as MenuFlyoutItem;
                var id = item.Tag as string;
                RequestViewModel.UpdateObjectImage(id, null);
                var file = await RequestViewModel.ImageFolder.GetFileAsync("dmj" + id);
                await file.DeleteAsync();
            }
            catch (Exception) { }
        }

        private async void MenuFlyoutItem_Click_Epingler(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = sender as MenuFlyoutItem;
                var id = item.Tag as string;
                JdObject objs = RequestViewModel.ObjectList.Where(o => o.id.Equals(id)).First();
                var TileExist = SecondaryTile.Exists(objs.id);
                if (!TileExist)
                {
                    var Tile = new SecondaryTile(objs.id)
                    {
                        DisplayName = objs.Name,
                        Arguments = "Object",
                    };
                    var succes = await Tile.RequestCreateAsync();
                }
            }
            catch (Exception) { }
        }

        private void Grid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
    }
}