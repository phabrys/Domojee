using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Jeedom.Api.Json.Event
{
    [DataContract]
    public class JdEvent
    {
        [DataMember]
        public string name;

        [DataMember]
        public double datetime;
    }
}