using Domojee.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Domojee.Controls
{
    [ContentProperty(Name = "ContentFrame")]
    public sealed partial class MainMenu : UserControl
    {
        public event EventHandler<NavigateEventArgs> NavigateToPage;
        public MainMenu()
        {
            this.InitializeComponent();
        }

        public object ContentFrame
        {
            get
            {
                return contentFrame.Content;
            }
            set
            {
                contentFrame.Content = value;
                var frm = value as FrameworkElement;
                /*if (frm?.Tag != null)
                    title.Text = frm.Tag as string;*/
            }
        }

        private void hamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FrameworkElement;
            var listview = sender as ListView;
            var tag = item.Tag as String;
            var ev = new NavigateEventArgs();
            switch (tag)
            {
                case "Dashboard":
                    ev.Page = typeof(Views.DashboardPage);
                    NavigateToPage(this, ev);
                    break;
                case "Scene":
                    ev.Page = typeof(Views.ScenePage);
                    NavigateToPage(this, ev);
                    break;
                case "Logout":
                    ContentDialog diag = new ContentDialog()
                    {
                        Title = "Se déconnecter",
                        Content = "Voulez-vous vraiment vous déconnecter ?",
                        PrimaryButtonText = "Se déconnecter",
                        SecondaryButtonText = "Annuler"
                    };

                    ContentDialogResult r = await diag.ShowAsync();
                    if (r == ContentDialogResult.Primary)
                    {
                        RequestViewModel.GetInstance().StopBackgroundTask();
                        ev.Page = typeof(Views.ConnectPage);
                        NavigateToPage(this, ev);
                    }
                    break;
                case "Server":
                    ev.Page = typeof(Views.ServerPage);
                    NavigateToPage(this, ev);
                    break;
                case "Config":
                    ev.Page = typeof(Views.ConfigPage);
                    NavigateToPage(this, ev);
                    break;
                default:
                    ev.Page = typeof(Views.AboutPage);
                    NavigateToPage(this, ev);
                    break;
            }
        }
    }
}
