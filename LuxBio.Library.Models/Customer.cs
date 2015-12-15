// Author: Kim Dam Grønhøj, Nick S. Reese
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.Models
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(User))]
    public class Customer : User
    {
        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string Email{ get; set; }

        [DataMember]
        public string Address{ get; set; }

        [DataMember]
        public CityName CityNames { get; set; }

    }
}
