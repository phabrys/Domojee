using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace BackgroundWorker.Helpers
{
    [DataContract]
    internal class ResponseSceneList : Response
    {
        [DataMember]
        public ObservableCollection<Scene> result;
    }
}