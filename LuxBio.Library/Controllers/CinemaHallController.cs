// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using LuxBio.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.Controllers
{
    public class CinemaHallController
    {
        private List<CinemaHall> halls;

        // TODO Refactor this
        public CinemaHallController()
        {
            halls = new List<CinemaHall>();

            var rows = new List<Row>();
            var rowType = new RowType() { ID = 1, Name = "Sofa", Price = 100 };
            var hall = new CinemaHall() { Number = 1, Title = "Cinema1", Rows = rows };

            var moviePlayTimeController = new MoviePlayController(hall);
            hall.MoviePlayTimes = moviePlayTimeController.GetAllMovieTimes().ToList();

            halls.Add(hall);
            int chairId = 1;
            int rowID = 1;
            for (int i = 1; i < 6; i++)
			{
                var row = new Row() { ID = rowID, Number = i, RowType = rowType, CinemaHall = hall};
                rows.Add(row);
                var Chairs = new List<Chair>();
                int a = 2;
                int b = 1;

                for (int c = 1; c < 11; c++)
                {
                    Chairs.Add(new Chair() { ID = chairId, Number = c > 4 ? b : a, Row = row });
                    if (c > 4)
                    {
                        b = b + 2;
                    }
                    else
                    {
                        a = a + 2;
                    }

                    chairId++;
                }

                rowID++;
                row.Chairs = Chairs;
			}
               
        }

        public CinemaHall GetHall(int id)
        {
            return halls.FirstOrDefault(h => h.Number == id);
        }
    }
}
