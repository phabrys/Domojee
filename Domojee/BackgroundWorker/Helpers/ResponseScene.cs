using System.Runtime.Serialization;

namespace BackgroundWorker.Helpers
{
    [DataContract]
    internal class ResponseScene : Response
    {
        [DataMember]
        public Scene result;
    }
}