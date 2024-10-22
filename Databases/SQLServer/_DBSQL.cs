using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace VQuery.Databases.SQLServer
{
    public class _DBSQL: DBSQL
    {

        protected SqlConnection? _CN = null;
        protected string ESS = "";

        IConfigurationRoot config = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json").Build();

        //Execute SQL
        protected bool _DataExecuteNonQuerySQL(string sql)
        {
            if (_CN != null)
            {
                return DataExecuteNonQuerySQL(sql, _CN);
            }
            else
            {
                return false;
            }

        }
        //Execute SQL TRANSACTION
        protected bool _DataExecuteTRANSACTIONSQL(List<string> SQL)
        {
            if (_CN != null)
            {
                return DataExecuteTRANSACTIONSQL(SQL, _CN);
            }
            else
            {
                return false;
            }

        }

        public bool ConnectionOpen(string ConnectionStrings = "SQLSERVERConnection")
        {
            var server = config["ConnectionStrings:" + ConnectionStrings + ":server"];
            var database = config["ConnectionStrings:" + ConnectionStrings + ":database"];
            var username = config["ConnectionStrings:" + ConnectionStrings + ":username"];
            var password = config["ConnectionStrings:" + ConnectionStrings + ":password"];

            var myConnectionString = "Server=" + server + ";Initial Catalog=" + database + ";Persist Security Info=True;User ID=" + username + ";Password=" + password + ";";

            _CN = new SqlConnection(myConnectionString);
            // _CN = new SqlConnection(@WebConfigurationManager.ConnectionStrings[ConnectionStrings].ToString());
            try
            {
                bool b = true;
                if (_CN.State.ToString() != "Open")
                {
                    _CN.Open();
                    this._DataExecuteNonQuerySQL(" Set DateFormat DMY ");
                }
                return b;
            }
            catch
            {
                return false;
            }




        }


        public bool ConnectionClose()
        {
            ESS = "";
            if (_CN != null)
            {
                try
                {
                    _CN.Close();
                    return true;
                }
                catch (SqlException ex)
                {
                    ESS = (ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }

        }


        protected int _DataInsertSQL(string sql)
        {
            if (_CN != null)
            {
                return base.DataInsertSQL(sql, _CN);
            }
            else
            {
                return 0;
            }

        }

        protected int _DataInsertSQL(string TableName, Dictionary<string, string> sql)
        {
            if (_CN != null)
            {
                return DataInsertSQL(TableName, sql, _CN);
            }
            else
            {
                return 0;
            }

        }

        protected bool _DataUpdateSQL(string sql)
        {
            if (_CN != null)
            {
                return DataUpdateSQL(sql, _CN);
            }
            else
            {
                return false;
            }


        }

        protected bool _DataUpdateSQL(string TableName, Dictionary<string, string> sql, string Condition = " ")
        {
            return DataUpdateSQL(TableName, sql, Condition, _CN);
        }

        protected bool _DataDeleteSQL(string sql)
        {
            if (_CN != null)
            {
                return DataDeleteSQL(sql, _CN);
            }
            else
            {
                return false;
            }

        }

        protected DataTable _SelectDataTableSQL(string sql)
        {
            if (_CN != null) { return SelectDataTableSQL(sql, _CN); } else { return null; }

        }
        protected DataTable _SelectDataTableSQL(string sql, Dictionary<string, string> Field_OVER_ORDER, int from_numrow, int to_numrow)
        {
            if (_CN != null)
            {
                return SelectDataTableSQL(sql, Field_OVER_ORDER, from_numrow, to_numrow, _CN);
            }
            else
            {
                return null;
            }

        }

        protected DataRow _SelectDataRowSQL(string sql)
        {
            if (_CN != null)
            {
                return SelectDataRowSQL(sql, _CN);
            }
            else
            {
                return null;
            }

        }

        protected string _SelectDataValueSQL(string sql)
        {
            if (_CN != null)
            {
                return SelectDataValueSQL(sql, _CN);
            }
            else
            {
                return "";
            }

        }

        protected void _DataBeginSQL(string Tran_ID = "")
        {

            if (_CN != null) { DataBeginSQL(_CN, Tran_ID); }
        }

        protected void _DataCommitSQL(string Tran_ID = "")
        {
            if (_CN != null) { DataCommitSQL(_CN, Tran_ID); }
        }

        protected void _DataRollBackSQL(string Tran_ID = "")
        {
            if (_CN != null) { DataRollBackSQL(_CN, Tran_ID); }
        }

    }
}
