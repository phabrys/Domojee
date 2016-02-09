using Jeedom.Model;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Jeedom.Api.Json.Response
{
    [DataContract]
    internal class ResponseSceneList : Response
    {
        [DataMember]
        public ObservableCollection<Scene> result;
    }
}