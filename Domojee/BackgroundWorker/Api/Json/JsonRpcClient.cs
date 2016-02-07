using Jeedom.Api.Json.Response;
using Jeedom.Model;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jeedom.Api.Json
{
    internal class JsonRpcClient
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

        private T DeserializeFromJson<T>(string dataToDeserialize)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(dataToDeserialize));
            stream.Position = 0;
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            return (T)ser.ReadObject(stream);
        }

        private string SerializeToJson<T>(T objectToSerialize)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            ser.WriteObject(stream, objectToSerialize);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);

            return sr.ReadToEnd();
        }

        private async Task<String> Request(string command, Parameters parameters)
        {
            var config = new ConfigurationViewModel();

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.BaseAddress = new Uri(config.Address + "/core/api/");

            Request requete = new Request();
            requete.parameters = parameters;
            requete.method = command;
            requete.id = Interlocked.Increment(ref Id);

            var requeteJson = "request=" + SerializeToJson<Request>(requete);
            var content = new StringContent(requeteJson, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await httpClient.PostAsync("jeeApi.php", content);
            var serialized = await response.Content.ReadAsStringAsync();
            httpClient.Dispose();

            return serialized;
        }

        public async Task<bool> SendRequest(string command)
        {
            var config = new ConfigurationViewModel();
            parameters.apikey = config.ApiKey;

            try
            {
                rawResponse = await Request(command, parameters);

                var resp = DeserializeFromJson<ResponseError>(rawResponse);
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

        public T GetRequestResponseDeserialized<T>()
        {
            try
            {
                var resp = DeserializeFromJson<T>(rawResponse);
                return resp;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /*public CommandResult GetCommandResult()
        {
            try
            {
                var resp = DeserializeFromJson<ResponseCommand>(rawResponse);
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
                var resp = DeserializeFromJson<ResponseCommandList>(rawResponse);
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
                var resp = DeserializeFromJson<ResponseEqLogicList>(rawResponse);
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
                var resp = DeserializeFromJson<ResponseJdObjectList>(rawResponse);
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
                var resp = DeserializeFromJson<ResponseMessageList>(rawResponse);
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
                var resp = DeserializeFromJson<ResponseSceneList>(rawResponse);
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
                var resp = DeserializeFromJson<ResponseScene>(rawResponse);
                return resp.result;
            }
            catch (Exception)
            {
                return null;
            }
        }*/
    }
}