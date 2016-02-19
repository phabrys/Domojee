using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Jeedom.Model
{
    [DataContract]
    public class Event
    {
        [DataMember]
        public string name;

        [DataMember]
        public double datetime;

        [DataMember]
        public Option option;
    }
}