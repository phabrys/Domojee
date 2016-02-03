using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domojee.Models
{
    [DataContract]
    public class Response
    {
        [DataMember]
        public string id;

        [DataMember]
        public string jsonrpc;
    }
}
