using System;
using System.Collections.Generic;
using System.Data;
using VQuery.Databases.PostgreSQL;
using System.Threading.Tasks;
using VQuery.Models;

namespace VQuery
{
    public class PostgreSQLConnection : IDisposable
    {
        private readonly _DBPOSTGRESQL _db = new();
        private readonly VariableConverter _convert = new();

        public void Dispose()
        {
            ConnectionClose();
        }
        


        #region Connection

        public bool ConnectionOpen(string connectionStringKey = "POSTGREConnection")
        {
            return _db.ConnectionOpen(connectionStringKey);
        }

        public bool ConnectionClose()
        {
            return _db.ConnectionClose();
        }

        public bool IsConnected
        {
            get
            {
                return _db.IsConnected;
            }
        }

        public string dbx(string database = "")
        {
            return _db.dbx(database);
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

        public DataTable SelectDataTable(string postgresql)
        {
            return _db._SelectDataTablePostgre(postgresql);
        }

        public DataTable SelectDataTable(string postgresql, Dictionary<string, object>? parameters)
        {
            return _db._SelectDataTablePostgre(postgresql, parameters);
        }

        public DataRow? SelectDataRow(string postgresql)
        {
            return _db._SelectDataRowPostgre(postgresql);
        }

        public DataRow? SelectDataRow(
            string postgresql,
            Dictionary<string, object>? parameters)
        {
            return _db._SelectDataRowPostgre(
                postgresql,
                parameters);
        }

        public string SelectDataValue(string postgresql)
        {
            return _db._SelectDataValuePostgre(postgresql);
        }

        public string SelectDataValue(string postgresql, Dictionary<string, object>? parameters)
        {
            return _db._SelectDataValuePostgre(postgresql, parameters);
        }

        public string SelectDataScalar(string postgresql)
        {
            return _db._SelectDataValuePostgre(postgresql);
        }

        public string SelectDataScalar(string postgresql, Dictionary<string, object>? parameters)
        {
            return _db._SelectDataValuePostgre(postgresql, parameters);
        }

        public int Insert(string postgresql)
        {
            return _db._DataInsertPostgre(postgresql);
        }

        public int Insert(string tableName, Dictionary<string, string> values)
        {
            return _db._DataInsertPostgre(tableName, values);
        }

        public int Insert(
            string postgresql,
            Dictionary<string, object>? parameters)
        {
            return _db._DataInsertPostgre(
                postgresql,
                parameters);
        }

        public bool Update(string postgresql)
        {
            return _db._DataUpdatePostgre(postgresql);
        }

        public bool Update(string postgresql, Dictionary<string, object>? parameters)
        {
            return _db._DataUpdatePostgre(postgresql, parameters);
        }

        public bool Update(string tableName,
                        Dictionary<string, string> values,
                        string condition = "")
        {
            return _db._DataUpdatePostgre(tableName, values, condition);
        }

        public bool Update(
            string tableName,
            Dictionary<string, string> values,
            Dictionary<string, object>? parameters)
        {
            return _db._DataUpdatePostgre(
                tableName,
                values,
                parameters);
        }

        public bool Delete(string postgresql)
        {
            return _db._DataDeletePostgre(postgresql);
        }

        public bool Delete(string postgresql, Dictionary<string, object>? parameters)
        {
            return _db._DataDeletePostgre(postgresql, parameters);
        }

        public bool Execute(string postgresql)
        {
            return _db._DataExecutePostgre(postgresql);
        }

        
        public bool Execute(string postgresql, Dictionary<string, object>? parameters)
        {
            return _db._DataExecutePostgre(postgresql, parameters);
        }

        public bool ExecuteRun(string postgresql, Dictionary<string, object>? parameters)
        {
            return _db._DataExecutePostgreReturn(postgresql, parameters);
        }
        
        public bool ExecuteRun(string sql)
        {
            return _db._DataExecutePostgreReturn(
                sql,
                null);
        }

        public void Begin()
        {
            _db._DataBeginPostgre();
        }

        public void Commit()
        {
            _db._DataCommitPostgre();
        }

        public void RollBack()
        {
            _db._DataRollBackPostgre();
        }

        #endregion

        public T? ExecuteScalar<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            return _db._ExecuteScalarPostgre<T>(
                sql,
                parameters);
        }

        public int ExecuteNonQuery(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            return _db._ExecuteNonQueryPostgre(
                sql,
                parameters);
        }

        public List<T> Query<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return _db._QueryReaderPostgre<T>(
                sql,
                parameters);
        }

        public T? QueryFirst<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return _db._QueryFirstReaderPostgre<T>(
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


        public async Task<List<T>>
            QueryAsync<T>(
                string sql,
                Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return await _db
                ._QueryReaderPostgreAsync<T>(
                    sql,
                    parameters);
        }

        public async Task<T?>
            QueryFirstAsync<T>(
                string sql,
                Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return await _db
                ._QueryFirstReaderPostgreAsync<T>(
                    sql,
                    parameters);
        }

        public MultiResult QueryMultiple(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            return _db._QueryMultiplePostgre(
                sql,
                parameters);
        }

        public async Task<T> QuerySingleAsync<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            var list =
                await QueryAsync<T>(
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

        public async Task<bool> ExistsAsync(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            var result =
                await ExecuteScalarAsync<int?>(
                    sql,
                    parameters);

            return result.GetValueOrDefault() > 0;
        }

        public async Task<T?> ExecuteScalarAsync<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            return await _db._ExecuteScalarPostgreAsync<T>(
                sql,
                parameters);
        }


        public async Task<int> ExecuteNonQueryAsync(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            return await _db
                ._ExecuteNonQueryPostgreAsync(
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

        public async Task<T?> QueryFirstOrDefaultAsync<T>(
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

            if (list.Count > 1)
                throw new InvalidOperationException(
                    "More than one record found.");

            return list.Count == 0
                ? default
                : list[0];
        }


        public async Task<T?> QuerySingleOrDefaultAsync<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            var list =
                await QueryAsync<T>(
                    sql,
                    parameters);

            if (list.Count > 1)
                throw new InvalidOperationException(
                    "More than one record found.");

            return list.Count == 0
                ? default
                : list[0];
        }
        


    }

}
