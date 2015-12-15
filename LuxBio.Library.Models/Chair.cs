// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using LuxBio.Library.Models.ExtraPropperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.Models
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(Row))]
    [KnownType(typeof(ExtraPropperties.Chair))]
    [KnownType(typeof(ChairAvailableType))]
    public class Chair
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public Row Row { get; set; }

        [DataMember]
        public int Number { get; set; }
    }
}
