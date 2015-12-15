// Author: Kim Dam Grønhøj, Nick S. Reese
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.Models
{
    public class Director
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public String Name { get; set; }
    }
}
