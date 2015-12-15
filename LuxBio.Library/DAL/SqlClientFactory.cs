// Author: Kim Dam Grønhøj
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.DAL
{
    public class SqlClientFactory : DbProviderFactory
    {
        private SqlConnection connection;

        public override DbConnection CreateConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["LuxBio.Library.Properties.Settings.dmaa0914_3Sem_3ConnectionString"].ConnectionString;
            connection = new SqlConnection(connectionString);
            return connection;
        }

        public override DbCommand CreateCommand()
        {
            var sqlC = new SqlCommand();
            sqlC.Connection = connection;
            return sqlC;
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }

        public override IDbReserve GetDbReserve()
        {
            return new MSSQLReserve(this);
        }
    }
}
