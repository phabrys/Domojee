﻿using System.Runtime.Serialization;

namespace Jeedom.Api.Json.Response
{
    [DataContract]
    public class Response<T>
    {
        [DataMember]
        public string id;

        [DataMember]
        public string jsonrpc;

        [DataMember]
        public T result;
    }
}