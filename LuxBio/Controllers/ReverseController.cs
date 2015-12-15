// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuxBio.Library.Models;
using LuxBio.WindowsApp.LuxBioWCF;

namespace LuxBio.WindowsApp.Controllers
{
    public class ReverseController
    {
        private Service1Client client;

        public ReverseController()
        {
            client = new LuxBioWCF.Service1Client("TcpBinding_IService1");
        }
        
        public IEnumerable<LuxBio.Library.Models.ExtraPropperties.Chair> GetAllChairsState(MoviePlayTime moviePlayTime, CinemaHall hall)
        {
            if (moviePlayTime != null && hall != null)
            {
                return client.GetAllChairsState(moviePlayTime, hall);
            }

            return new List<LuxBio.Library.Models.ExtraPropperties.Chair>();
        }

        public void CreateReserve(MoviePlayTime moviePlayTime, List<Chair> chairs, Customer customer, DateTime dato)
        {
            client.CreateReserve(moviePlayTime, chairs.ToArray(), customer, dato);
        }

        public List<LuxBio.Library.Models.ExtraPropperties.Chair> FindBestChairs(MoviePlayTime moviePlayTime, int chairCounts)
        {
            return client.FindBestChairs(moviePlayTime, chairCounts).ToList();
        }

        public List<LockedChair> GetLockedChairs(MoviePlayTime movieplaytime, CinemaHall hall)
        {
            return client.GetLockedChairs(movieplaytime, hall).ToList();
        }

        public void ReleaseLocked(MoviePlayTime movieplaytime, List<Chair> chairs)
        {
            client.ReleaseLocked(movieplaytime, chairs.ToArray());
        }

        public void ReleaseLocked(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer)
        {
            client.ReleaseLockedWithCustomer(movieplaytime, chairs.ToArray(), customer);
        }

        public List<LockedChair> GetLockedChairsForUser(MoviePlayTime movieplaytime, Customer customer)
        {
            return client.GetLockedChairsForUser(movieplaytime, customer).ToList();
        }

        public void LockChairs(MoviePlayTime movieplaytime, IEnumerable<Chair> chairs, Customer customer)
        {
            client.LockChairs(movieplaytime, chairs.ToArray(), customer);
        }

        public LuxBio.Library.Models.ExtraPropperties.Chair[] FindChairsByFirstSelected(List<Library.Models.ExtraPropperties.Chair> statedChairsFromRow, Library.Models.ExtraPropperties.Chair selectedChair, MoviePlayTime moviePlayTime, int chairCount)
        {
            return client.FindChairsByFirstSelected(statedChairsFromRow.ToArray(), selectedChair, moviePlayTime, chairCount);
        }

        public void UpdateLockedChairs(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer)
        {
            client.UpdateLockedChairs(movieplaytime, chairs.ToArray(), customer);
        }
    }
}
