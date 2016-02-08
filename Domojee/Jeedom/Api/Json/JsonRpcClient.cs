using Jeedom.Api.Json.Response;
using Jeedom.Model;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

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

            var filter = new HttpBaseProtocolFilter();
            if (config.IsSelfSigned)
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

            var requeteJson = "request=" + SerializeToJson<Request>(requete);
            var content = new HttpStringContent(requeteJson, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");
            var response = await httpClient.PostAsync(config.Uri, content);
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
    }
}