using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace BackgroundWorker.Helpers
{
    [DataContract]
    internal class ResponseMessageList : Response
    {
        [DataMember]
        public ObservableCollection<Message> result;
    }
}