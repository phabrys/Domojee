using System.Runtime.Serialization;

namespace Jeedom.Api.Json.Response
{
    [DataContract]
    public class Response
    {
        [DataMember]
        public string id;

        [DataMember]
        public string jsonrpc;
    }
}