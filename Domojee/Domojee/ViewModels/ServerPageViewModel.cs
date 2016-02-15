using Jeedom.Api.Json;
using Jeedom.Api.Json.Response;
using Jeedom.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Navigation;

namespace Domojee.ViewModels
{
    internal class ServerPageViewModel : ViewModelBase
    {
        public static ServerPageViewModel Instance { get; set; }

        public ObservableCollection<Message> MessageList { get; set; }

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public ServerPageViewModel()
        {
            Instance = this;

            MessageList = new ObservableCollection<Message>();
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
                Views.Shell.SetBusy(true, "Téléchargement des Messages");
                await DownloadMessages();
                Views.Shell.SetBusy(false);

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

        public async Task<Error> DownloadMessages()
        {
            var jsonrpc = new JsonRpcClient();

            if (await jsonrpc.SendRequest("message::all"))
            {
                MessageList.Clear();
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseMessageList>();
                if (response != null)
                    MessageList = response.result;
            }

            return jsonrpc.Error;
        }
    }
}