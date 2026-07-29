using System.Data;
using VQuery.Databases.SQLServer;
using System.Threading.Tasks;
using VQuery.Core;
using VQuery.Models;

namespace VQuery
{
    public class SQLServerConnection : IDisposable
    {

        private readonly _DBSQL _db = new();
        private readonly VariableConverter _convert = new();

        public void Dispose() { ConnectionClose(); }

        public bool ConnectionOpen(string connectionStringKey = "SQLSERVERConnection")
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


        public int Insert(string sql)
        {
            return _db._DataInsertSQL(sql);
        }

        public int Insert(string TableName, Dictionary<string, string> sql)
        {
            return _db._DataInsertSQL(TableName, sql);
        }

        public bool Update(string sql)
        {
            return _db._DataUpdateSQL(sql);

        }

        public bool Update(string TableName, Dictionary<string, string> sql, string Condition = " ")
        {
            return _db._DataUpdateSQL(TableName, sql, Condition);
        }

        public bool Delete(string sql)
        {
            return _db._DataDeleteSQL(sql);
        }

        private static bool IsSafeSqlIdentifier(string name)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(
                name,
                @"^[A-Za-z_][A-Za-z0-9_]*$");
        }



        public bool DeleteRecord(
            string tableName,
            string fieldName,
            object id)
        {
            if (!IsSafeSqlIdentifier(tableName))
                throw new ArgumentException("Invalid table name.");

            if (!IsSafeSqlIdentifier(fieldName))
                throw new ArgumentException("Invalid field name.");

            return _db._DataDeleteSQL(
                $"DELETE FROM [{tableName}] WHERE [{fieldName}] = @id",
                new Dictionary<string, object>
                {
                    ["@id"] = id
                });
        }

        public bool DataExecuteNonQuery(string sql)
        {
            return _db._DataExecuteNonQuerySQL(sql);
        }

        public bool DataExecuteTRANSACTION(List<string> SQL)
        {
            return _db._DataExecuteTRANSACTIONSQL(SQL);
        }


        public DataTable SelectDataTable(string sql)
        {
            return _db._SelectDataTableSQL(sql);
        }

        public DataTable SelectDataTable(string sql, Dictionary<string, string> Field_OVER_ORDER, int from_numrow, int to_numrow)
        {
            return _db._SelectDataTableSQL(sql, Field_OVER_ORDER, from_numrow, to_numrow);
        }


        public DataRow? SelectDataRow(string sql)
        {
            return _db._SelectDataRowSQL(sql);
        }


        public string SelectDataValue(string sql)
        {
            return _db._SelectDataValueSQL(sql);
        }

        public string SelectDataScalar(string sql)
        {
            return _db._SelectDataValueSQL(sql);
        }


        public DataTable SelectDataTable(
            string sql,
            Dictionary<string, object>? parameters)
        {
            return _db._SelectDataTableSQL(sql, parameters);
        }

        public DataRow? SelectDataRow(
            string sql,
            Dictionary<string, object>? parameters)
        {
            return _db._SelectDataRowSQL(sql, parameters);
        }

        public string SelectDataValue(
            string sql,
            Dictionary<string, object>? parameters)
        {
            return _db._SelectDataValueSQL(sql, parameters);
        }

        public int Insert(
            string sql,
            Dictionary<string, object>? parameters)
        {
            return _db._DataInsertSQL(sql, parameters);
        }

        public bool Update(
            string sql,
            Dictionary<string, object>? parameters)
        {
            return _db._DataUpdateSQL(sql, parameters);
        }

        public bool Delete(
            string sql,
            Dictionary<string, object>? parameters)
        {
            return _db._DataDeleteSQL(sql, parameters);
        }

        public bool Execute(string sql)
        {
            return _db._DataExecuteSQL(sql);
        }

        

        public bool Execute(
            string sql,
            Dictionary<string, object>? parameters)
        {
            return _db._DataExecuteSQL(sql, parameters);
        }

        public bool ExecuteRun(string sql)
        {
            return _db._DataExecuteSQLReturn(sql);
        }

        public bool ExecuteRun(
            string sql,
            Dictionary<string, object>? parameters)
        {
            return _db._DataExecuteSQLReturn(sql, parameters);
        }

        


        public void Begin(string Tran_ID = "")
        {
            _db._DataBeginSQL(Tran_ID);
        }

        public void Commit(string Tran_ID = "")
        {
            _db._DataCommitSQL(Tran_ID);
        }

        public void RollBack(string Tran_ID = "")
        {
            _db._DataRollBackSQL(Tran_ID);
        }

 
        // ============================================
        // ORM METHODS
        // ============================================

        public T? ExecuteScalar<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            return _db._ExecuteScalarSQL<T>(
                sql,
                parameters);
        }

        public async Task<T?> ExecuteScalarAsync<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            return await _db._ExecuteScalarSQLAsync<T>(
                sql,
                parameters);
        }

        public int ExecuteNonQuery(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            return _db._ExecuteNonQuerySQL(
                sql,
                parameters);
        }

        public async Task<int> ExecuteNonQueryAsync(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            return await _db._ExecuteNonQuerySQLAsync(
                sql,
                parameters);
        }

        public List<T> Query<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return _db._QueryReaderSQL<T>(
                sql,
                parameters);
        }

        public async Task<List<T>> QueryAsync<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return await _db._QueryReaderSQLAsync<T>(
                sql,
                parameters);
        }

        public T? QueryFirst<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return _db._QueryFirstReaderSQL<T>(
                sql,
                parameters);
        }

        public async Task<T?> QueryFirstAsync<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            return await _db._QueryFirstReaderSQLAsync<T>(
                sql,
                parameters);
        }

        public T QuerySingle<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            var list = Query<T>(
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

        public async Task<T> QuerySingleAsync<T>(
            string sql,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            var list = await QueryAsync<T>(
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
            var list = Query<T>(
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
            var list = await QueryAsync<T>(
                sql,
                parameters);

            if (list.Count > 1)
                throw new InvalidOperationException(
                    "More than one record found.");

            return list.Count == 0
                ? default
                : list[0];
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

        public MultiResult QueryMultiple(
            string sql,
            Dictionary<string, object>? parameters = null)
        {
            return _db._QueryMultipleSQL(
                sql,
                parameters);
        }

    }
}
