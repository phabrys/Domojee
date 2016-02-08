using Jeedom.Api.Json;
using Jeedom.Api.Json.Response;
using Jeedom.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

namespace Jeedom
{
    public class RequestViewModel : INotifyPropertyChanged
    {
        static private RequestViewModel _instance;

        private RequestViewModel()
        { }

        static public RequestViewModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new RequestViewModel();
            }
            return _instance;
        }

        static public StorageFolder ImageFolder = ApplicationData.Current.LocalFolder;
        static public ObservableCollection<Message> MessageList = new ObservableCollection<Message>();
        static public ObservableCollection<EqLogic> EqLogicList = new ObservableCollection<EqLogic>();
        static public ObservableCollection<Command> CommandList = new ObservableCollection<Command>();
        static public ObservableCollection<JdObject> ObjectList = new ObservableCollection<JdObject>();
        static public ObservableCollection<Scene> SceneList = new ObservableCollection<Scene>();

        public CancellationTokenSource tokenSource;

        private bool _updating = false;

        public bool Updating
        {
            get
            {
                return _updating;
            }
            set
            {
                _updating = value;
                NotifyPropertyChanged();
            }
        }

        public bool Populated = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async Task<Error> DownloadObjects()
        {
            var jsonrpc = new JsonRpcClient();

            if (await jsonrpc.SendRequest("object::all"))
            {
                ObjectList.Clear();

                List<string> idList = new List<string>();
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseJdObjectList>();
                foreach (JdObject o in response.result)
                {
                    ObjectList.Add(o);
                    idList.Add("dmj" + o.id);
                    UpdateObjectImage(o);
                }

                // Efface les images inutiles
                var files = await ImageFolder.GetFilesAsync();
                foreach (StorageFile f in files)
                {
                    if (!idList.Contains(f.DisplayName))
                    {
                        await f.DeleteAsync();
                    }
                }

                JdObject fakeobj = new JdObject();
                fakeobj.name = "Equipements sans objet parent";
                ObjectList.Add(fakeobj);
            }

            return jsonrpc.Error;
        }

        private async void UpdateObjectImage(JdObject obj)
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
        }

        public static void UpdateObjectImage(string id, string name)
        {
            var objs = from o in ObjectList where o.id == id select o;
            if (objs.Count() != 0)
            {
                var obj = objs.First();
                if (name == null)
                    obj.Image = "ms-appx:///Images/WP.jpg";
                else
                    obj.Image = "ms-appdata:///local/" + name;
            }
        }

        public async Task<Error> DownloadEqLogics()
        {
            var jsonrpc = new JsonRpcClient();

            if (await jsonrpc.SendRequest("eqLogic::all"))
            {
                EqLogicList.Clear();
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseEqLogicList>();
                EqLogicList = response.result;
                foreach (EqLogic eq in EqLogicList)
                {
                    // Recherche l'objet parent
                    var _objectlist = from o in ObjectList where o.id == eq.object_id select o;
                    var _obj = _objectlist.First();
                    if (_obj.eqLogics == null)
                        _obj.eqLogics = new ObservableCollection<EqLogic>();

                    eq.Parent = _obj;
                    _obj.eqLogics.Add(eq);
                }

                // Efface le faux objet contenant les eqlogics avec object_id == null
                var objectlist = from o in ObjectList where o.id == null select o;
                var obj = objectlist.First();

                if (obj?.eqLogics == null)
                    ObjectList.Remove(obj);
            }

            return jsonrpc.Error;
        }

        public async Task<Error> DownloadScenes()
        {
            var jsonrpc = new JsonRpcClient();

            if (await jsonrpc.SendRequest("scenario::all"))
            {
                SceneList.Clear();
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseSceneList>();
                SceneList = response.result;
            }

            return jsonrpc.Error;
        }

        public async Task<Error> DownloadMessages()
        {
            var jsonrpc = new JsonRpcClient();

            if (await jsonrpc.SendRequest("message::all"))
            {
                MessageList.Clear();
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseMessageList>();
                MessageList = response.result;
            }

            return jsonrpc.Error;
        }

        public async Task<Error> DownloadCommands()
        {
            var jsonrpc = new JsonRpcClient();

            if (await jsonrpc.SendRequest("cmd::all"))
            {
                CommandList.Clear();
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseCommandList>();
                CommandList = response.result;
                foreach (Command cmd in CommandList)
                {
                    var eqlist = from eq in EqLogicList where eq.id == cmd.eqLogic_id select eq;
                    var parenteq = eqlist.First();
                    if (parenteq.cmds == null)
                        parenteq.cmds = new ObservableCollection<Command>();
                    parenteq.cmds.Add(cmd);
                    cmd.Parent = parenteq;
                    if (cmd.name == "On")
                        parenteq.OnVisibility = true;

                    if (cmd.type == "info")
                        await ExecuteCommand(cmd);
                }
            }

            return jsonrpc.Error;
        }

        public async Task<Error> DownloadInteraction()
        {
            var jsonrpc = new JsonRpcClient();
            //Ajouter le téléchargemnent et la mise a jours des interaction Jeedom
            try
            {
                var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///DomojeeVoiceCommandes.xml"));
                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(storageFile);

                VoiceCommandDefinition commandDefinitions;

                string countryCode = CultureInfo.CurrentCulture.Name.ToLower();
                if (countryCode.Length == 0)
                {
                    countryCode = "fr-fr";
                }

                if (VoiceCommandDefinitionManager.InstalledCommandDefinitions.TryGetValue("DomojeeCommandSet_" + countryCode, out commandDefinitions))
                {
                    List<string> ObjectsList = new List<string>();
                    foreach (var jdObject in ObjectList)
                    {
                        ObjectsList.Add(jdObject.name);
                    }
                    await commandDefinitions.SetPhraseListAsync("Object", ObjectsList);
                    List<string> CommandesList = new List<string>();
                    foreach (var jdCommand in CommandList)
                    {
                        ObjectsList.Add(jdCommand.name);
                    }
                    await commandDefinitions.SetPhraseListAsync("Commande", ObjectsList);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Updating Phrase list for VCDs: " + ex.ToString());
            }

            return jsonrpc.Error;
        }

        public void SendNotificationUri(string uri)
        {
            try
            {
                var config = new ConfigurationViewModel();

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri(config.Address + "/plugins/pushNotification/php/");

                httpClient.GetAsync("updatUri.php?api=" + config.ApiKey + "&id=" + config.NotificationObjectId + "&uri=" + uri);
                httpClient.Dispose();
                
            }
            catch
            {
            }
        }
        public async Task<bool> SendPosition(string position)
        {
            try
            {
                var config = new ConfigurationViewModel();

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.BaseAddress = new Uri(config.Address + "/core/api/");

                //Envoie de l'information a jeedom sous la forme 
                //#URL_JEEDOM#/core/api/jeeApi.php?api=#API_KEY#&type=geoloc&id=#ID_CMD#&value=%LOCN
                httpClient.GetAsync("jeeApi.php?api=" + config.ApiKey + "&type=geoloc&id=" + config.GeolocObjectId + "&value=" + position);
                httpClient.Dispose();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> Shutdown()
        {
            var jsonrpc = new JsonRpcClient();

            await jsonrpc.SendRequest("jeeNetwork::halt");

            if (jsonrpc.Error == null)
                return true;
            else
                return false;
        }

        public async Task<bool> Upgrade()
        {
            var jsonrpc = new JsonRpcClient();

            await jsonrpc.SendRequest("update::update");

            if (jsonrpc.Error == null)
                return true;
            else
                return false;
        }

        public async Task<bool> Reboot()
        {
            var jsonrpc = new JsonRpcClient();

            await jsonrpc.SendRequest("jeeNetwork::reboot");

            if (jsonrpc.Error == null)
                return true;
            else
                return false;
        }

        public async Task UpdateEqLogic(EqLogic eq)
        {
            var infoCmds = from cmd in eq.cmds where cmd.type == "info" select cmd;
            foreach (Command cmd in infoCmds)
            {
                if (!cmd.Updating)
                {
                    cmd.Updating = true;
                    await ExecuteCommand(cmd);
                    cmd.Updating = false;
                }
            }
        }

        public async Task UpdateObject(JdObject obj)
        {
            var parameters = new Parameters();
            parameters.object_id = obj.id;
            var jsonrpc = new JsonRpcClient(parameters);

            if (await jsonrpc.SendRequest("eqLogic::byObjectId"))
            {
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseEqLogicList>();
                foreach (EqLogic eq in response.result)
                {
                    var lst = from e in EqLogicList where e.id == eq.id select e;
                    if (lst.Count() != 0)
                    {
                        var eqold = lst.First();
                        eqold = eq;
                    }
                    else
                    {
                        EqLogicList.Add(eq);
                        obj.eqLogics.Add(eq);
                    }
                }
            }
        }

        public async Task UpdateObjectList()
        {
            var jsonrpc = new JsonRpcClient();

            if (await jsonrpc.SendRequest("object::all"))
            {
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseJdObjectList>();
                foreach (JdObject obj in response.result)
                {
                    var lst = from o in ObjectList where o.id == obj.id select o;
                    if (lst.Count() != 0)
                    {
                        var ob = lst.First();
                        ob = obj;
                    }
                    else
                        ObjectList.Add(obj);
                }
            }
        }

        private async Task UpdateScene(Scene scene)
        {
            var parameters = new Parameters();
            parameters.id = scene.id;
            var jsonrpc = new JsonRpcClient(parameters);

            if (await jsonrpc.SendRequest("scenario::byId"))
            {
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseScene>();
                scene.lastLaunch = response.result.lastLaunch;
            }
        }

        public async Task RunScene(Scene scene)
        {
            var parameters = new Parameters();
            parameters.id = scene.id;
            parameters.state = "run";
            var jsonrpc = new JsonRpcClient(parameters);

            if (await jsonrpc.SendRequest("scenario::changeState"))
            {
                await UpdateScene(scene);
            }
        }

        public async Task ExecuteCommand(Command cmd)
        {
            var parameters = new Parameters();
            parameters.id = cmd.id;
            parameters.name = cmd.name;
            var jsonrpc = new JsonRpcClient(parameters);

            if (await jsonrpc.SendRequest("cmd::execCmd"))
            {
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseCommand>();
                cmd._value = response.result.value;
            }
            else
            {
                cmd._value = "N/A";
            }
        }
    }
}