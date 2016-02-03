using System.Runtime.Serialization;

namespace Domojee.Models
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