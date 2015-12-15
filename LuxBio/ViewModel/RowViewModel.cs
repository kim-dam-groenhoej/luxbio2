// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using LuxBio.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.WindowsApp.ViewModel
{
    public class RowViewModel : Row
    {
        public RowViewModel(Row row, int countedAvailableChairs)
        {
            this.ID = row.ID;
            this.Chairs = row.Chairs;
            this.CinemaHall = row.CinemaHall;
            this.RowType = row.RowType;
            this.Number = row.Number;
            this.CountedAvailableChairs = countedAvailableChairs;
            this.IsCombinedChairsAvailable = false;
        }

        public int CountedAvailableChairs { get; set; }

        public bool IsCombinedChairsAvailable { get; set; }
    }
}
