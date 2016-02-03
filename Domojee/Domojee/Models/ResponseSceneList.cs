using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Domojee.Models
{
    [DataContract]
    internal class ResponseSceneList : Response
    {
        [DataMember]
        public ObservableCollection<Scene> result;
    }
}