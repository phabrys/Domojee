using System.Runtime.Serialization;

namespace Domojee.Models
{
    [DataContract]
    public class ResponseError : Response
    {
        [DataMember]
        public Error error;
    }
}