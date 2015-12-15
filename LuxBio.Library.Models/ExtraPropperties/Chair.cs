// Author: Kim Dam Grønhøj, Gitte Nielsen
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.Models.ExtraPropperties
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(Row))]
    [KnownType(typeof(Models.Chair))]
    [KnownType(typeof(ChairAvailableType))]
    public class Chair : Models.Chair
    {

        public Chair()
        {
            this.Available = ChairAvailableType.Available;
            this.OrderNumber = 0;
        }

        [DataMember]
        public ChairAvailableType Available { get; set; }

        [DataMember]
        public int OrderNumber { get; set; }
    }
}
