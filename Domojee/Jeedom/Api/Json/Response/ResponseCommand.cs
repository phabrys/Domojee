using Jeedom.Model;
using System.Runtime.Serialization;

namespace Jeedom.Api.Json.Response
{
    [DataContract]
    public class ResponseCommand : Response
    {
        [DataMember]
        public CommandResult result;
    }
}