using Jeedom.Model;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Jeedom.Api.Json.Response
{
    [DataContract]
    public class ResponseJdObjectList : Response
    {
        [DataMember]
        public ObservableCollection<JdObject> result;
    }
}