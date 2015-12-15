// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using LuxBio.Library.Controllers;
using LuxBio.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Timers;

namespace LuxBio.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        private MovieInfoController movieInfoCtr;
        private CinemaHallController cinemaHallCtr;
        private ReserveController reserveCtr;
        private MoviePlayController moviePlayTimeCtr;
        private CustomerController customerCtr;

   
        public Service1()
        {
            movieInfoCtr = new MovieInfoController();
            cinemaHallCtr = new CinemaHallController();
            reserveCtr = new ReserveController();
            moviePlayTimeCtr = new MoviePlayController(cinemaHallCtr.GetHall(1));
            customerCtr = new CustomerController();
        }

        public IEnumerable<MovieInfo> GetAllMovies()
        {
            return movieInfoCtr.GetAllMovies();
        }

        public MovieInfo GetMovie(int id)
        {
            return movieInfoCtr.GetMovie(id);
        }

        public CinemaHall GetHall(int id)
        {
            return cinemaHallCtr.GetHall(id);
        }

        public MoviePlayTime GetMoviePlayTime(int id)
        {
            return moviePlayTimeCtr.GetMoviePlayTime(id);
        }

        public IEnumerable<MoviePlayTime> GetMoviePlayTimes()
        {
            return moviePlayTimeCtr.GetAllMovieTimes();
        }

        public List<LuxBio.Library.Models.ExtraPropperties.Chair> GetAllChairsState(MoviePlayTime moviePlayTime, CinemaHall hall)
        {
            return reserveCtr.GetAllChairsState(moviePlayTime, hall);
        }

        public void CreateReserve(MoviePlayTime moviePlayTime, List<Chair> chairs, Customer customer, DateTime dato)
        {
            reserveCtr.CreateReserve(moviePlayTime, chairs, customer, dato);
        }

        public Customer GetCustomer(int id)
        {
            return customerCtr.GetCustomer(1);
        }

        public List<LuxBio.Library.Models.ExtraPropperties.Chair> FindBestChairs(MoviePlayTime moviePlayTime, int chairCounts)
        {
            return reserveCtr.FindBestChairs(moviePlayTime, chairCounts);
        }

        public List<LockedChair> GetLockedChairs(MoviePlayTime movieplaytime, CinemaHall hall)
        {
            return reserveCtr.GetLockedChairs(movieplaytime, hall);
        }

        public void LockChairs(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer)
        {
            ServiceData myServiceData = new ServiceData();
            try {
                reserveCtr.LockChairs(movieplaytime, chairs, customer);
            } catch (Exception ex)
            {
                myServiceData.ErrorDetails = ex.StackTrace;
                myServiceData.ErrorMessage = ex.Message;
                myServiceData.Result = true;
                throw new FaultException<ServiceData>(myServiceData, ex.Message);
            }
        }

        public List<LockedChair> GetLockedChairsForUser(MoviePlayTime movieplaytime, Customer customer)
        {
            return reserveCtr.GetLockedChairsForUser(movieplaytime, customer);
        }

        public List<LuxBio.Library.Models.ExtraPropperties.Chair> FindChairsByFirstSelected(List<Library.Models.ExtraPropperties.Chair> statedChairsFromRow, Library.Models.ExtraPropperties.Chair selectedChair, MoviePlayTime moviePlayTime, int chairCount)
        {
            return reserveCtr.FindChairsByFirstSelected(statedChairsFromRow, selectedChair, moviePlayTime, chairCount);
        }

        public void ReleaseLocked(MoviePlayTime movieplaytime, List<Chair> chairs)
        {
            reserveCtr.ReleaseLocked(movieplaytime, chairs);
        }

        public void ReleaseLockedWithCustomer(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer)
        {
            reserveCtr.ReleaseLocked(movieplaytime, chairs, customer);
        }

        public void ReleaseLockedByTime(MoviePlayTime movieplaytime, List<Chair> chairs)
        {
            reserveCtr.ReleaseLockedChairsByTime(movieplaytime, chairs);
        }

        public void UpdateLockedChairs(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer)
        {
            reserveCtr.UpdateLockedChairs(movieplaytime, chairs, customer);
        }


    }

    // Use a data contract as illustrated in the sample
    // below to add composite types to service operations.
    [DataContract]
    public class ServiceData
    {
        [DataMember]
        public bool Result { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public string ErrorDetails { get; set; }
    }
}
