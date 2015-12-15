// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuxBio.Library.Models;
using LuxBio.Library.DAL;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;

namespace LuxBio.Library.Controllers
{
    public class ReserveController
    {
        public static int expireMinutesForLock = 1;
        public static int expireMinutesCustomerReserve = 30;

        private IDbReserve reserveDb;

        public ReserveController()
        {
            reserveDb = DbProviderFactory.Get(DbProvider.MicrosoftSQL).GetDbReserve();
        }

        public void UpdateLockedChairs(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer)
        {
            if (chairs.Count() > 0)
            {
                reserveDb.UpdateLockedChairs(movieplaytime, chairs, customer, DateTime.Now.AddMinutes(expireMinutesForLock));
            }
        }

        public List<Models.ExtraPropperties.Chair> FindBestChairs(MoviePlayTime moviePlayTime, int chairCounts)
        {
            var bestChairs = new List<Models.ExtraPropperties.Chair>();
            var statedChairs = GetAllChairsState(moviePlayTime, moviePlayTime.CinemaHall);

            // Finding rows in chairs and counting available chairs
            var rows = new List<Row>();
            statedChairs.ForEach(chair =>
            {
                var row = rows.FirstOrDefault(r => r.ID == chair.Row.ID);

                if (row == null)
                {
                    rows.Add(chair.Row);
                }
            });

            // Order by: "the middle row first"
            rows = rows.OrderByMiddle();

            // searching rows for number of chairs user want
            int i = 0;
            var foundRow = false;
            while (!foundRow && i <= (rows.Count() - 1))
            {
                var currentRow = rows[i];
                bestChairs = statedChairs.Where(c => c.Row.ID == currentRow.ID).ToList().FindCombinedObjects(chairCounts);

                if (bestChairs.Count() == chairCounts)
                {
                    foundRow = true;
                }
                i++;
            }

            return bestChairs;
        }

        public List<Models.ExtraPropperties.Chair> FindChairsByFirstSelected(List<Models.ExtraPropperties.Chair> statedChairsFromRow, Models.ExtraPropperties.Chair selectedChair, MoviePlayTime moviePlayTime, int chairCount)
        {
            var chairs = statedChairsFromRow.FindCombinedObjectsByFirstChoice(selectedChair, chairCount);

            // Nothing found, find best choices
            if (chairs.Count() == 0)
            {
                chairs = FindBestChairs(moviePlayTime, chairCount);
            }

            return chairs;
        }

        public List<Models.ExtraPropperties.Chair> GetAllChairsState(MoviePlayTime moviePlayTime, CinemaHall hall)
        {
            var statedChairs = reserveDb.GetAllChairsState(moviePlayTime, hall).OrderByDescending(c => c.Row.ID);

            // Adding order number for chairs for every row. Resetting order number after every row
            int generateOrderNumber = 1;
            Row rowBefore = statedChairs.FirstOrDefault().Row;

            foreach (var item in statedChairs)
            {
                if (item.Row.ID != rowBefore.ID)
                {
                    generateOrderNumber = 1;
                    rowBefore = item.Row;
                }

                item.OrderNumber = generateOrderNumber++;

                // TODO refactor this - this get a collection of all chairs and check only one every time. SUCKS!
                var alreadyLockedChair = GetLockedChairs(moviePlayTime, hall).FirstOrDefault(lc => lc.Char.ID == item.ID && lc.Char.Row.ID == item.Row.ID);
                if (alreadyLockedChair != null)
                {
                    item.Available = Models.ExtraPropperties.ChairAvailableType.Locked;
                }
            }

            return statedChairs.ToList();
        }

        public bool IsAvalible(MoviePlayTime moviePlayTime, IEnumerable<Chair> chairs)
        {
            return reserveDb.IsAvalible(moviePlayTime, chairs);
        }

        public void CreateReserve(MoviePlayTime moviePlayTime, List<Chair> chairs,  Customer customer, DateTime dato)
        {
            reserveDb.CreateReserve(moviePlayTime, chairs, customer, dato);
        }

        // https://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.methodimploptions(v=vs.110).aspx
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LockChairs(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer)
        {
            if (chairs.Count() > 0)
            {
                // Unlock last selected chairs
                reserveDb.ReleaseLockedChairs(movieplaytime, customer);

                var alreadyLockedChairs = GetLockedChairs(movieplaytime, movieplaytime.CinemaHall).Where(c => chairs.FirstOrDefault(lc => lc.ID == c.Char.ID && lc.Row.ID == c.Char.Row.ID) != null);
                // Check status for chairs again
                var statedChairs = GetAllChairsState(movieplaytime, movieplaytime.CinemaHall).Where(c => chairs.FirstOrDefault(lc => lc.ID == c.ID && lc.Row.ID == c.Row.ID) != null && c.Available != Models.ExtraPropperties.ChairAvailableType.Available);

                if (alreadyLockedChairs.Count() == 0 && statedChairs.Count() == 0)
                {
                    reserveDb.LockChairs(chairs, movieplaytime, customer, DateTime.Now.AddMinutes(expireMinutesForLock));
                }
                else
                {
                    throw new Exception("Disse sæder er allerede optaget.");
                }
            }
        }

        public List<LockedChair> GetLockedChairs(MoviePlayTime movieplaytime, CinemaHall hall)
        {
            return reserveDb.GetLockedChairs(movieplaytime);
        }

        public List<LockedChair> GetLockedChairsForUser(MoviePlayTime movieplaytime, Customer customer)
        {
            return reserveDb.GetLockedChairs(customer, movieplaytime);
        }

        public void ReleaseLocked(MoviePlayTime movieplaytime, List<Chair> chairs)
        {
            if (chairs.Count() > 0)
            {
                reserveDb.ReleaseLockedChairs(movieplaytime, chairs);
            }
        }

        public void ReleaseLocked(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer)
        {
            if (chairs.Count() > 0)
            {
                reserveDb.ReleaseLockedChairs(movieplaytime, chairs, customer, null);
            }
        }

        public void ReleaseLockedChairsByTime(MoviePlayTime movieplaytime, List<Chair> chairs)
        {
            if (chairs.Count() > 0)
            {
                reserveDb.ReleaseLockedChairsByTime(movieplaytime, chairs);
            }
        }

        public void ReleaseRowReserve(MoviePlayTime movieplaytime, Row row)
        {
            ReleaseLocked(movieplaytime, row.Chairs);
        }
    }
}
