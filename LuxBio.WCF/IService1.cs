// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using LuxBio.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace LuxBio.WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(Namespace = "http://mycompany.com/LuxBio.WCF")]
    public interface IService1
    {
        //[OperationContract]
        //string GetData(int value);

        [OperationContract]
        IEnumerable<MovieInfo> GetAllMovies();

        [OperationContract]
        MovieInfo GetMovie(int id);

        [OperationContract]
        CinemaHall GetHall(int id);

        [OperationContract]
        MoviePlayTime GetMoviePlayTime(int id);

        [OperationContract]
        IEnumerable<MoviePlayTime> GetMoviePlayTimes();

        [OperationContract]
        List<LuxBio.Library.Models.ExtraPropperties.Chair> GetAllChairsState(MoviePlayTime moviePlayTime, CinemaHall hall);

        [OperationContract]
        void CreateReserve(MoviePlayTime moviePlayTime, List<Chair> chairs, Customer customer, DateTime dato);

        [OperationContract]
        Customer GetCustomer(int id);

        [OperationContract]
        List<LuxBio.Library.Models.ExtraPropperties.Chair> FindBestChairs(MoviePlayTime moviePlayTime, int chairCounts);

        [OperationContract]
        List<LockedChair> GetLockedChairs(MoviePlayTime movieplaytime, CinemaHall hall);

        [OperationContract]
        void LockChairs(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer);

        [OperationContract]
        List<LockedChair> GetLockedChairsForUser(MoviePlayTime movieplaytime, Customer customer);

        [OperationContract]
        void ReleaseLocked(MoviePlayTime movieplaytime, List<Chair> chairs);

        [OperationContract]
        void ReleaseLockedWithCustomer(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer);

        [OperationContract]
        List<LuxBio.Library.Models.ExtraPropperties.Chair> FindChairsByFirstSelected(List<LuxBio.Library.Models.ExtraPropperties.Chair> statedChairsFromRow, Library.Models.ExtraPropperties.Chair selectedChair, MoviePlayTime moviePlayTime, int chairCount);

        [OperationContract]
        void ReleaseLockedByTime(MoviePlayTime movieplaytime, List<Chair> chairs);

        [OperationContract]
        void UpdateLockedChairs(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer);
    }
}
