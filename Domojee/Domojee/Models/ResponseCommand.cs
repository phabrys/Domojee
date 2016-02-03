using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domojee.Models
{
    [DataContract]
    public class ResponseCommand : Response
    {
        [DataMember]
        public CommandResult result;

        [DataMember]
        public Error error;
    }
}
