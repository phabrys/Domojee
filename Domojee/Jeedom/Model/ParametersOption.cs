using System.Runtime.Serialization;

namespace Jeedom.Model
{
    [DataContract]
    public class ParametersOption
    {
        [DataMember]
        public string slider;
        [DataMember]
        public string title;
        [DataMember]
        public string message;
        [DataMember]
        public string color;
    }
}