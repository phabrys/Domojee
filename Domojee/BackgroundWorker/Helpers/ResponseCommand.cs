using System.Runtime.Serialization;

namespace BackgroundWorker.Helpers
{
    [DataContract]
    public class ResponseCommand : Response
    {
        [DataMember]
        public CommandResult result;

        [DataMember]
        public Error error;
    }
}