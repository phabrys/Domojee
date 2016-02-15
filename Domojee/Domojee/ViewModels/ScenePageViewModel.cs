using Jeedom.Api.Json;
using Jeedom.Api.Json.Response;
using Jeedom.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Navigation;

namespace Domojee.ViewModels
{
    internal class ScenePageViewModel : ViewModelBase
    {
        public static ScenePageViewModel Instance { get; private set; }

        public ObservableCollection<Scene> SceneList { get; set; }

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public ScenePageViewModel()
        {
            Instance = this;
            SceneList = new ObservableCollection<Scene>();
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }

            var taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
            await taskFactory.StartNew(() => DoWork(tokenSource), tokenSource.Token);

            return;
        }

        private async Task DoWork(CancellationTokenSource tokenSource)
        {
            while (!tokenSource.IsCancellationRequested)
            {
                Views.Shell.SetBusy(true, "Téléchargement des Scénarios");
                await DownloadScenes();
                Views.Shell.SetBusy(false);

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(2), tokenSource.Token);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public async Task<Error> DownloadScenes()
        {
            var jsonrpc = new JsonRpcClient();

            if (await jsonrpc.SendRequest("scenario::all"))
            {
                SceneList.Clear();
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseSceneList>();
                if (response != null)
                {
                    foreach (Scene s in response.result)
                    {
                        SceneList.Add(s);
                    }
                }
            }

            return jsonrpc.Error;
        }
    }
}