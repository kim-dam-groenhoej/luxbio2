// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuxBio.WindowsApp.LuxBioWCF;
using LuxBio.Library.Models;

namespace LuxBio.WindowsApp.ViewModel
{
    public class ChairViewModel : LuxBio.Library.Models.ExtraPropperties.Chair
    {
        public ChairViewModel()
        {

        }
        public ChairViewModel(LuxBio.Library.Models.ExtraPropperties.Chair chair)
        {
            this.ID = chair.ID;
            this.Available = chair.Available;
            this.Number = chair.Number;
            this.Row = chair.Row;
            this.CheckBoxEnabled = true;
        }

        public bool CheckBoxEnabled { get; set; }

        public String Color { get; set; }

        public decimal MarginLeft { get; set; }

        public decimal MarginTop { get; set; }
    }
}
