// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.Models
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(RowType))]
    [KnownType(typeof(Chair))]
    [KnownType(typeof(CinemaHall))]
    public class Row
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public RowType RowType { get; set; }

        [DataMember]
        public List<Chair> Chairs { get; set; }

        [DataMember]
        public CinemaHall CinemaHall { get; set; }
    }
}
