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
using Windows.UI.Xaml.Navigation;

namespace Domojee.ViewModels
{
    internal class DashboardPageViewModel : ViewModelBase
    {
        public static DashboardPageViewModel Instance { get; private set; }

        public ObservableCollection<JdObject> ObjectList { get; set; }
        public ObservableCollection<EqLogic> EqLogicList { get; set; }
        public ObservableCollection<Command> CommandList { get; set; }

        //public StorageFolder ImageFolder;
        //private string ImageFolderName = "Images";

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public DashboardPageViewModel()
        {
            Instance = this;

            ObjectList = new ObservableCollection<JdObject>();
            EqLogicList = new ObservableCollection<EqLogic>();
            CommandList = new ObservableCollection<Command>();
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
            }

            /*try
            {
                ImageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(ImageFolderName);
            }
            catch (FileNotFoundException)
            {
                try
                {
                    await ApplicationData.Current.LocalFolder.CreateFolderAsync(ImageFolderName);
                }
                catch (Exception)
                {
                    //TODO: désactiver l'utilisation des images dans le cas où on ne peut pas créer le dossier Images
                }
            }*/

            var taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
            await taskFactory.StartNew(() => DoWork(tokenSource), tokenSource.Token);

            return;
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            tokenSource.Cancel();
            tokenSource.Dispose();
            return base.OnNavigatingFromAsync(args);
        }

        private async Task DoWork(CancellationTokenSource tokenSource)
        {
            while (!tokenSource.IsCancellationRequested)
            {
                Views.Shell.SetBusy(true, "Téléchargement des Objets");
                await DownloadObjects();
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

        /*private async void UpdateObjectImage(JdObject obj)
        {
            try
            {
                var file = await ImageFolder.GetFileAsync("dmj" + obj.id);
                obj.Image = "ms-appdata:///local/" + file.DisplayName;
            }
            catch (Exception)
            {
                obj.Image = "ms-appx:///Images/WP.jpg";
            }
        }*/

        public async Task<Error> DownloadObjects()
        {
            var jsonrpc = new JsonRpcClient();
            ObjectList.Clear();
            CommandList.Clear();
            EqLogicList.Clear();

            if (await jsonrpc.SendRequest("object::full"))
            {
                List<string> idList = new List<string>();
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseJdObjectList>();
                if (response.result != null)
                {
                    foreach (JdObject o in response.result)
                    {
                        ObjectList.Add(o);
                        idList.Add("dmj" + o.id);
                        //UpdateObjectImage(o);
                        if (o.eqLogics != null)
                        {
                            foreach (EqLogic eq in o.eqLogics)
                            {
                                EqLogicList.Add(eq);
                                if (eq.cmds != null)
                                {
                                    foreach (Command cmd in eq.cmds)
                                    {
                                        CommandList.Add(cmd);
                                    }
                                }
                            }
                        }
                    }

                    JdObject fakeobj = new JdObject();
                    fakeobj.name = "Autres";
                    //UpdateObjectImage(fakeobj);
                    ObjectList.Add(fakeobj);
                    fakeobj.eqLogics = new ObservableCollection<EqLogic>();

                    // Récupère les EqLogic du fake (object_id==null)
                    if (await jsonrpc.SendRequest("eqLogic::byObjectId"))
                    {
                        var responseEqLogic = jsonrpc.GetRequestResponseDeserialized<ResponseEqLogicList>();
                        if (responseEqLogic != null)
                        {
                            foreach (EqLogic eq in responseEqLogic.result)
                            {
                                var param = new Parameters();
                                param.id = eq.id;
                                jsonrpc.SetParameters(param);
                                if (await jsonrpc.SendRequest("eqLogic::fullById"))
                                {
                                    var responseEq = jsonrpc.GetRequestResponseDeserialized<ResponseEqLogic>();
                                    if (responseEq.result?.cmds != null)
                                        eq.cmds = responseEq.result.cmds;
                                    else
                                        eq.cmds = new ObservableCollection<Command>();
                                    fakeobj.eqLogics.Add(eq);
                                    EqLogicList.Add(eq);
                                    foreach (Command cmd in eq.cmds)
                                    {
                                        CommandList.Add(cmd);
                                    }
                                }
                            }
                        }
                    }

                    // Efface les images inutiles
                    /*var files = await ImageFolder.GetFilesAsync();
                    foreach (StorageFile f in files)
                    {
                        if (!idList.Contains(f.DisplayName))
                        {
                            await f.DeleteAsync();
                        }
                    }*/
                }
                else
                {
                    if (ObjectList == null)
                        ObjectList = new ObservableCollection<JdObject>();
                    else
                        ObjectList.Clear();

                    if (EqLogicList == null)
                        EqLogicList = new ObservableCollection<EqLogic>();
                    else
                        EqLogicList.Clear();

                    if (CommandList == null)
                        CommandList = new ObservableCollection<Command>();
                    else
                        CommandList.Clear();
                }
            }

            return jsonrpc.Error;
        }
    }
}