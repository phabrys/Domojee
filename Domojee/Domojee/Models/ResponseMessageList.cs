using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Domojee.Models
{
    [DataContract]
    internal class ResponseMessageList : Response
    {
        [DataMember]
        public ObservableCollection<Message> result;
    }
}