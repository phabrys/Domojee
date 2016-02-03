using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Domojee.Models
{
    [DataContract]
    public class ResponseJdObjectList : Response
    {
        [DataMember]
        public ObservableCollection<JdObject> result;
    }
}