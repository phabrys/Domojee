using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Jeedom.Api.Json.Event
{
    [DataContract]
    public class Event<T> : JdEvent
    {
        [DataMember]
        public T option;
    }
}