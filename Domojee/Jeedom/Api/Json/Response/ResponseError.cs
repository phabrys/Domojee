using Jeedom.Model;
using System.Runtime.Serialization;

namespace Jeedom.Api.Json.Response
{
    [DataContract]
    public class ResponseError
    {
        [DataMember]
        public string id;

        [DataMember]
        public string jsonrpc;

        [DataMember]
        public Error error;
    }
}