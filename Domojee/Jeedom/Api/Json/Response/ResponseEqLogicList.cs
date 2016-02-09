﻿using Jeedom.Model;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Jeedom.Api.Json.Response
{
    [DataContract]
    public class ResponseEqLogicList : Response
    {
        [DataMember]
        public ObservableCollection<EqLogic> result;
    }
}