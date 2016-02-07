using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace BackgroundWorker.Helpers
{
    [DataContract]
    public class ResponseEqLogicList : Response
    {
        [DataMember]
        public ObservableCollection<EqLogic> result;
    }
}