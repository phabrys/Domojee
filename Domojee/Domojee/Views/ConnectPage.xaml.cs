using System;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Domojee.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConnectPage : Page
    {
        public ConnectPage()
        {
            this.InitializeComponent();
        }

        private void bConnect_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Views.LoadingPage));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            Logo.Begin();
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as TextBlock);
        }

        private void tbAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            FlyoutURL.Visibility = Visibility.Visible;
        }

        private void tbAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            FlyoutURL.Visibility = Visibility.Collapsed;
        }
    }
}