using System.Runtime.Serialization;

namespace Jeedom.Model
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