using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace VQuery.Databases.PostgreSQL
{
    public class _DBPOSTGRESQL : DBPOSTGRESQL
    {
        NpgsqlConnection? _connection;

        protected string ESS = "";

        IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json").Build();



        public bool ConnectionOpen(string ConnectionStrings = "POSTGREConnection")
        {
            var server = config["ConnectionStrings:" + ConnectionStrings + ":server"];
            var database = config["ConnectionStrings:" + ConnectionStrings + ":database"];
            var username = config["ConnectionStrings:" + ConnectionStrings + ":username"];
            var password = config["ConnectionStrings:" + ConnectionStrings + ":password"];
            var port = config["ConnectionStrings:" + ConnectionStrings + ":port"]; //5432

            var myConnectionString = "Server=" + server + ";User Id=" + username + ";Database=" + database + ";Port=" + port + ";Password=" + password + ";";

            _connection = new NpgsqlConnection(myConnectionString);



            try
            {
                bool b = true;
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
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

            try
            {
                if (_connection != null)
                {
                    _connection.Close();
                    return true;
                }
                else { return false; }

            }
            catch (NpgsqlException ex)
            {
                ESS = (ex.Message);
                return false;
            }
        }

        protected DataTable _SelectDataTablePostgre(string postgresql)
        {
            if (_connection != null)
            {
                return SelectDataTablePostgre(postgresql, _connection);
            }
            else
            {
                return null;
            }

        }

        protected DataRow _SelectDataRowPostgre(string postgresql)
        {
            if (_connection != null)
            {
                return SelectDataRowPostgre(postgresql, _connection);
            }
            else
            {
                return null;
            }

        }

        protected string _SelectDataValuePostgre(string postgresql)
        {
            if (_connection != null)
            {
                return SelectDataValuePostgre(postgresql, _connection);
            }
            else
            {
                return "";
            }

        }

        protected int _DataInsertPostgre(string postgresql)
        {
            if (_connection != null)
            {
                return base.DataInsertPostgre(postgresql, _connection);
            }
            else
            {
                return 0;
            }

        }

        protected int _DataInsertPostgre(string TableName, Dictionary<string, string> postgresql)
        {
            if (_connection != null)
            {
                return DataInsertPostgre(TableName, postgresql, _connection);
            }
            else
            {
                return 0;
            }

        }

        protected bool _DataUpdatePostgre(string postgresql)
        {
            if (_connection != null)
            {
                return DataUpdatePostgre(postgresql, _connection);
            }
            else
            {
                return false;
            }

        }

        protected bool _DataUpdatePostgre(string TableName, Dictionary<string, string> postgresql, string Condition = " ")
        {
            return DataUpdatePostgre(TableName, postgresql, Condition, _connection);
        }

        protected bool _DataDeletePostgre(string postgresql)
        {
            if (_connection != null) { return DataDeletePostgre(postgresql, _connection); } else { return false; }
        }

        protected void _DataBeginPostgre()
        {
            if (_connection != null) { DataBeginPostgre(_connection); }
        }

        protected void _DataCommitPostgre()
        {
            if (_connection != null) { DataCommitPostgre(_connection); }
        }

        protected void _DataRollBackPostgre()
        {
            if (_connection != null) { DataRollBackPostgre(_connection); }
        }
    }
}
