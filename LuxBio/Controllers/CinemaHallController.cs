// Author: Kim Dam Grønhøj, Nick S. Reese
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuxBio.WindowsApp.LuxBioWCF;
using LuxBio.Library.Models;

namespace LuxBio.WindowsApp.Controllers
{
    public class CinemaHallController
    {
        private Service1Client client;

        public CinemaHallController()
        {
            client = new LuxBioWCF.Service1Client("TcpBinding_IService1");
            
        }

        public CinemaHall GetCinemaHall(int id)
        {
            try
            {
                return client.GetHall(id);
            } catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }
    }
}
