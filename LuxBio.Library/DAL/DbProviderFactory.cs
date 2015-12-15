// Author: Kim Dam Grønhøj, Nick S. Reese, Gitte Nielsen
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.DAL
{
    public class DbProviderFactory
    {
        protected static DbProviderFactory factory;

        protected DbProviderFactory()
        {
        }

        public static DbProviderFactory Get(DbProvider provider)
        {
            if (factory == null)
            {
                switch (provider)
                {
                    case DbProvider.MicrosoftSQL:
                        factory = new SqlClientFactory();
                        break;
                }
            }

            return factory;
        }

        public virtual DbCommand CreateCommand()
        {
            return factory.CreateCommand();
        }

        public virtual DbConnection CreateConnection()
        {
            return factory.CreateConnection();
        }

        public virtual DbDataAdapter CreateDataAdapter()
        {
            return factory.CreateDataAdapter();
        }

        public virtual IDbReserve GetDbReserve()
        {
            return factory.GetDbReserve();
        }
    }
}
