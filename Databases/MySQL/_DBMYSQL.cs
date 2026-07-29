using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using VQuery.Models;
using System.Threading.Tasks;
using System.Linq;

namespace VQuery.Databases.MySQL
{
    internal class _DBMYSQL: DBMYSQL
    {

        
        private MySqlConnection? _Connection;
        private IConfigurationRoot _config;
        

        internal _DBMYSQL()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        internal string dbx(string database = "")
        {
            if (database == "")
            {
                database = _config["ConnectionStrings:MYSQLConnection2:database"];
            }

            return database;
        }

        internal bool IsConnected
        {
            get
            {
                try
                {
                    return _Connection != null &&
                        _Connection.State == ConnectionState.Open;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }
        }

        internal bool ConnectionOpen(
            string connectionStringKey = "MYSQLConnection")
        {
            try
            {
                string connectionString =
                    BuildConnectionString(connectionStringKey);

                if (_Connection != null)
                {
                    if (_Connection.State == ConnectionState.Open)
                        _Connection.Close();

                    _Connection.Dispose();
                }

                _Connection =
                    new MySqlConnection(connectionString);

                _Connection.Open();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        /*internal string BuildConnectionString(string connectionStringKey)
        {
            var server = _config[$"ConnectionStrings:{connectionStringKey}:server"];
            var database = _config[$"ConnectionStrings:{connectionStringKey}:database"];
            var username = _config[$"ConnectionStrings:{connectionStringKey}:username"];
            var password = _config[$"ConnectionStrings:{connectionStringKey}:password"];
            var port = _config[$"ConnectionStrings:{connectionStringKey}:port"];

            return $"server={server};user={username};database={database};port={port};password={password};Persist Security Info=False;Convert Zero Datetime=True;";
        }*/

        internal string BuildConnectionString(
            string connectionStringKey)
        {
            var server =
                _config[$"ConnectionStrings:{connectionStringKey}:server"];

            var database =
                _config[$"ConnectionStrings:{connectionStringKey}:database"];

            var username =
                _config[$"ConnectionStrings:{connectionStringKey}:username"];

            var password =
                _config[$"ConnectionStrings:{connectionStringKey}:password"];

            var port =
                _config[$"ConnectionStrings:{connectionStringKey}:port"];

            var charset =
                _config[$"ConnectionStrings:{connectionStringKey}:charset"]
                ?? "utf8mb4";

            var pooling =
                _config[$"ConnectionStrings:{connectionStringKey}:pooling"]
                ?? "true";

            var maxPoolSize =
                _config[$"ConnectionStrings:{connectionStringKey}:maxPoolSize"]
                ?? "100";

            var connectionTimeout =
                _config[$"ConnectionStrings:{connectionStringKey}:connectionTimeout"]
                ?? "30";

            var commandTimeout =
                _config[$"ConnectionStrings:{connectionStringKey}:commandTimeout"]
                ?? "60";
            var minimumPoolSize =
                _config[$"ConnectionStrings:{connectionStringKey}:minimumPoolSize"]
                ?? "5";
            


            return
                $"Server={server};" +
                $"Port={port};" +
                $"Database={database};" +
                $"User ID={username};" +
                $"Password={password};" +
                $"Charset={charset};" +
                $"Pooling={pooling};" +
                $"MaximumPoolSize={maxPoolSize};" +
                $"MinimumPoolSize={minimumPoolSize};" +
                $"ConnectionTimeout={connectionTimeout};" +
                $"DefaultCommandTimeout={commandTimeout};" +
                $"Persist Security Info=False;" +
                $"Convert Zero Datetime=True;";
        }


        internal bool ConnectionClose()
        {
            try
            {
                if (_Connection != null)
                {
                    if (_Connection.State != ConnectionState.Closed)
                        _Connection.Close();

                    _Connection.Dispose();
                    _Connection = null;

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            return false;
        }

        internal DataTable _SelectDataTableMysql(string mysql)
        {
            return _Connection != null ? SelectDataTableMysql(mysql, _Connection) : new DataTable();
        }

        internal DataTable _SelectDataTableMysql(string mysql,Dictionary<string, object> ?parameters = null)
        {
            return _Connection != null ? SelectDataTableMysql(mysql, _Connection, parameters) : new DataTable();
        }

        internal DataRow? _SelectDataRowMysql(string mysql)
        {
            return _Connection != null ? SelectDataRowMysql(mysql, _Connection) : null;
        }

        internal DataRow? _SelectDataRowMysql(string mysql,Dictionary<string, object> ?parameters = null)
        {
            return _Connection != null ? SelectDataRowMysql(mysql, _Connection, parameters) : null;
        }

        internal string _SelectDataValueMysql(string mysql)
        {
            return _Connection != null ? SelectDataValueMysql(mysql, _Connection) : string.Empty;
        }

        internal string _SelectDataValueMysql(string mysql, Dictionary<string, object> ?parameters = null)
        {
            return _Connection != null ? SelectDataValueMysql(mysql, _Connection, parameters) : string.Empty;
        }

        internal int _DataInsertMysql(string mysql)
        {
            return _Connection != null ? DataInsertMysql(mysql, _Connection) : 0;
        }

        internal int _DataInsertMysql(string tableName, Dictionary<string, string> mysql)
        {
            
            return _Connection != null ? DataInsertMysql(tableName, mysql, _Connection) : 0;
        }


        internal int _DataInsertMysql(
            string tableName,
            Dictionary<string, object> values)
        {
            return _Connection != null
                ? DataInsertMysql(
                    tableName,
                    values,
                    _Connection)
                : 0;
        }

        internal bool _DataUpdateMysql(string mysql)
        {
            return _Connection != null && DataUpdateMysql(mysql, _Connection);
        }


        internal bool _DataUpdateMysql(
            string tableName,
            Dictionary<string, string> mysql,
            string condition = " ")
        {
            if (_Connection == null)
                return false;

            return DataUpdateMysql(
                tableName,
                mysql,
                condition,
                _Connection);
        }

        internal bool _DataUpdateMysql(string tableName, Dictionary<string, string> mysql, Dictionary<string, object> ?parameters = null)
        {
            if (_Connection == null)
                return false;

            return DataUpdateMysql(
                tableName,
                mysql, 
                _Connection, 
                parameters);
        }

        internal bool _DataUpdateMysql(string mysql , Dictionary<string, object> ?parameters = null)
        {
            if (_Connection == null)
                return false;

            return DataUpdateMysql(mysql, _Connection, parameters);
        }


        internal bool _DataUpdateMysql(
            string tableName,
            Dictionary<string, object> values,
            string condition = "")
        {
            if (_Connection == null)
                return false;

            return DataUpdateMysql(
                tableName,
                values,
                condition,
                _Connection);
        }

        internal bool _DataUpdateMysql(
            string tableName,
            Dictionary<string, object> values,
            Dictionary<string, object>? parameters)
        {
            if (_Connection == null)
                return false;

            return DataUpdateMysql(
                tableName,
                values,
                _Connection,
                parameters);
        }

        internal bool _DataDeleteMysql(string mysql)
        {
            return _Connection != null && DataDeleteMysql(mysql, _Connection);
        }

        internal bool _DataDeleteMysql(string mysql, Dictionary<string, object> ?parameters = null)
        {
            return _Connection != null && DataDeleteMysql(mysql, _Connection, parameters);
        }


        internal bool _DataExecuteMysql(string mysql)
        {
            return _Connection != null && DataExecuteMysql(mysql, _Connection);
        }

        internal bool _DataExecuteMysql(string mysql, Dictionary<string, object>? parameters = null)
        {
            return _Connection != null && DataExecuteMysql(mysql, _Connection, parameters);
        }

        internal bool _DataExecuteMysqlReturn(string mysql)
        {
            return _Connection != null &&
                DataExecuteMysqlReturn(mysql, _Connection);
        }

        internal bool _DataExecuteMysqlReturn(string mysql, Dictionary<string, object>? parameters = null)
        {
            return _Connection != null && DataExecuteMysqlReturn(mysql, _Connection, parameters);
        }

        internal void _DataBeginMysql()
        {
            if (_Connection != null)
            {
                DataBeginMysql(_Connection);
            }
        }

        internal void _DataCommitMysql()
        {
            if (_Connection != null)
            {
                DataCommitMysql(_Connection);
            }
        }

        internal void _DataRollBackMysql()
        {
            if (_Connection != null)
            {
                DataRollBackMysql(_Connection);
            }
        }


        internal T? _ExecuteScalarMysql<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            if (_Connection == null)
                return default;

            return ExecuteScalarMysql<T>(
                sql,
                _Connection,
                parameters);
        }

        internal int _ExecuteNonQueryMysql(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            if (_Connection == null)
                return 0;

            return ExecuteNonQueryMysql(
                sql,
                _Connection,
                parameters);
        }


        internal async Task<DataTable>
            _SelectDataTableMysqlAsync(
                string sql,
                Dictionary<string, object>? parameters = null)
        {
            if (_Connection == null)
                return new DataTable();

            return await
                SelectDataTableMysqlAsync(
                    sql,
                    _Connection,
                    parameters);
        }

        internal MultiResult _QueryMultipleMysql(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            MultiResult result = new();

            if (_Connection == null)
                return result;

            using var cmd =
                CreateCommand(
                    sql,
                    _Connection);

            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.AddWithValue(
                        p.Key,
                        p.Value ?? DBNull.Value);
                }
            }

            using var reader =
                cmd.ExecuteReader();


            do
            {
                if (reader.HasRows)
                {
                    DataTable dt = new();

                    dt.Load(reader);

                    result.AddTable(dt);
                }
            }
            while (reader.NextResult());

            return result;
        }

        internal async Task<int>
            _ExecuteNonQueryMysqlAsync(
                string sql,
                Dictionary<string, object>? parameters = null)
        {
            if (_Connection == null)
                return 0;

            return await ExecuteNonQueryMysqlAsync(
                sql,
                _Connection,
                parameters);
        }


        internal async Task<T?>
            _ExecuteScalarMysqlAsync<T>(
                string sql,
                Dictionary<string, object>? parameters = null)
        {
            if (_Connection == null)
                return default;

            return await ExecuteScalarMysqlAsync<T>(
                sql,
                _Connection,
                parameters);
        }


        internal async Task<DataRow?>
            _SelectDataRowMysqlAsync(
                string sql,
                Dictionary<string, object>? parameters = null)
        {
            if (_Connection == null)
                return null;

            return await SelectDataRowMysqlAsync(
                sql,
                _Connection,
                parameters);
        }


        internal List<T> _QueryReaderMysql<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            if (_Connection == null)
                return new();

            return QueryReaderMysql<T>(
                sql,
                _Connection,
                parameters);
        }

        internal T? _QueryFirstReaderMysql<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            if (_Connection == null)
                return default;

            return QueryFirstReaderMysql<T>(
                sql,
                _Connection,
                parameters);
        }

        internal async Task<List<T>>
            _QueryReaderMysqlAsync<T>(
                string sql,
                Dictionary<string, object>? parameters = null)
            where T : new()
        {
            if (_Connection == null)
                return new();

            return await QueryReaderMysqlAsync<T>(
                sql,
                _Connection,
                parameters);
        }

        internal async Task<T?>
            _QueryFirstReaderMysqlAsync<T>(
                string sql,
                Dictionary<string, object>? parameters = null)
            where T : new()
        {
            if (_Connection == null)
                return default;

            return await QueryFirstReaderMysqlAsync<T>(
                sql,
                _Connection,
                parameters);
        }
        



    }
}
