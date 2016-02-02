using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domojee.Models
{
    [DataContract]
    class ResponseInteractList
    {
        [DataMember]
        public string id;

        [DataMember]
        public ObservableCollection<Interact> result;

        [DataMember]
        public string jsonrpc;

        [DataMember]
        public Error error;
    }
}
