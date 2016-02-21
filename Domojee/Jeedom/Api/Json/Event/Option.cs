using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Jeedom.Api.Json.Event
{
    [DataContract]
    public class Option
    {
        [DataMember]
        public string cmd_id;

        [DataMember]
        public string value;
    }
}