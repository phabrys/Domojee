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
        public ScenePage()
        {
            this.InitializeComponent();
        }

        private async void gridview_ItemClick(object sender, ItemClickEventArgs e)
        {
            Scene scene = e.ClickedItem as Scene;
            scene.Updating = true;
            await RequestViewModel.Instance.RunScene(scene);
            scene.Updating = false;
        }
    }
}