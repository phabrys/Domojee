using System.Runtime.Serialization;

namespace BackgroundWorker.Helpers
{
    [DataContract]
    public class ResponseError : Response
    {
        [DataMember]
        public Error error;
    }
}