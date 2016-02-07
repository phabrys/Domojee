using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace BackgroundWorker.Helpers
{
    [DataContract]
    internal class ResponseCommandList : Response
    {
        [DataMember]
        public ObservableCollection<Command> result;
    }
}