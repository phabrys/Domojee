using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Domojee.Models
{
    [DataContract]
    public class ResponseEqLogicList : Response
    {
        [DataMember]
        public ObservableCollection<EqLogic> result;
    }
}