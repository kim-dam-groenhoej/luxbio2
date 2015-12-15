// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using LuxBio.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.Controllers
{
    public class MovieInfoController
    {
        private List<MovieInfo> movies;

        // TODO Refactor this
        public MovieInfoController()
        {
            movies = new List<MovieInfo>();

            ((List<MovieInfo>)movies).Add(new MovieInfo()
            {
                Title = "Avatar",
                Description = "Total fed film",
                ID = 1,
                Director = new Director()
                {
                    ID = 1,
                    Name = "Søren Jensen"
                },
                Image = "C:/Images/avatar.jpg",
                Length = 120,
                License = new License()
                {
                    ID = 1,
                    StartDate = DateTime.Now,
                    ExpiredDate = DateTime.Now.AddDays(10)
                },
                Price = 100,
                ProductionDate = DateTime.Now.AddDays(-10)
            });

            ((List<MovieInfo>)movies).Add(new MovieInfo()
            {
                Title = "Predator - Arnold",
                Description = "Total fed film",
                ID = 2,
                Director = new Director()
                {
                    ID = 1,
                    Name = "Søren Jensen"
                },
                Image = "C:/Images/Predator.jpg",
                Length = 120,
                License = new License()
                {
                    ID = 2,
                    StartDate = DateTime.Now,
                    ExpiredDate = DateTime.Now.AddDays(10)
                },
                Price = 120,
                ProductionDate = DateTime.Now.AddDays(-10)
            });

        }

        public IEnumerable<MovieInfo> GetAllMovies()
        {
            return movies;
        }
        public MovieInfo GetMovie(int id)
        {
            return movies.FirstOrDefault(m => m.ID == id);
        }
    }
}
