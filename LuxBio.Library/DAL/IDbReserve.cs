using LuxBio.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.DAL
{
    public interface IDbReserve
    {
        void CreateTempReserve(MoviePlayTime moviePlayTime, List<Chair> chairs, DateTime datetime, Customer user);

        void CreateReserve(MoviePlayTime moviePlayTime, List<Chair> chairs, Customer user, DateTime dateTime);

        void CreateReserve(MoviePlayTime moviePlayTime, List<Chair> chairs, Customer user, Sale sale, DateTime dateTime, bool isTemp);

        void LockChairs(List<Chair> chairs, MoviePlayTime moviePlayTime, Customer customer, DateTime dateTime);

        List<LockedChair> GetLockedChairs(MoviePlayTime moviePlayTime);

        List<LockedChair> GetLockedChairs(Customer customer, MoviePlayTime moviePlayTime);

        void UpdateLockedChairs(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer, DateTime datetime);

        void ReleaseLockedChairs(MoviePlayTime movieplaytime, List<Chair> chairs);

        void ReleaseLockedChairsByTime(MoviePlayTime movieplaytime, List<Chair> chairs);

        void ReleaseLockedChairs(MoviePlayTime movieplaytime, Customer customer);

        void ReleaseLockedChairs(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer, DateTime? timeToRemove);

        IEnumerable<Models.ExtraPropperties.Chair> GetAllChairsState(MoviePlayTime moviePlayTime, CinemaHall hall);

        bool IsAvalible(MoviePlayTime moviePlayTime, IEnumerable<Chair> chairs);


    }
}
