using System.Runtime.Serialization;

namespace Domojee.Models
{
    [DataContract]
    internal class ResponseScene : Response
    {
        [DataMember]
        public Scene result;
    }
}