using Jeedom.Api.Http;
using Jeedom.Api.Json;
using Jeedom.Api.Json.Response;
using Jeedom.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Jeedom
{
    public class RequestViewModel : INotifyPropertyChanged
    {
        static private RequestViewModel _instance;

        private int pass = 0;

        private RequestViewModel()
        { }

        static public RequestViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RequestViewModel();
                }
                return _instance;
            }
        }

        private ObservableCollection<Message> _messageList = new ObservableCollection<Message>();
        private ObservableCollection<EqLogic> _eqLogicList = new ObservableCollection<EqLogic>();
        private ObservableCollection<Command> _commandList = new ObservableCollection<Command>();
        private ObservableCollection<JdObject> _objectList = new ObservableCollection<JdObject>();
        private ObservableCollection<Scene> _sceneList = new ObservableCollection<Scene>();

        public StorageFolder ImageFolder = ApplicationData.Current.LocalFolder;

        public ObservableCollection<Message> MessageList
        {
            get { return _messageList; }
            set
            {
                _messageList = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<EqLogic> EqLogicList
        {
            get { return _eqLogicList; }
            set
            {
                _eqLogicList = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<Command> CommandList
        {
            get { return _commandList; }
            set
            {
                _commandList = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<JdObject> ObjectList
        {
            get { return _objectList; }
            set
            {
                _objectList = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<Scene> SceneList
        {
            get { return _sceneList; }
            set
            {
                _sceneList = value;
                NotifyPropertyChanged();
            }
        }

        public CancellationTokenSource tokenSource;

        private Visibility _updating;

        public Visibility Updating
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

        private string _loadingMessage;

        public string LoadingMessage
        {
            get
            {
                return _loadingMessage;
            }

            private set
            {
                _loadingMessage = value;
                NotifyPropertyChanged();
            }
        }

        private int _progress = 0;

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
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

        public async Task<Error> PingJeedom()
        {
            var jsonrpc = new JsonRpcClient();
            if (await jsonrpc.SendRequest("ping"))
            {
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseString>();
                if (response.result == "pong")
                    return null;
            }

            return jsonrpc.Error;
        }

        public async Task<Error> DownloadAll()
        {
            Updating = Visibility.Visible;

            LoadingMessage = "Chargement des Objets";
            var error = await DownloadObjects();
            if (error != null)
                return error;
            Progress += 50;

            LoadingMessage = "Chargement des Scénarios";
            error = await DownloadScenes();
            if (error != null)
                return error;
            Progress += 25;

            LoadingMessage = "Chargement des Messages";
            error = await DownloadMessages();
            if (error != null)
                return error;
            Progress += 25;

            /*LoadingMessage = "Chargement des Interactions";
            error = await DownloadInteraction();
            if (error != null)
                return error;
            Progress += 25;*/

            Updating = Visibility.Collapsed;
            return null;
        }

        public async Task FirstLaunch()
        {
            Updating = Visibility.Visible;

            foreach (EqLogic eq in EqLogicList)
            {
                await UpdateEqLogic(eq);
            }

            await DownloadInteraction();

            Updating = Visibility.Collapsed;
        }

        /// <summary>
        /// Télécharge les informations sur les JdObject, les EqLogic et les Command (sans les value)
        /// </summary>
        /// <returns>Le code et le message d'erreur de Jeedom</returns>
        public async Task<Error> DownloadObjects()
        {
            var jsonrpc = new JsonRpcClient();
            EqLogicList.Clear();
            CommandList.Clear();
            ObjectList.Clear();

            if (await jsonrpc.SendRequest("object::full"))
            {
                //List<string> idList = new List<string>();
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseJdObjectList>();
                if (response.result != null)
                {
                    foreach (JdObject o in response.result)
                    {
                        ObjectList.Add(o);
                        //idList.Add("dmj" + o.id);
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
                                else
                                    eq.cmds = new ObservableCollection<Command>();
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
            }

            return jsonrpc.Error;
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
        }

        /*public static void UpdateObjectImage(string id, string name)
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

        /*public async Task<Error> DownloadEqLogics()
        {
            var jsonrpc = new JsonRpcClient();

            if (await jsonrpc.SendRequest("eqLogic::all"))
            {
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseEqLogicList>();
                if (response.result != null)
                {
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
                else
                {
                    EqLogicList.Clear();
                }
            }

            return jsonrpc.Error;
        }*/

        public async Task<Error> DownloadScenes()
        {
            var jsonrpc = new JsonRpcClient();

            if (await jsonrpc.SendRequest("scenario::all"))
            {
                SceneList.Clear();
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseSceneList>();
                if (response != null)
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
                if (response != null)
                    MessageList = response.result;
            }

            return jsonrpc.Error;
        }

        /*public async Task<Error> DownloadCommands()
        {
            var jsonrpc = new JsonRpcClient();
            if (await jsonrpc.SendRequest("cmd::all"))
            {
                CommandList.Clear();
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseCommandList>();
                if (response != null)
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
        }*/

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

        public async Task<bool> SendNotificationUri(string uri)
        {
            var config = new ConfigurationViewModel();
            var httpRpcClient = new HttpRpcClient("/plugins/pushNotification/php/updatUri.php?api=" + config.ApiKey + "&id=" + config.NotificationObjectId + "&uri=" + uri);

            return await httpRpcClient.SendRequest();
        }

        public async Task<bool> SendPosition(string position)
        {
            var config = new ConfigurationViewModel();
            var httpRpcClient = new HttpRpcClient("/core/api/jeeApi.php?api=" + config.ApiKey + "&type=geoloc&id=" + config.GeolocObjectId + "&value=" + position);

            return await httpRpcClient.SendRequest();
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

        public async Task UpdateTask()
        {
            Updating = Visibility.Visible;
            foreach (EqLogic eq in EqLogicList)
            {
                await UpdateEqLogic(eq);
            }

            if (pass % 15 == 14)
                await DownloadMessages();
            Updating = Visibility.Collapsed;
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
                if (response.result != null)
                {
                    foreach (EqLogic eqnew in response.result)
                    {
                        var lst = from e in EqLogicList where e.id == eqnew.id select e;
                        if (lst.Count() != 0)
                        {
                            var eqold = lst.First();
                            eqnew.cmds = eqold.cmds;
                            eqold = eqnew;
                        }
                        else
                        {
                            EqLogicList.Add(eqnew);
                            obj.eqLogics.Add(eqnew);
                        }
                        await UpdateEqLogic(eqnew);
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
            cmd.Updating = true;
            var parameters = new Parameters();
            parameters.id = cmd.id;
            parameters.name = cmd.name;
            var jsonrpc = new JsonRpcClient(parameters);

            if (await jsonrpc.SendRequest("cmd::execCmd"))
            {
                var response = jsonrpc.GetRequestResponseDeserialized<ResponseCommand>();
                cmd.Value = response.result.value;
            }
            else
            {
                cmd.Value = "N/A";
            }
            cmd.Updating = false;
        }
    }
}