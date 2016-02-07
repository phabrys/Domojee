using System.Runtime.Serialization;

namespace BackgroundWorker.Helpers
{
    [DataContract]
    public class Error
    {
        [DataMember]
        public string code;

        [DataMember]
        public string message;
    }
}