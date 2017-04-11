using Jeedom;
using Jeedom.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void RunScene_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var id = button.Tag as string;
            var lst = from s in RequestViewModel.Instance.SceneList where s.id == id select s;
            if (lst.Count() != 0)
            {
                var scene = lst.First();
                await RequestViewModel.Instance.RunScene(scene);
            }
        }
    }
}