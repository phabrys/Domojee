using System.Runtime.Serialization;

namespace Domojee.Models
{
    [DataContract]
    public class CommandResult
    {
        [DataMember]
        public string value;

        [DataMember]
        public string collectDate;
    }
}
