using System;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace Jeedom.Api.Http
{
    internal class HttpRpcClient
    {
        private string _path;

        public HttpRpcClient(string path)
        {
            _path = path;
        }

        public async Task<bool> SendRequest()
        {
            try
            {
                var config = new ConfigurationViewModel();
                var uri = new Uri(config.Uri + _path);

                var filter = new HttpBaseProtocolFilter();
                if (config.IsSelfSigned == true)
                {
                    filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
                    filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
                }

                var httpClient = new HttpClient(filter);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("text/plain"));
                httpClient.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue(new HttpProductHeaderValue("Mozilla", "5.0").ToString()));
                httpClient.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue(new HttpProductHeaderValue("Firefox", "26.0").ToString()));

                var reponse = await httpClient.GetAsync(uri);
                httpClient.Dispose();
                return reponse.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}