using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Jeedom.Api.Json.Response
{
    [DataContract]
    public class ResponseString : Response
    {
        [DataMember]
        public string result;
    }
}