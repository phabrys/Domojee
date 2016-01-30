using Domojee.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Domojee.ViewModels
{
    public class RequestViewModel : INotifyPropertyChanged
    {
        static private RequestViewModel _instance;

        private RequestViewModel()
        { }

        static internal RequestViewModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new RequestViewModel();
            }
            return _instance;
        }

        private int Id;

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
        private CancellationTokenSource TokenSource;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private async Task<String> Request(HttpClient httpclient, string cmd, Parameters parms)
        {

            Request requete;
            requete = new Request();
            requete.parameters = parms;
            requete.method = cmd;
            requete.id = Interlocked.Increment(ref Id);

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Request));
            MemoryStream stream = new MemoryStream();
            ser.WriteObject(stream, requete);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            var request = "request=" + sr.ReadToEnd();
            var content = new StringContent(request, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await httpclient.PostAsync("jeeApi.php", content);
            var serialized = await response.Content.ReadAsStringAsync();
            return serialized;
        }

        private HttpClient GetNewHttpClient()
        {
            var config = new ConfigurationViewModel();
            HttpClient httpclient = new HttpClient();
            httpclient.DefaultRequestHeaders.Accept.Clear();
            httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpclient.BaseAddress = new Uri(config.Address + "/core/api/");
            return httpclient;
        }

        public async Task<Error> DownloadObjects()
        {
            var config = new ConfigurationViewModel();
            var parameters = new Parameters();
            parameters.apikey = config.ApiKey;

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                var serialized = await Request(httpclient, "object::all", parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseJdObjectList));
                ResponseJdObjectList resp = ser.ReadObject(stream) as ResponseJdObjectList;

                if (resp.error == null)
                {
                    ObjectList.Clear();
                    JdObject obj = new JdObject();
                    obj.name = "Equipements sans objet parent";
                    ObjectList.Add(obj);
                    resp.result.ToList().ForEach(p =>
                    {
                        ObjectList.Add(p);
                        UpdateObjectImage(p);
                    });
                }

                return resp.error;
            }
            catch (Exception)
            {
                Error error = new Error();
                error.code = "-1";
                error.message = "Une erreur s'est produite lors de l'exécution de votre requête!";
                return error;
            }
        }

        private async void UpdateObjectImage(JdObject obj)
        {
            try
            {
                var file = await ApplicationData.Current.RoamingFolder.GetFileAsync("dmj" + obj.id);
                obj.Image = "ms-appdata:///roaming/" + file.DisplayName;
            }
            catch (Exception)
            {
                obj.Image = "ms-appx:///Images/WP.jpg";
            }
        }

        public static void UpdateObjectImage(string id, string name)
        {
            var objs = ObjectList.Where(o => o.id == id);
            if (objs.Count() != 0)
            {
                var obj = objs.First();
                if (name == null)
                    obj.Image = "ms-appx:///Images/WP.jpg";
                else
                    obj.Image = "ms-appdata:///roaming/" + name;
            }
        }

        public async Task<Error> DownloadEqLogics()
        {
            var config = new ConfigurationViewModel();
            var parameters = new Parameters();
            parameters.apikey = config.ApiKey;

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                var serialized = await Request(httpclient, "eqLogic::all", parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseEqLogicList));
                ResponseEqLogicList resp = ser.ReadObject(stream) as ResponseEqLogicList;

                if (resp.error == null)
                {
                    EqLogicList.Clear();
                    resp.result.ToList().ForEach(p =>
                    {
                        EqLogicList.Add(p);
                        JdObject obj = ObjectList.Where(o => o.id == p.object_id).First();
                        if (obj.eqLogics == null)
                            obj.eqLogics = new ObservableCollection<EqLogic>();

                        p.Parent = obj;
                        obj.eqLogics.Add(p);
                    });
                    JdObject obje = ObjectList.Where(o => o.id == null).First();
                    if (obje?.eqLogics == null)
                        ObjectList.Remove(obje);
                }

                return resp.error;
            }
            catch (Exception)
            {
                Error error = new Error();
                error.code = "-1";
                error.message = "Une erreur s'est produite lors de l'exécution de votre requête!";
                return error;
            }
        }

        public async Task<Error> DownloadScenes()
        {
            var config = new ConfigurationViewModel();
            var parameters = new Parameters();
            parameters.apikey = config.ApiKey;

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                var serialized = await Request(httpclient, "scenario::all", parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseSceneList));
                ResponseSceneList resp = ser.ReadObject(stream) as ResponseSceneList;

                if (resp.error == null)
                {
                    SceneList.Clear();
                    resp.result.ToList().ForEach(p => SceneList.Add(p));
                }

                return resp.error;
            }
            catch (Exception)
            {
                Error error = new Error();
                error.code = "-1";
                error.message = "Une erreur s'est produite lors de l'exécution de votre requête!";
                return error;
            }
        }

        internal void Clear()
        {
            Populated = false;
            MessageList.Clear();
            SceneList.Clear();
            CommandList.Clear();
            EqLogicList.Clear();
            ObjectList.Clear();
        }

        public async Task<Error> DownloadMessages()
        {
            var config = new ConfigurationViewModel();
            var parameters = new Parameters();
            parameters.apikey = config.ApiKey;

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                var serialized = await Request(httpclient, "message::all", parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseMessageList));
                ResponseMessageList resp = ser.ReadObject(stream) as ResponseMessageList;

                if (resp.error == null)
                {
                    MessageList.Clear();
                    resp.result.ToList().ForEach(p => MessageList.Add(p));
                }

                return resp.error;
            }
            catch (Exception)
            {
                Error error = new Error();
                error.code = "-1";
                error.message = "Une erreur s'est produite lors de l'exécution de votre requête!";
                return error;
            }
        }

        public async Task<Error> DownloadCommands()
        {
            var config = new ConfigurationViewModel();
            var parameters = new Parameters();
            parameters.apikey = config.ApiKey;

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                var serialized = await Request(httpclient, "cmd::all", parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseCommandList));
                ResponseCommandList resp = ser.ReadObject(stream) as ResponseCommandList;

                if (resp.error == null)
                {
                    CommandList.Clear();
                    resp.result.ToList().ForEach(async p =>
                    {
                        CommandList.Add(p);
                        EqLogic eq = EqLogicList.Where(e => e.id == p.eqLogic_id).First();
                        if (eq.cmds == null)
                            eq.cmds = new ObservableCollection<Command>();
                        eq.cmds.Add(p);
                        p.Parent = eq;
                        if (p.name == "On")
                            eq.OnVisibility = true;

                        if (p.type == "info")
                            await ExecuteCommand(p);
                    });
                }

                return resp.error;
            }
            catch (Exception)
            {
                Error error = new Error();
                error.code = "-1";
                error.message = "Une erreur s'est produite lors de l'exécution de votre requête!";
                return error;
            }
        }

        public async Task<bool> Shutdown()
        {
            var config = new ConfigurationViewModel();
            var parameters = new Parameters();
            parameters.apikey = config.ApiKey;

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                var serialized = await Request(httpclient, "jeeNetwork::halt", parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                Response resp = ser.ReadObject(stream) as Response;
                if (resp.error != null)
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Upgrade()
        {
            var config = new ConfigurationViewModel();
            var parameters = new Parameters();
            parameters.apikey = config.ApiKey;

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                var serialized = await Request(httpclient, "update::update", parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                Response resp = ser.ReadObject(stream) as Response;
                if (resp.error != null)
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Reboot()
        {
            var config = new ConfigurationViewModel();
            var parameters = new Parameters();
            parameters.apikey = config.ApiKey;

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                var serialized = await Request(httpclient, "jeeNetwork::reboot", parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                Response resp = ser.ReadObject(stream) as Response;
                if (resp.error != null)
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void CancelThread()
        {
            TokenSource?.Cancel();
        }

        public async Task UpdateEqLogic(EqLogic eq)
        {
            var infoCmds = eq.cmds.Where(c => c.type == "info");
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

        public async Task UpdateAllAsyncThread(CancellationTokenSource tokenSource)
        {
            TokenSource = tokenSource;
            while (!tokenSource.Token.IsCancellationRequested)
            {
                Updating = true;

                //Mise à jour des commandes d'information
                if (tokenSource.Token.IsCancellationRequested)
                {
                    return;
                }
                var infoCmds = from cmd in CommandList where cmd.type == "info" select cmd;
                foreach (Command cmd in infoCmds)
                {
                    if (!cmd.Updating)
                    {
                        if (tokenSource.Token.IsCancellationRequested)
                        {
                            return;
                        }
                        cmd.Updating = true;
                        await ExecuteCommand(cmd);
                        cmd.Updating = false;
                    }
                }

                // Mise à jour des scénarios
                if (tokenSource.Token.IsCancellationRequested)
                {
                    return;
                }
                foreach (Scene scene in SceneList)
                {
                    await UpdateScene(scene);
                }

                // Mise à jour des messages
                if (tokenSource.Token.IsCancellationRequested)
                {
                    return;
                }
                await DownloadMessages();

                Updating = false;

                if (tokenSource.Token.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), tokenSource.Token);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public async Task UpdateObject(JdObject obj)
        {
            var config = new ConfigurationViewModel();
            var parameters = new Parameters();
            parameters.apikey = config.ApiKey;
            parameters.object_id = obj.id;

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                var serialized = await Request(httpclient, "eqLogic::byObjectId", parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseJdObjectList));
                ResponseEqLogicList resp = ser.ReadObject(stream) as ResponseEqLogicList;

                if (resp.error == null)
                {
                    foreach (EqLogic eq in resp.result)
                    {
                        var lst = EqLogicList.Where(p => p.id == eq.id);
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
            catch (Exception)
            {
                return;
            }
        }

        public async Task UpdateObjectList()
        {
            var config = new ConfigurationViewModel();
            var parameters = new Parameters();
            parameters.apikey = config.ApiKey;

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                var serialized = await Request(httpclient, "object::all", parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseJdObjectList));
                ResponseJdObjectList resp = ser.ReadObject(stream) as ResponseJdObjectList;

                if (resp.error == null)
                {
                    foreach (JdObject obj in resp.result)
                    {
                        var lst = ObjectList.Where(p => p.id == obj.id);
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
            catch (Exception)
            {
                return;
            }
        }

        private async Task UpdateScene(Scene scene)
        {
            var config = new ConfigurationViewModel();
            var parameters = new Parameters();
            parameters.apikey = config.ApiKey;
            parameters.id = scene.id;

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                var serialized = await Request(httpclient, "scenario::byId", parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseScene));
                ResponseScene resp = ser.ReadObject(stream) as ResponseScene;
                if (resp.error != null)
                {
                    return;
                }
                else
                {
                    scene.LastLaunch = resp.result.lastLaunch;
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task RunScene(Scene scene)
        {
            var config = new ConfigurationViewModel();
            var parameters = new Parameters();
            parameters.apikey = config.ApiKey;
            parameters.id = scene.id;
            parameters.state = "run";

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                var serialized = await Request(httpclient, "scenario::changeState", parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
                Response resp = ser.ReadObject(stream) as Response;
                if (resp.error != null)
                {
                    return;
                }
                else
                {
                    await UpdateScene(scene);
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task RunBackgroundTask(TaskScheduler taskScheduler)
        {
            var taskFactory = new TaskFactory(taskScheduler);
            tokenSource = new CancellationTokenSource();
            await taskFactory.StartNew(() => UpdateAllAsyncThread(tokenSource), tokenSource.Token);
        }

        public void StopBackgroundTask()
        {
            if (tokenSource != null)
                tokenSource.Cancel();
        }

        public async Task ExecuteCommand(Command cmd)
        {
            var config = new ConfigurationViewModel();
            var parameters = new Parameters();
            parameters.apikey = config.ApiKey;
            parameters.id = cmd.id;
            parameters.name = cmd.name;

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                var serialized = await Request(httpclient, "cmd::execCmd", parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serialized));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseCommand));
                ResponseCommand resp = ser.ReadObject(stream) as ResponseCommand;
                if (resp.error == null)
                {
                    cmd._value = resp.result.value;
                }
                else
                {
                    cmd._value = "N/A";
                }
            }
            catch (Exception)
            {
                cmd._value = "N/A";
            }
        }
    }
}
