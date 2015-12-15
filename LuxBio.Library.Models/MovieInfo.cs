// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.Models
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(Director))]
    [KnownType(typeof(License))]
    public class MovieInfo
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime ProductionDate { get; set; }

        [DataMember]
        public String Image { get; set; }

        [DataMember]
        public decimal Length { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public License License { get; set; }

        [DataMember]
        public Director Director { get; set; }
    }
}
