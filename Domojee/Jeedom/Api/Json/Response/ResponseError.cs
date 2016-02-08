using Jeedom.Model;
using System.Runtime.Serialization;

namespace Jeedom.Api.Json.Response
{
    [DataContract]
    public class ResponseError : Response
    {
        [DataMember]
        public Error error;
    }
}