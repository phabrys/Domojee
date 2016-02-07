using Jeedom;
using Jeedom.Model;
using System;
using System.Collections.ObjectModel;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace Domojee.Views
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ScenePage : Page
    {
        private ObservableCollection<Scene> SceneList = RequestViewModel.SceneList;

        public ScenePage()
        {
            this.InitializeComponent();
            menu.NavigateToPage += Menu_NavigateToPage;
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

        private async void gridview_ItemClick(object sender, ItemClickEventArgs e)
        {
            Scene scene = e.ClickedItem as Scene;
            scene.Updating = true;
            await RequestViewModel.GetInstance().RunScene(scene);
            scene.Updating = false;
        }
    }
}