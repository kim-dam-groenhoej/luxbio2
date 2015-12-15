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
    public class MovieInfoController
    {
        private Service1Client client;

        public MovieInfoController()
        {
            client = new LuxBioWCF.Service1Client("TcpBinding_IService1");
        }

        public IEnumerable<MovieInfo> GetAllMovies()
        {
            return client.GetAllMovies();
        }

        public MovieInfo GetMovie(int id)
        {
            return client.GetMovie(id);
        }
    }
}
