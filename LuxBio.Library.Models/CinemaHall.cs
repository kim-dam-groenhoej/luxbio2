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
    [KnownType(typeof(Row))]
    [KnownType(typeof(MoviePlayTime))]
    public class CinemaHall
    {
        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public List<Row> Rows { get; set; }

        [DataMember]
        public List<MoviePlayTime> MoviePlayTimes { get; set; }
    }
}
