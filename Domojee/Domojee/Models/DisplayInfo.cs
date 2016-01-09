using System.Runtime.Serialization;
using Windows.UI;
using Windows.UI.Xaml.Media;

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
