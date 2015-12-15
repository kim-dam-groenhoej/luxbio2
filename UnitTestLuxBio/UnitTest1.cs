// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;
using LuxBio.Library;
using LuxBio.Library.Controllers;
using LuxBio.Library.Models;
using System.Collections.Generic;

namespace UnitTestLuxBio
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //LuxBio.Website.WCF.Service1Client client = new LuxWCF.Service1Client("");
            //string dd = client.GetData(1);

            //Trace.WriteLine(dd);
        }

        [TestMethod]
        public void GetMovies()
        {
            var moviesCrl = new MovieInfoController();

            var movies = moviesCrl.GetAllMovies();

            Assert.IsTrue(movies.Count() > 0);
        }

        [TestMethod]
        public void GetMovieNegative()
        {
            var moviesCrl = new MovieInfoController();

            var movie = moviesCrl.GetMovie(3);

            Assert.IsNull(movie);
        }

        [TestMethod]
        public void GetMoviePositive()
        {
            var moviesCrl = new MovieInfoController();

            var movie = moviesCrl.GetMovie(1);

            Assert.IsNotNull(movie);
            Assert.IsNotNull(movie.Director);
        }

        [TestMethod]
        public void CreateReserve()
        {
            var hallCtr = new CinemaHallController();
            var reserveCtr = new ReserveController();
            var customerCtr = new CustomerController();

            var customer = customerCtr.GetCustomer(1);

            var hall = hallCtr.GetHall(1);

            var moviePlayTimeCtr = new MoviePlayController(hall);
            var moviePlayTime = moviePlayTimeCtr.GetMoviePlayTime(1);

            var chairs = hall.Rows.FirstOrDefault().Chairs.Where(c => c.Number == 2 || c.Number == 4);

            reserveCtr.CreateReserve(moviePlayTime, chairs.ToList(), customer, DateTime.Now.AddMinutes(60 * 5));

            var chairs1 = new List<Chair>();
            chairs1.AddRange(chairs);
            Assert.IsTrue(!reserveCtr.IsAvalible(moviePlayTime, chairs1));
        }

        [TestMethod]
        public void GetChairs()
        {
            var hallCtr = new CinemaHallController();
            var reserveCtr = new ReserveController();
            
            var hall = hallCtr.GetHall(1);

            var moviePlayTimeCtr = new MoviePlayController(hall);
            var moviePlayTime = moviePlayTimeCtr.GetMoviePlayTime(1);

            var reserve = reserveCtr.GetAllChairsState(moviePlayTime, hall);

            Assert.IsTrue(reserve.Where(c => c.Available == LuxBio.Library.Models.ExtraPropperties.ChairAvailableType.Available).Count() > 0);
            Assert.IsTrue(reserve.Where(c => c.Available == LuxBio.Library.Models.ExtraPropperties.ChairAvailableType.Busy).Count() > 0);
        }

        [TestMethod]
        public void IsCharsAvailable()
        {
            var isValid = false;
            var reserveCtr = new ReserveController();
            var hallCtr = new CinemaHallController();

            var hall = hallCtr.GetHall(1);

            var moviePlayTimeCtr = new MoviePlayController(hall);
            var moviePlayTime = moviePlayTimeCtr.GetMoviePlayTime(1);

            var chairsWithState = reserveCtr.GetAllChairsState(moviePlayTime, hall);

            var chairs = chairsWithState.Where(c => c.Number == 2 || c.Number == 4 || c.Number == 6);

            isValid = reserveCtr.IsAvalible(moviePlayTime, chairs);

            foreach (var item in chairs)
            {
                isValid = item.Row.Number == 1;
            }

            Assert.IsTrue(isValid);

            //foreach (var item in chairs)
            //{
            //    Trace.WriteLine(item.Row.Number);
            //}

        }
        [TestMethod]
        public void GetMovieTime()
        {
            var hallCtr = new CinemaHallController();
            var moviePlayTimeCtr = new MoviePlayController(hallCtr.GetHall(1));

            var movieTime = moviePlayTimeCtr.GetMoviePlayTime(1);

            Assert.IsNotNull(movieTime);
        }

        [TestMethod]
        public void ReleaseChairsReserve()
        {
            var clearLockedChairs = new List<Chair>();
            var hallCtr = new CinemaHallController();
            var reserveCtr = new ReserveController();

            var customerCtr = new CustomerController();

            var customer = customerCtr.GetCustomer(1);
            var hall = hallCtr.GetHall(1);

            var moviePlayTimeCtr = new MoviePlayController(hall);
            var moviePlayTime = moviePlayTimeCtr.GetMoviePlayTime(1);


            // First inserting locked chairs
            var chairs = hall.Rows.FirstOrDefault().Chairs.Where(c => c.Number == 2 || c.Number == 4).ToList();
            reserveCtr.LockChairs(moviePlayTime, chairs, customer);

            // Checking the locked chairs is in memory
            var lockedChairs = reserveCtr.GetLockedChairs(moviePlayTime, hall);
            foreach (var lockedChair in lockedChairs)
            {
                if (lockedChair.Char.Number == 2 || lockedChair.Char.Number == 4)
                {
                    // do nothing
                }
                else
                {
                    Assert.IsTrue(true);
                    clearLockedChairs.Add(lockedChair.Char);
                }
            }

            // clear locked chairs in memory
            reserveCtr.ReleaseLocked(moviePlayTime, clearLockedChairs);
        }

        [TestMethod]
        public void ReleaseUserChairsReserve()
        {
            var clearLockedChairs = new List<Chair>();
            var hallCtr = new CinemaHallController();
            var reserveCtr = new ReserveController();

            var customerCtr = new CustomerController();

            var hall = hallCtr.GetHall(1);

            var customer = customerCtr.GetCustomer(1);
            var moviePlayTimeCtr = new MoviePlayController(hall);
            var moviePlayTime = moviePlayTimeCtr.GetMoviePlayTime(1);


            // First inserting locked chairs
            var chairs = hall.Rows.FirstOrDefault().Chairs.Where(c => c.Number == 2 || c.Number == 4).ToList();
            reserveCtr.LockChairs(moviePlayTime, chairs, customer);

            // Checking the locked chairs for a specific user is in memory
            var lockedChairs = reserveCtr.GetLockedChairsForUser(moviePlayTime, customer);
            foreach (var lockedChair in lockedChairs)
            {
                if (lockedChair.Char.Number == 2 || lockedChair.Char.Number == 4)
                {
                    // do nothing
                }
                else
                {
                    Assert.IsTrue(true);
                    clearLockedChairs.Add(lockedChair.Char);
                }
            }

            // clear locked chairs in memory
            reserveCtr.ReleaseLocked(moviePlayTime, clearLockedChairs);
        }

        [TestMethod]
        public void FindBestChairs()
        {
            var hallCtr = new CinemaHallController();
            var reserveCtr = new ReserveController();
            var moviePlayTimeCtr = new MoviePlayController(hallCtr.GetHall(1));

            var bestChairs = reserveCtr.FindBestChairs(moviePlayTimeCtr.GetMoviePlayTime(1), 3);

            var firstChair = bestChairs[0];
            var nextChair = bestChairs[1];
            var thridChair = bestChairs[2];

            var firstOrderNumber = firstChair.OrderNumber;

            Assert.IsTrue(firstOrderNumber + 1 == nextChair.OrderNumber && firstOrderNumber + 2 == thridChair.OrderNumber);
        }

        public void GetChairsBySelectingOne()
        {

        }
    }
}