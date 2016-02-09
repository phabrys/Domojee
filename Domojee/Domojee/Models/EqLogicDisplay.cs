using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domojee.Models
{
    [DataContract]
    public class CustomParameters
    {
        [DataMember(Name = "DomojeeTemplate")]
        public string DomojeeTemplate { get; set; }

        [DataMember(Name = "DomojeeRowSpan")]
        public int DomojeeRowSpan { get; set; }

        [DataMember(Name = "DomojeeColSpan")]
        public int DomojeeColSpan { get; set; }
    }

    [DataContract]
    public class EqLogicDisplay
    {
        [DataMember(Name = "hideOndashboard")]
        public bool hideOndashboard { get; set; }

        [DataMember(Name = "hideOnmobile")]
        public bool hideOnmobile { get; set; }

        [DataMember(Name = "doNotShowNameOnDashboard")]
        public bool doNotShowNameOnDashboard { get; set; }

        [DataMember(Name = "doNotShowNameOnView")]
        public bool doNotShowNameOnView { get; set; }

        [DataMember(Name = "doNotShowNameOnMobile")]
        public bool doNotShowNameOnMobile { get; set; }

        [DataMember(Name = "doNotShowObjectNameOnView")]
        public bool doNotShowObjectNameOnView { get; set; }

        [DataMember(Name = "doNotDisplayBatteryLevelOnDashboard")]
        public bool doNotDisplayBatteryLevelOnDashboard { get; set; }

        [DataMember(Name = "doNotDisplayBatteryLevelOnView")]
        public bool doNotDisplayBatteryLevelOnView { get; set; }

        [DataMember(Name = "parameters")]
        public CustomParameters customParameters { get; set; }
    }
}