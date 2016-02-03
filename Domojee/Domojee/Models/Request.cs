using System.Runtime.Serialization;

namespace Domojee.Models
{
    [DataContract]
    internal class Request
    {
        [DataMember]
        public int id;

        [DataMember]
        public string method;

        [DataMember(Name = "params")]
        public Parameters parameters;

        [DataMember]
        public string jsonrpc = "2.0";
    }
}