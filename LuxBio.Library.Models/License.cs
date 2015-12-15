// Author: Kim Dam Grønhøj, Nick S. Reese
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.Models
{
    public class License
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime ExpiredDate { get; set; }
    }
}
