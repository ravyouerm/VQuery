using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace VQuery.Databases.MySQL
{
    public class _DBMYSQL: DBMYSQL
    {

        
        private MySqlConnection _Connection;
        private IConfigurationRoot _config;

        public _DBMYSQL()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public string dbx(string database = "")
        {
            if (database == "")
            {
                database = _config["ConnectionStrings:MSQLConnection2:database"];
            }

            return database;
        }

        public bool ConnectionOpen(string connectionStringKey = "MSQLConnection")
        {
            var connectionString = BuildConnectionString(connectionStringKey);
            _Connection = new MySqlConnection(connectionString);

            try
            {
                _Connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                // Log exception (consider using a logging framework)
                Console.WriteLine($"Error opening connection: {ex.Message}");
                return false;
            }
        }

        private string BuildConnectionString(string connectionStringKey)
        {
            var server = _config[$"ConnectionStrings:{connectionStringKey}:server"];
            var database = _config[$"ConnectionStrings:{connectionStringKey}:database"];
            var username = _config[$"ConnectionStrings:{connectionStringKey}:username"];
            var password = _config[$"ConnectionStrings:{connectionStringKey}:password"];
            var port = _config[$"ConnectionStrings:{connectionStringKey}:port"];

            return $"server={server};user={username};database={database};port={port};password={password};persistsecurityinfo=True;Convert Zero Datetime=True;";
        }

        public bool ConnectionClose()
        {
            try
            {
                if (_Connection != null && _Connection.State == ConnectionState.Open)
                {
                    _Connection.Close();
                    return true;
                }
            }
            catch (MySqlException ex)
            {
                // Log exception (consider using a logging framework)
                Console.WriteLine($"Error closing connection: {ex.Message}");
            }

            return false;
        }

        protected DataTable _SelectDataTableMysql(string mysql)
        {
            return _Connection != null ? SelectDataTableMysql(mysql, _Connection) : null;
        }

        protected DataTable _SelectDataTableMysql(string mysql,Dictionary<string, object> ?parameters = null)
        {
            return _Connection != null ? SelectDataTableMysql(mysql, _Connection, parameters) : null;
        }

        protected DataRow _SelectDataRowMysql(string mysql)
        {
            return _Connection != null ? SelectDataRowMysql(mysql, _Connection) : null;
        }

        protected DataRow _SelectDataRowMysql(string mysql,Dictionary<string, object> ?parameters = null)
        {
            return _Connection != null ? SelectDataRowMysql(mysql, _Connection, parameters) : null;
        }

        protected string _SelectDataValueMysql(string mysql)
        {
            return _Connection != null ? SelectDataValueMysql(mysql, _Connection) : string.Empty;
        }

        protected string _SelectDataValueMysql(string mysql, Dictionary<string, object> ?parameters = null)
        {
            return _Connection != null ? SelectDataValueMysql(mysql, _Connection, parameters) : string.Empty;
        }

        protected int _DataInsertMysql(string mysql)
        {
            return _Connection != null ? DataInsertMysql(mysql, _Connection) : 0;
        }

        protected int _DataInsertMysql(string tableName, Dictionary<string, string> mysql)
        {
            return _Connection != null ? DataInsertMysql(tableName, mysql, _Connection) : 0;
        }

        protected bool _DataUpdateMysql(string mysql)
        {
            return _Connection != null && DataUpdateMysql(mysql, _Connection);
        }

        protected bool _DataUpdateMysql(string tableName, Dictionary<string, string> mysql, string condition = " ")
        {
            return DataUpdateMysql(tableName, mysql, condition, _Connection);
        }

        protected bool _DataUpdateMysql(string tableName, Dictionary<string, string> mysql, Dictionary<string, object> ?parameters = null)
        {
            return DataUpdateMysql(tableName, mysql, _Connection, parameters);
        }

        protected bool _DataDeleteMysql(string mysql)
        {
            return _Connection != null && DataDeleteMysql(mysql, _Connection);
        }

        protected bool _DataDeleteMysql(string mysql, Dictionary<string, object> ?parameters = null)
        {
            return _Connection != null && DataDeleteMysql(mysql, _Connection, parameters);
        }

        protected void _DataBeginMysql()
        {
            if (_Connection != null)
            {
                DataBeginMysql(_Connection);
            }
        }

        protected void _DataCommitMysql()
        {
            if (_Connection != null)
            {
                DataCommitMysql(_Connection);
            }
        }

        protected void _DataRollBackMysql()
        {
            if (_Connection != null)
            {
                DataRollBackMysql(_Connection);
            }
        }
        



    }
}
