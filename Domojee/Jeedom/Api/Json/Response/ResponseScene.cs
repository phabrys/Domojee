using Jeedom.Model;
using System.Runtime.Serialization;

namespace Jeedom.Api.Json.Response
{
    [DataContract]
    internal class ResponseScene : Response
    {
        [DataMember]
        public Scene result;
    }
}