using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Domojee.Models
{
    [DataContract]
    internal class ResponseCommandList : Response
    {
        [DataMember]
        public ObservableCollection<Command> result;
    }
}