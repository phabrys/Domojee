
using System.Runtime.Serialization;

namespace Domojee.Models
{
    [DataContract]
    public class Error
    {
        [DataMember]
        public string code;

        [DataMember]
        public string message;
    }
}
