using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace BackgroundWorker.Helpers
{
    [DataContract]
    public class ResponseJdObjectList : Response
    {
        [DataMember]
        public ObservableCollection<JdObject> result;
    }
}