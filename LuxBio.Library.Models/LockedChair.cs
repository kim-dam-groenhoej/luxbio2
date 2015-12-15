// Author: Kim Dam Grønhøj, Gitte Nielsen
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.Models
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(Chair))]
    [KnownType(typeof(MoviePlayTime))]
    [KnownType(typeof(Customer))]
    public class LockedChair
    {
        [DataMember]
        public Chair Char { get; set; }

        [DataMember]
        public MoviePlayTime MoviePlayTime { get; set; }

        [DataMember]
        public Customer Customer { get; set; }
    }
}
