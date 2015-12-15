// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.Models
{
    [KnownType(typeof(Customer))]
    [DataContract(IsReference = true)]
    public abstract class User
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public DateTime Birthday { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
