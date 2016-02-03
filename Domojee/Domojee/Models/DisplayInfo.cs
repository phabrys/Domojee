using System.Runtime.Serialization;

namespace Domojee.Models
{
    [DataContract]
    public class DisplayInfo
    {
        [DataMember]
        public string Icon;

        [DataMember]
        public string TagColor;

        [DataMember]
        public string TagTextColor;
    }
}