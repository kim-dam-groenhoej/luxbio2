// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using LuxBio.Library.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.DAL
{
    public class MSSQLReserve : IDbReserve
    {
        private SqlClientFactory factory {
            get;
            set;
        }

        public MSSQLReserve(SqlClientFactory factory)
        {
            this.factory = factory;
        }

        public void CreateTempReserve(MoviePlayTime moviePlayTime, List<Chair> chairs, DateTime datetime, Customer user)
        {
            CreateReserve(moviePlayTime, chairs, user, null, datetime, true);
        }

        public void CreateReserve(MoviePlayTime moviePlayTime, List<Chair> chairs, Customer user, DateTime dateTime)
        {
            CreateReserve(moviePlayTime, chairs, user, null, dateTime, false);
        }

        public void CreateReserve(MoviePlayTime moviePlayTime, List<Chair> chairs, Customer user, Sale sale, DateTime dateTime, bool isTemp)
        {
            using (var con = factory.CreateConnection())
            {
                con.Open();

                // https://msdn.microsoft.com/en-us/library/ms177523.aspx
                var cmdRead = con.CreateCommand();
                var sqlRead = "UPDATE r " +
                    "SET r.Active = 'false' " +
                    "FROM [Reserve] r " +
                    "INNER JOIN [ReserveChairRelations] rcr ON r.ID = rcr.ReserveID " +
                    "INNER JOIN [Chairs] c ON rcr.ChairID = c.ID WHERE ";

                var c = 0;
                foreach (var chair in chairs)
                {
                    cmdRead.AddParameter("ChairID" + c, chair.ID);
                    sqlRead += " c.ID = @ChairID" + c + " OR ";
                    c++;
                }
                sqlRead = sqlRead.Substring(0, sqlRead.Length - 3);

                cmdRead.CommandText = sqlRead;

                using (DbDataReader dr = cmdRead.ExecuteReader(CommandBehavior.Default))
                {
                    if (dr.Read())
                    {
                        throw new Exception("duplicate chairs IDs for movieplaytime, hall, reservation.");
                    }
                }

                var cmd = con.CreateCommand();

                var sql = " DECLARE @ReserveID INT = 0; INSERT INTO [Reserve] (LatestPickupDate, UserID, " +
                  (sale != null ? "SaleID," : string.Empty) +
                    "MoviePlayTimeID, Active) VALUES(@LatestPickupDate, @UserID, " +
                    (sale != null ? "@SaleID," : string.Empty) +
                   "@MoviePlayTimeID, 'true'); SET @ReserveID = SCOPE_IDENTITY();";
                var i = 0;
                foreach (var chair in chairs)
                {
                    cmd.AddParameter("ChairID" + i, chair.ID);
                    sql += " INSERT INTO [ReserveChairRelations] (ReserveID, ChairID) VALUES(@ReserveID, @ChairID" + i + ")";
                    i++;
                }

                cmd.AddParameter("LatestPickupDate", dateTime);
                cmd.AddParameter("UserID", user.ID);
                if (sale != null)
                {
                    cmd.AddParameter("SaleID", sale.ID);
                }
                cmd.AddParameter("MoviePlayTimeID", moviePlayTime.ID);

                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();
            }
        }

        public void LockChairs(List<Chair> chairs, MoviePlayTime moviePlayTime, Customer customer, DateTime dateTime)
        {
            var lockedChairs = new List<LockedChair>();

            using (var con = factory.CreateConnection())
            {
                con.Open();
                var cmd = factory.CreateCommand();

                var sql = string.Empty;

                int i = 0;
                foreach (var c in chairs)
                {
                    var parameterChairName = string.Format("chair{0}", i);
                    var parameterDateName = string.Format("date{0}", i);
                    var parameterUserID = string.Format("UserID{0}", i);
                    var parameterMoviePlayTimeID = string.Format("MoviePlayTimeID{0}", i);

                    sql += string.Format("INSERT INTO [LockedChairs] (ChairID, LockedDateTime, UserID, MoviePlayTimeID) VALUES(@{0}, @{1}, @{2}, @{3});", 
                        parameterChairName, parameterDateName, parameterUserID, parameterMoviePlayTimeID);

                    cmd.AddParameter(parameterChairName, c.ID);
                    cmd.AddParameter(parameterDateName, dateTime);
                    cmd.AddParameter(parameterUserID, customer.ID);
                    cmd.AddParameter(parameterMoviePlayTimeID, moviePlayTime.ID);

                    i++;
                }

                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        public List<LockedChair> GetLockedChairs(MoviePlayTime moviePlayTime)
        {
            return GetLockedChairs(null, moviePlayTime);
        }

        public List<LockedChair> GetLockedChairs(Customer customer, MoviePlayTime moviePlayTime)
        {
            var lockedChairs = new List<LockedChair>();

            using (var con = factory.CreateConnection())
            {
                con.Open();

                var cmd = factory.CreateCommand();
                cmd.Connection = con;

                var sql = string.Format("SELECT * FROM [LockedChairs] WHERE MoviePlayTimeID=@MoviePlayTimeID AND {0}", customer != null ? "UserID=@customerID AND " : string.Empty);

                cmd.AddParameter("MoviePlayTimeID", moviePlayTime.ID);

                if (customer != null)
                {
                    cmd.AddParameter("customerID", customer.ID);
                }

                // SQL injections prevented by using SQL Parameters in driver layer
                var chairsSQL = "ChairID IN ("; // Help to IN keyword: https://msdn.microsoft.com/en-us/library/ms177682.aspx
                int i = 0;
                foreach (var row in moviePlayTime.CinemaHall.Rows)
                {
                    foreach (var c in row.Chairs)
                    {
                        var parameterChairName = string.Format("chair{0}", i);

                        // add sqlparameter name into SQL strings and combine 
                        chairsSQL += string.Format("@{0},", parameterChairName);

                        // add parameter values to SQLCommand
                        cmd.AddParameter(parameterChairName, c.ID);
                        i++;
                    }
                }

                // Remove last "," in the list of IDs and closing the list of IDs with ")"
                chairsSQL = chairsSQL.Substring(0, chairsSQL.Length - 1) + ")";

                // Combine chairsSQL to the SQL query
                sql += chairsSQL;

                // Adding the SQL string to SQLCommand
                cmd.CommandText = sql;

                // Sending SQL to SQL Server and getting data
                using (DbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        var lockedChair = new LockedChair();

                        // TODO refactor this
                        var chairID = dr.GetInt32(dr.GetOrdinal("ChairID"));
                        lockedChair.Char = moviePlayTime.CinemaHall.Rows.FirstOrDefault(r => r.Chairs.FirstOrDefault(c => c.ID == chairID) != null).Chairs.FirstOrDefault(c => c.ID == chairID);

                        lockedChair.Customer = customer;
                        lockedChair.MoviePlayTime = moviePlayTime;

                        lockedChairs.Add(lockedChair);
                    }
                }
            }

            return lockedChairs;
        }

        public void UpdateLockedChairs(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer, DateTime datetime)
        {
            using (var con = factory.CreateConnection())
            {
                con.Open();

                var cmd = con.CreateCommand();
                var sql = "UPDATE [LockedChairs] SET LockedDatetime=@LockedDatetime WHERE MoviePlayTimeID=@movieplaytime AND UserID=@UserID AND ";

                cmd.AddParameter("LockedDatetime", datetime);
                cmd.AddParameter("UserID", customer.ID);
                cmd.AddParameter("MoviePlayTime", movieplaytime.ID);

                // SQL injections prevented by using SQL Parameters in driver layer
                var chairsSQL = "ChairID IN ("; // Help to IN keyword: https://msdn.microsoft.com/en-us/library/ms177682.aspx
                var i = 0;
                foreach (var c in chairs)
                {
                    var parameterChairName = string.Format("chair{0}", i);

                    // add sqlparameter name into SQL strings and combine 
                    chairsSQL += string.Format("@{0},", parameterChairName);

                    // add parameter values to SQLCommand
                    cmd.AddParameter(parameterChairName, c.ID);
                    i++;
                }

                // Remove last "," in the list of IDs and closing the list of IDs with ")"
                chairsSQL = chairsSQL.Substring(0, chairsSQL.Length - 1) + ")";

                cmd.CommandText = string.Format("{0}{1}", sql, chairsSQL);

                cmd.ExecuteNonQuery();
            }
        }

        public void ReleaseLockedChairs(MoviePlayTime movieplaytime, List<Chair> chairs)
        {
            ReleaseLockedChairs(movieplaytime, chairs, null, null);
        }

        public void ReleaseLockedChairsByTime(MoviePlayTime movieplaytime, List<Chair> chairs)
        {
            ReleaseLockedChairs(movieplaytime, chairs, null, DateTime.Now);
        }

        public void ReleaseLockedChairs(MoviePlayTime movieplaytime, Customer customer)
        {
            ReleaseLockedChairs(movieplaytime, null, customer, null);
        }

        public void ReleaseLockedChairs(MoviePlayTime movieplaytime, List<Chair> chairs, Customer customer, DateTime? timeToRemove)
        {
            using (var con = factory.CreateConnection())
            {
                con.Open();

                var cmd = con.CreateCommand();
                var sql = "DELETE FROM [LockedChairs] " +
                    string.Format("WHERE MovieplaytimeID=@movieplaytimeID {0} {1}", customer != null ? "AND UserID=@userID" : string.Empty, timeToRemove.HasValue ? "AND LockedDateTime<@LockedDateTime" : string.Empty);

                cmd.AddParameter("movieplaytimeID", movieplaytime.ID);
                if (customer != null)
                {
                    cmd.AddParameter("userID", customer.ID);
                }

                if (timeToRemove.HasValue)
                {
                    cmd.AddParameter("LockedDateTime", timeToRemove.Value);
                }

                var chairsSQL = string.Empty;
                if (chairs != null)
                {
                    // SQL injections prevented by using SQL Parameters in driver layer
                    chairsSQL = " AND ChairID IN ("; // Help to IN keyword: https://msdn.microsoft.com/en-us/library/ms177682.aspx
                    var i = 0;
                    foreach (var c in chairs)
                    {
                        var parameterChairName = string.Format("chair{0}", i);

                        // add sqlparameter name into SQL strings and combine 
                        chairsSQL += string.Format("@{0},", parameterChairName);

                        // add parameter values to SQLCommand
                        cmd.AddParameter(parameterChairName, c.ID);
                        i++;
                    }

                    // Remove last "," in the list of IDs and closing the list of IDs with ")"
                    chairsSQL = chairsSQL.Substring(0, chairsSQL.Length - 1) + ")";
                }

                cmd.CommandText = string.Format("{0}{1}", sql, chairsSQL);

                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<Models.ExtraPropperties.Chair> GetAllChairsState(MoviePlayTime moviePlayTime, CinemaHall hall)
        {
            var chairs = new List<Models.ExtraPropperties.Chair>();

            using (var con = factory.CreateConnection())
            {
                con.Open();

                var cmd = factory.CreateCommand();
                cmd.Connection = con;

                var sql = "SELECT c.Number, rcr.ReserveID, c.ID, c.RowID " +
                            "FROM Chairs c " +
                            "LEFT JOIN ReserveChairRelations rcr " +
                            "ON rcr.ChairID = c.ID " +
                            "FULL OUTER JOIN Reserve r " +
                            "ON rcr.ReserveID = r.ID " +
                            "WHERE (r.Active = 'true' OR r.Active IS NULL) AND ";

                // SQL injections prevented by using SQL Parameters in driver layer
                var chairsSQL = "c.ID IN ("; // Help to IN keyword: https://msdn.microsoft.com/en-us/library/ms177682.aspx
                int i = 0;
                foreach (var row in hall.Rows)
                {
                    foreach (var c in row.Chairs)
                    {
                        var parameterChairName = string.Format("chair{0}", i);

                        // add sqlparameter name into SQL strings and combine 
                        chairsSQL += string.Format("@{0},", parameterChairName);

                        // add parameter values to SQLCommand
                        cmd.AddParameter(parameterChairName, c.ID);
                        i++;
                    }
                }

                // Remove last "," in the list of IDs and closing the list of IDs with ")"
                chairsSQL = chairsSQL.Substring(0, chairsSQL.Length - 1) + ")";

                // Combine chairsSQL to the SQL query
                sql += string.Format("({0}) AND (r.MoviePlayTimeID = @mpt0 OR r.MoviePlayTimeID IS NULL)", chairsSQL);

                cmd.AddParameter("mpt0", moviePlayTime.ID);

                // Adding the SQL string to SQLCommand
                cmd.CommandText = sql;

                // Sending SQL to SQL Server and getting data
                using (DbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        var chair = new Models.ExtraPropperties.Chair();
                        chair.Available = dr.IsDBNull(dr.GetOrdinal("ReserveID")) ? LuxBio.Library.Models.ExtraPropperties.ChairAvailableType.Available : LuxBio.Library.Models.ExtraPropperties.ChairAvailableType.Busy; // ReserveID, if NULL is availble!
                        chair.Number = dr.GetInt32(dr.GetOrdinal("Number"));
                        chair.ID = dr.GetInt32(dr.GetOrdinal("ID"));

                        // TODO Hardcoded rows in dbreserve!
                        chair.Row = hall.Rows.FirstOrDefault(r => r.Chairs.FirstOrDefault(c => c.ID == chair.ID) != null);
                        chair.Row.CinemaHall = hall;

                        chairs.Add(chair);
                    }
                }
            }

            return chairs;
        }

        public bool IsAvalible(MoviePlayTime moviePlayTime, IEnumerable<Chair> chairs)
        {
            var hall = chairs.FirstOrDefault().Row.CinemaHall;
            var isAvalible = false;
            using (var con = factory.CreateConnection())
            {
                con.Open();

                var cmd = factory.CreateCommand();
                cmd.Connection = con;
                var sql = "SELECT r.ID AS rID, r.LatestPickupDate, r.SaleID, r.UserID " +
                            "FROM Reserve r " +
                            "INNER JOIN ReserveChairRelations rcr " +
                            "ON rcr.ReserveID = r.ID " +
                            "WHERE (r.Active = 'true' OR r.Active IS NULL) AND ";


                // Add hall sql parameter number
                cmd.AddParameter("hall", hall.Number);

                // SQL injections prevented by using SQL Parameters in driver layer
                var chairsSQL = string.Empty;
                int i = 0;
                foreach (var row in hall.Rows)
                {
                    foreach (var c in row.Chairs)
                    {
                        var parameterChairName = string.Format("chair{0}", i);

                        // add chairs SQL strings and ccombine
                        chairsSQL += string.Format("(rcr.ChairID = @{0} ) OR ", parameterChairName);

                        // add parameters to SQLCommand
                        cmd.AddParameter(parameterChairName, c.ID);
                        i++;
                    }
                }

                if (chairsSQL.Length > 0)
                {
                    chairsSQL = chairsSQL.Substring(0, chairsSQL.Length - 3);
                    sql += string.Format("({0}) AND (r.MoviePlayTimeID = @mpt0 OR r.MoviePlayTimeID IS NULL)", chairsSQL);
                }
                else
                {
                    sql += "t.MoviePlayTimeID = @mpt0";
                }

                cmd.AddParameter("mpt0", moviePlayTime.ID);

                cmd.CommandText = sql;

                using (DbDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    isAvalible = !dr.Read();
                }
            }

            return isAvalible;
        }
    }
}
