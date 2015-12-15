// Author: Kim Dam Grønhøj
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library.DAL
{
    public static class DbHelpers
    {
        /// <summary>
        /// Shorter method to add dbparameters
        /// </summary>
        /// <param name="cmd">Current DbCommand</param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void AddParameter(this DbCommand cmd, string name, object value)
        {
            DbType dbType = DbType.Object;

            var @switch = new Dictionary<Type, Action> {
                { typeof(int), () => {
                    dbType = DbType.Int32;
                } },
                { typeof(DateTime), () => {
                    dbType = DbType.DateTime;
                } },
                { typeof(string), () => {
                    dbType = DbType.String;
                } },
                { typeof(decimal), () => {
                    dbType = DbType.Decimal;
                } },
                { typeof(bool), () => {
                    dbType = DbType.Boolean;
                } },
                { typeof(object), () => {
                    throw new Exception(value.GetType() + " not implemented");
                } }
            };

            @switch[value.GetType()]();

            var pLatestPickupDate = cmd.CreateParameter();
            pLatestPickupDate.DbType = dbType;
            pLatestPickupDate.ParameterName = name;
            pLatestPickupDate.Value = value;

            cmd.Parameters.Add(pLatestPickupDate);
        }
    }
}
