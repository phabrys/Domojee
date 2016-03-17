using Jeedom.Api.Json.Event;
using Jeedom.Api.Json.Response;
using Jeedom.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace Jeedom.Api.Json
{
    public class JsonRpcClient
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

        public void SetParameters(Parameters parameters)
        {
            this.parameters = parameters;
        }

        public JsonRpcClient()
        {
            this.parameters = new Parameters();
        }

        /*private T DeserializeFromJson<T>(string dataToDeserialize)
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
        }*/

        private async Task<String> Request(string command, Parameters parameters)
        {
            var config = new ConfigurationViewModel();
            var uri = new Uri(config.Uri + "/core/api/jeeApi.php");

            var filter = new HttpBaseProtocolFilter();
            if (config.IsSelfSigned == true)
            {
                filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
                filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
            }

            HttpClient httpClient = new HttpClient(filter);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

            Request requete = new Request();
            requete.parameters = parameters;
            requete.method = command;
            requete.id = Interlocked.Increment(ref Id);

            var requeteJson = "request=" + JsonConvert.SerializeObject(requete);
            var content = new HttpStringContent(requeteJson, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");
            var response = await httpClient.PostAsync(uri, content);
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

                var resp = JsonConvert.DeserializeObject<ResponseError>(rawResponse);
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

        public EventResult GetEvents()
        {
            try
            {
                JObject json = JObject.Parse(rawResponse);
                var event_result = new EventResult();
                var result = json["result"].Children();
                event_result.datetime = json["result"]["datetime"].Value<double>();
                foreach (var e in json["result"]["result"].Children())
                {
                    switch (e["name"].Value<string>())
                    {
                        case "cmd::update":
                            var evcmd = JsonConvert.DeserializeObject<Event<Option>>(e.ToString());
                            event_result.result.Add(evcmd);
                            break;

                        case "eqLogic::update":
                            var eveq = JsonConvert.DeserializeObject<Event<string>>(e.ToString());
                            event_result.result.Add(eveq);
                            break;

                        default:
                            var evdef = JsonConvert.DeserializeObject<JdEvent>(e.ToString());
                            event_result.result.Add(evdef);
                            break;
                    }
                }

                return event_result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public T GetRequestResponseDeserialized<T>()
        {
            try
            {
                var resp = JsonConvert.DeserializeObject<T>(rawResponse);
                return resp;
            }
            catch (JsonException e)
            {
                return default(T);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }
    }
}