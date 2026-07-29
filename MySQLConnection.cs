using System;
using System.Collections.Generic;
using System.Data;
using VQuery.Databases.MySQL;
using VQuery.Core;
using VQuery.Models;
using System.Threading.Tasks;
using System.Linq;

namespace VQuery
{
    public class MySQLConnection : IDisposable
    {
        private readonly _DBMYSQL _db = new();
        private readonly VariableConverter _convert = new();

        public void Dispose()
        {
            ConnectionClose();
        }

        #region Connection

        public bool ConnectionOpen(string connectionStringKey = "MYSQLConnection")
        {
            return _db.ConnectionOpen(connectionStringKey);
        }

        public bool ConnectionClose()
        {
            return _db.ConnectionClose();
        }

        public string dbx(string database = "")
        {
            return _db.dbx(database);
        }

        public bool IsConnected
        {
            get
            {
                return _db.IsConnected;
            }
        }

        #endregion

        #region VariableConverter

        public int ToInt(object? value)
        {
            return _convert.ToInt(value);
        }

        public double ToDouble(object? value)
        {
            return _convert.ToDouble(value);
        }

        public string ToIntString(object value)
        {
            return _convert.ToIntString(value);
        }

        public string ToDoubleString(object value)
        {
            return _convert.ToDoubleString(value);
        }

        public string ToString(object value)
        {
            return _convert.ToString(value);
        }

        public string ToString(DateTime value, string format = "dd-MM-yyyy")
        {
            return _convert.ToString(value, format);
        }

        public string ToStringDate(DateTime value, string format = "dd-MM-yyyy")
        {
            return _convert.ToStringDate(value, format);
        }

        public string ToStringDateTime(DateTime value, string format = "dd-MM-yyyy HH:mm:ss")
        {
            return _convert.ToStringDateTime(value, format);
        }

        public DateTime ToDate(object? value, string format = "dd-MM-yyyy")
        {
            return _convert.ToDate(value, format);
        }

        public DateTime ToDateTime(object? value, string format = "dd-MM-yyyy HH:mm:ss")
        {
            return _convert.ToDateTime(value, format);
        }

        public string NumberToText(long number)
        {
            return _convert.NumberToText(number);
        }

        public string NumberToTextKH(long number)
        {
            return _convert.NumberToTextKH(number);
        }

        public string NumberToKhNumber(double number)
        {
            return _convert.NumberToKhNumber(number);
        }

        public bool IsEmpty(DataRow row)
        {
            return _convert.IsEmpty(row);
        }

        #endregion

        #region Query

        public DataTable SelectDataTable(string mysql)
        {
            return _db._SelectDataTableMysql(mysql);
        }

        public DataTable SelectDataTable(string mysql, Dictionary<string, object>? parameters)
        {
            return _db._SelectDataTableMysql(mysql, parameters);
        }

        public DataRow? SelectDataRow(string mysql)
        {
            return _db._SelectDataRowMysql(mysql);
        }

        public DataRow? SelectDataRow(string mysql, Dictionary<string, object>? parameters)
        {
            return _db._SelectDataRowMysql(mysql, parameters);
        }

        public string SelectDataValue(string mysql)
        {
            return _db._SelectDataValueMysql(mysql);
        }

        public string SelectDataValue(string mysql, Dictionary<string, object>? parameters)
        {
            return _db._SelectDataValueMysql(mysql, parameters);
        }

        public string SelectDataScalar(string mysql)
        {
            return _db._SelectDataValueMysql(mysql);
        }

        public string SelectDataScalar(string mysql, Dictionary<string, object>? parameters)
        {
            return _db._SelectDataValueMysql(mysql, parameters);
        }


        

        public int Insert(string mysql)
        {
            return _db._DataInsertMysql(mysql);
        }

        public int Insert(
            string tableName,
            Dictionary<string, string> values)
        {
            return _db._DataInsertMysql(
                tableName,
                values);
        }

        public int Insert(
            string tableName,
            Dictionary<string, object> values)
        {
            return _db._DataInsertMysql(
                tableName,
                values);
        }

        public bool Update(string mysql)
        {
            return _db._DataUpdateMysql(mysql);
        }

        public bool Update(string mysql, Dictionary<string, object>? parameters)
        {
            return _db._DataUpdateMysql(mysql, parameters);
        }

        public bool Update(string tableName, Dictionary<string, string> values, string condition = "")
        {
            return _db._DataUpdateMysql(tableName, values, condition);
        }

        public bool Update(string tableName, Dictionary<string, string> values, Dictionary<string, object>? parameters)
        {
            return _db._DataUpdateMysql(tableName, values, parameters);
        }


        public bool Update(
            string tableName,
            Dictionary<string, object> values,
            string condition = "")
        {
            return _db._DataUpdateMysql(
                tableName,
                values,
                condition);
        }

        public bool Update(
            string tableName,
            Dictionary<string, object> values,
            Dictionary<string, object>? parameters)
        {
            return _db._DataUpdateMysql(
                tableName,
                values,
                parameters);
        }

