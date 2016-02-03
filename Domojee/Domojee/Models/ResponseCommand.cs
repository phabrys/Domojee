using System.Runtime.Serialization;

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