using Domojee.Models;
using Domojee.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domojee.Models
{
    class JsonRpcClient
    {
        static private int Id;
        private Parameters parameters;
        private string rawResponse;

        private Error error;
        public Error Error
        {
            get
            { return error; }
        }

        public JsonRpcClient(Parameters parameters)
        {
            this.parameters = parameters;
        }

        public JsonRpcClient()
        {
            this.parameters = new Parameters();
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

        public async Task<bool> SendRequest(string cmd)
        {
            var config = new ConfigurationViewModel();
            parameters.apikey = config.ApiKey;

            try
            {
                HttpClient httpclient = GetNewHttpClient();
                rawResponse = await Request(httpclient, cmd, parameters);
                httpclient.Dispose();
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseError));
                ResponseError resp = ser.ReadObject(stream) as ResponseError;
                stream.Position = 0;
                var other = ser.ReadObject(stream);
                error = resp.error;
                if (error == null)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                error = new Error();
                error.code = "-1";
                error.message = "Une erreur s'est produite lors de l'exécution de votre requête!";
                return false;
            }
        }

        public CommandResult GetCommandResult()
        {
            try
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseCommand));
                ResponseCommand resp = ser.ReadObject(stream) as ResponseCommand;
                return resp.result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ObservableCollection<Command> GetCommandList()
        {
            try
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseCommandList));
                ResponseCommandList resp = ser.ReadObject(stream) as ResponseCommandList;
                return resp.result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ObservableCollection<EqLogic> GetEqLogicList()
        {
            try
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseEqLogicList));
                ResponseEqLogicList resp = ser.ReadObject(stream) as ResponseEqLogicList;
                return resp.result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ObservableCollection<JdObject> GetObjectList()
        {
            try
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseJdObjectList));
                ResponseJdObjectList resp = ser.ReadObject(stream) as ResponseJdObjectList;
                return resp.result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ObservableCollection<Message> GetMessageList()
        {
            try
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseMessageList));
                ResponseMessageList resp = ser.ReadObject(stream) as ResponseMessageList;
                return resp.result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ObservableCollection<Scene> GetSceneList()
        {
            try
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseSceneList));
                ResponseSceneList resp = ser.ReadObject(stream) as ResponseSceneList;
                return resp.result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Scene GetScene()
        {
            try
            {
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse));
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseScene));
                ResponseScene resp = ser.ReadObject(stream) as ResponseScene;
                return resp.result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

