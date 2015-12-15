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
    [KnownType(typeof(CinemaHall))]
    [KnownType(typeof(MovieInfo))]
    public class MoviePlayTime
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public DateTime StartTime { get; set; }

        [DataMember]
        public int DelayTime { get; set; }

        [DataMember]
        public CinemaHall CinemaHall { get; set; }

        [DataMember]
        public MovieInfo MovieInfo { get; set; }
    }
}
