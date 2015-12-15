// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using LuxBio.Library.Models;
using LuxBio.WindowsApp.LuxBioWCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.WindowsApp.Controllers
{
    public class MoviePlayTimeController
    {
        private Service1Client client;

        public MoviePlayTimeController()
        {
            client = new LuxBioWCF.Service1Client("TcpBinding_IService1");
        }

        public IEnumerable<MoviePlayTime> GetAllyMoviePlaTimes()
        {
            return client.GetMoviePlayTimes();
        }

        public MoviePlayTime GetMoviePlayTime(int id)
        {
            return client.GetMoviePlayTime(id);
        }
    }
}
