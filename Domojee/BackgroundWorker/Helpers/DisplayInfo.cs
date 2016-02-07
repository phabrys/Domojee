using System.Runtime.Serialization;

namespace BackgroundWorker.Helpers
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