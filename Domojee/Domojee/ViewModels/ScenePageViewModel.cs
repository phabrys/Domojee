using Jeedom;
using Jeedom.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Navigation;

namespace Domojee.ViewModels
{
    internal class ScenePageViewModel : ViewModelBase
    {
        public static ScenePageViewModel Instance { get; private set; }

        public ObservableCollection<Scene> SceneList => RequestViewModel.Instance.SceneList;

        public ScenePageViewModel()
        {
            Instance = this;
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }

            return;
        }
    }
}