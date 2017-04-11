using System;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, voir la page http://go.microsoft.com/fwlink/?LinkId=234238

namespace Domojee.Views
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
            PackageVersion version = Package.Current.Id.Version;
            tbVersion.Text = String.Format(": {0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            //Logo.Begin();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            /*if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }*/

            base.OnNavigatedTo(e);
        }
    }
}