using System.Runtime.Serialization;

namespace BackgroundWorker.Helpers
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