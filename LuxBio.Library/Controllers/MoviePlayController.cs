// Author: Nick S. Reese
using LuxBio.Library.Models;
using LuxBio.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.Controllers
{
    public class MoviePlayController
    {
        private List<MoviePlayTime> movieTimes;

        // TODO Refactor this
        public MoviePlayController(CinemaHall hall)
        {
            movieTimes = new List<MoviePlayTime>();
            var movieInfoCtr = new MovieInfoController();

            var movieInfo = movieInfoCtr.GetMovie(1);
            for (int i = 1; i < 6; i++)
			{
			  movieTimes.Add(new MoviePlayTime() { ID = i, StartTime = DateTime.Now, DelayTime = 30, MovieInfo = movieInfo, CinemaHall = hall });
			}
            
        }

        public MoviePlayTime GetMoviePlayTime(int id)
        {
            return movieTimes.FirstOrDefault(mp => mp.ID == id);
            //return new MoviePlayTime() { ID = 1 };
        }

        public IEnumerable<MoviePlayTime> GetAllMovieTimes()
        {
            return movieTimes;
        }
    }
}
