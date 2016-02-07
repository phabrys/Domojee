using System.Runtime.Serialization;

namespace BackgroundWorker.Helpers
{
    [DataContract]
    public class CommandResult
    {
        [DataMember]
        public string value;

        [DataMember]
        public string collectDate;
    }
}