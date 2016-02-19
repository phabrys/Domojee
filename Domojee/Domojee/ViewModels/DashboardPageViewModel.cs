using Jeedom;
using Jeedom.Api.Json;
using Jeedom.Api.Json.Response;
using Jeedom.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Domojee.ViewModels
{
    public class DashboardPageViewModel : ViewModelBase
    {
        public static DashboardPageViewModel Instance { get; private set; }

        public ObservableCollection<JdObject> ObjectList => RequestViewModel.Instance.ObjectList;
        public ObservableCollection<EqLogic> EqLogicList => RequestViewModel.Instance.EqLogicList;
        public ObservableCollection<Command> CommandList => RequestViewModel.Instance.CommandList;
        public bool Updating => RequestViewModel.Instance.Updating;

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public DashboardPageViewModel()
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

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            tokenSource.Cancel();
            tokenSource.Dispose();
            return base.OnNavigatingFromAsync(args);
        }
    }
}