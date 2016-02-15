using Jeedom.Model;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Jeedom.Api.Json.Response
{
    [DataContract]
    public class ResponseMessageList : Response
    {
        [DataMember]
        public ObservableCollection<Message> result;
    }
}