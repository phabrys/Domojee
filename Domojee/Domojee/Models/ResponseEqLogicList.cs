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
    public class ResponseEqLogicList : Response
    {
        [DataMember]
        public ObservableCollection<EqLogic> result;
    }
}
