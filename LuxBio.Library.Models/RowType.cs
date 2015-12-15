// Author: Kim Dam Grønhøj
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.Models
{
    [DataContract(IsReference = true)]
    public class RowType
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public decimal Price { get; set; }
    }
}
