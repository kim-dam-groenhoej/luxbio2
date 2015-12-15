// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using LuxBio.Website.LuxBioWCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LuxBio.Website.Controllers
{
    public class HomeController : Controller
    {
        private Service1Client client;

        public HomeController()
        {
            client = new Service1Client("TcpBinding_IService1");
        }

        public ActionResult Index()
        {
            var hall = client.GetHall(1);
            var moviePlayTime = client.GetMoviePlayTime(1);
            var chairs = client.GetAllChairsState(moviePlayTime, hall);

            ViewBag.Chairs = chairs;

            return View();
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}