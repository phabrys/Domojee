using System.Runtime.Serialization;

namespace Jeedom.Model
{
    [DataContract]
    public class Parameters
    {
        [DataMember]
        public string apikey;

        [DataMember(IsRequired = false)]
        public string eqLogic_id;

        [DataMember]
        public string id;

        [DataMember]
        public string name;
        [DataMember]
        public string query;
        [DataMember]
        public ParametersOption options;
        
        [DataMember(IsRequired = false)]
        public string state;

        [DataMember(IsRequired = false)]
        public string object_id;

        [DataMember(IsRequired = false)]
        public double datetime;
    }
}