        public bool Delete(string mysql)
        {
            return _db._DataDeleteMysql(mysql);
        }

        public bool Delete(string mysql, Dictionary<string, object>? parameters)
        {
            return _db._DataDeleteMysql(mysql, parameters);
        }

        public bool Execute(string mysql)
        {
            return _db._DataExecuteMysql(mysql);
        }

        public bool Execute(string mysql, Dictionary<string, object>? parameters)
        {
            return _db._DataExecuteMysql(mysql, parameters);
        }

        

        public bool ExecuteRun(string mysql)
        {
            return _db._DataExecuteMysqlReturn(mysql);
        }

        public bool ExecuteRun(string mysql, Dictionary<string, object>? parameters)
        {
            return _db._DataExecuteMysqlReturn(mysql, parameters);
        }

        public T? ExecuteScalar<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            return _db._ExecuteScalarMysql<T>(
                sql,
                parameters);
        }

        public List<T> Query<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return _db._QueryReaderMysql<T>(
                sql,
                parameters);
        }

        

        public T? QueryFirst<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return _db._QueryFirstReaderMysql<T>(
                sql,
                parameters);
        }

        public T QuerySingle<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            var list =
                Query<T>(
                    sql,
                    parameters);

            if (list.Count == 0)
                throw new InvalidOperationException(
                    "No record found.");

            if (list.Count > 1)
                throw new InvalidOperationException(
                    "More than one record found.");

            return list[0];
        }


        

        public async Task<List<T>>
            QueryAsync<T>(
                string sql,
                Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return await _db
                ._QueryReaderMysqlAsync<T>(
                    sql,
                    parameters);
        }


        public async Task<int>
            ExecuteNonQueryAsync(
                string sql,
                Dictionary<string, object>? parameters = null)
        {
            return await _db
                ._ExecuteNonQueryMysqlAsync(
                    sql,
                    parameters);
        }

        public async Task<T?>
            ExecuteScalarAsync<T>(
                string sql,
                Dictionary<string, object>? parameters = null)
        {
            return await _db
                ._ExecuteScalarMysqlAsync<T>(
                    sql,
                    parameters);
        }


        public async Task<bool>
            ExecuteAsync(
                string sql,
                Dictionary<string, object>? parameters = null)
        {
            int rows =
                await ExecuteNonQueryAsync(
                    sql,
                    parameters);

            return rows > 0;
        }



        public async Task<T?>
            QueryFirstAsync<T>(
                string sql,
                Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return await _db
                ._QueryFirstReaderMysqlAsync<T>(
                    sql,
                    parameters);
        }


        public async Task<T>
            QuerySingleAsync<T>(
                string sql,
                Dictionary<string, object>? parameters = null)
            where T : new()
        {
            var list =
                await QueryAsync<T>(
                    sql,
                    parameters);

            if (list.Count == 0)
            {
                throw new InvalidOperationException(
                    "No record found.");
            }

            if (list.Count > 1)
            {
                throw new InvalidOperationException(
                    "More than one record found.");
            }

            return list[0];
        }

        public async Task<bool>
            ExistsAsync(
                string sql,
                Dictionary<string, object>? parameters = null)
        {
            var result =
                await ExecuteScalarAsync<int?>(
                    sql,
                    parameters);

            return result.GetValueOrDefault() > 0;
        }

        public bool Exists(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            var result =
                ExecuteScalar<int?>(
                    sql,
                    parameters);

            return result.GetValueOrDefault() > 0;

        }
        

        public int ExecuteNonQuery(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            return _db._ExecuteNonQueryMysql(
                sql,
                parameters);
        }


     



        public void Begin()
        {
            _db._DataBeginMysql();
        }

        public void Commit()
        {
            _db._DataCommitMysql();
        }

        public void RollBack()
        {
            _db._DataRollBackMysql();
        }


        public MultiResult QueryMultiple(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            return _db._QueryMultipleMysql(
                sql,
                parameters);
        }


        public T? QueryFirstOrDefault<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return QueryFirst<T>(
                sql,
                parameters);
        }


        public async Task<T?>
            QueryFirstOrDefaultAsync<T>(
                string sql,
                Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return await QueryFirstAsync<T>(
                sql,
                parameters);
        }


        public T? QuerySingleOrDefault<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            var list =
                Query<T>(
                    sql,
                    parameters);

            if (list.Count == 0)
                return default;

            if (list.Count > 1)
            {
                throw new InvalidOperationException(
                    "More than one record found.");
            }

            return list[0];
        }


        public async Task<T?>
            QuerySingleOrDefaultAsync<T>(
                string sql,
                Dictionary<string, object>? parameters = null)
            where T : new()
        {
            var list =
                await QueryAsync<T>(
                    sql,
                    parameters);

            if (list.Count == 0)
                return default;

            if (list.Count > 1)
            {
                throw new InvalidOperationException(
                    "More than one record found.");
            }

            return list[0];
        }

        

        #endregion
    }
}