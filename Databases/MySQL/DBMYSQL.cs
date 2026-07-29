using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using VQuery.Core;


namespace VQuery.Databases.MySQL
{
    internal class DBMYSQL: VariableConverter
    { 
        protected MySqlTransaction? _Transaction;

        protected MySqlCommand CreateCommand(
            string sql,
            MySqlConnection connection)
        {
            var cmd =
                new MySqlCommand(
                    sql,
                    connection);

            if (_Transaction != null)
            {
                cmd.Transaction =
                    _Transaction;
            }

            return cmd;
        }
        private static readonly HashSet<string> ReservedWords =
            new(StringComparer.OrdinalIgnoreCase)
        {
            "SELECT",
            "INSERT",
            "UPDATE",
            "DELETE",
            "DROP",
            "ALTER",
            "CREATE",
            "TRUNCATE",
            "EXEC",
            "EXECUTE",
            "UNION",
            "FROM",
            "WHERE",
            "TABLE"
        };

        private static bool IsSafeIdentifier(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (!Regex.IsMatch(
                    name,
                    @"^[A-Za-z_][A-Za-z0-9_]*$",
                    RegexOptions.CultureInvariant))
            {
                return false;
            }

            return !ReservedWords.Contains(name);
        }

        protected DataTable SelectDataTableMysql(string MySQL, MySqlConnection Connection)
        {
            using (var cmd = CreateCommand(MySQL,Connection))
            {
                DataTable dataTable = new DataTable();
                try
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine(ex.Message);
                }
                return dataTable;
            }
        }

        protected DataTable SelectDataTableMysql(string MySQL, MySqlConnection Connection, Dictionary<string, object> ?parameters = null)
        {
            using MySqlCommand mySqlCommand = CreateCommand(MySQL,Connection);
            DataTable dataTable = new DataTable();

            try
            {
                // Step 1: Add parameters if provided
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        mySqlCommand.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                // Step 2: Execute command and load result into DataTable
                using MySqlDataReader reader = mySqlCommand.ExecuteReader();
                dataTable.Load(reader);
            }
            catch (Exception ex)
            {
                // Improved exception handling with full stack trace
                Console.WriteLine($"Error executing query: {ex.Message}\n{ex.StackTrace}");
            }

            return dataTable;
        }


        protected DataRow SelectDataRowMysql(string MySQL, MySqlConnection Connection)
        {
            DataTable dataTable = SelectDataTableMysql(MySQL, Connection);
            return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
        }

        protected DataRow SelectDataRowMysql(string MySQL, MySqlConnection Connection, Dictionary<string, object> ?parameters = null)
        {
            DataTable dataTable = SelectDataTableMysql(MySQL, Connection, parameters);
            return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
        }

        protected string SelectDataValueMysql(string MySQL, MySqlConnection Connection)
        {
            try
            {
                DataRow row = SelectDataRowMysql(MySQL, Connection);
                return row?[0]?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        protected string SelectDataValueMysql(string MySQL, MySqlConnection Connection, Dictionary<string, object> ?parameters = null)
        {
            try
            {
                DataRow row = SelectDataRowMysql(MySQL, Connection,parameters);
                return row?[0]?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        protected int DataInsertMysql(string MySQL, MySqlConnection Connection)
        {
            string query = MySQL + "; SELECT LAST_INSERT_ID();";
            using (var cmd = CreateCommand(query,Connection))
            {
                try
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine(ex.Message);
                    return 0;
                }
            }
        }

        

        protected int DataInsertMysql(
            string tableName,
            Dictionary<string, string> values,
            MySqlConnection connection)
        {
            if (!IsSafeIdentifier(tableName))
                throw new ArgumentException(
                    $"Invalid table name: {tableName}");

            foreach (string column in values.Keys)
            {
                if (!IsSafeIdentifier(column))
                    throw new ArgumentException(
                        $"Invalid column name: {column}");
            }

            string fields = string.Join(", ",
                values.Keys.Select(c => $"`{c}`"));

            string parameters = string.Join(", ",
                values.Keys.Select(c => $"@{c}"));

            string query =
                $"INSERT INTO `{tableName}` ({fields}) VALUES ({parameters}); SELECT LAST_INSERT_ID();";

            //using var cmd = new MySqlCommand(query, connection);
            using var cmd = CreateCommand(query,connection);

            foreach (var item in values)
            {
                cmd.Parameters.AddWithValue(
                    $"@{item.Key}",
                    item.Value ?? (object)DBNull.Value);
            }

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        protected int DataInsertMysql(
            string tableName,
            Dictionary<string, object> values,
            MySqlConnection connection)
        {
            if (!IsSafeIdentifier(tableName))
                throw new ArgumentException(
                    $"Invalid table name: {tableName}");

            foreach (string column in values.Keys)
            {
                if (!IsSafeIdentifier(column))
                    throw new ArgumentException(
                        $"Invalid column name: {column}");
            }

            string fields =
                string.Join(", ",
                    values.Keys.Select(x => $"`{x}`"));

            string parameters =
                string.Join(", ",
                    values.Keys.Select(x => $"@{x}"));

            string sql =
                $"INSERT INTO `{tableName}` ({fields}) " +
                $"VALUES ({parameters}); " +
                $"SELECT LAST_INSERT_ID();";

            /*using var cmd =
                new MySqlCommand(sql, connection);*/
            using var cmd = CreateCommand(sql,connection);

            foreach (var item in values)
            {
                cmd.Parameters.AddWithValue(
                    $"@{item.Key}",
                    item.Value ?? DBNull.Value);
            }

            return Convert.ToInt32(
                cmd.ExecuteScalar());
        }
        protected bool DataUpdateMysql(string MySQL, MySqlConnection Connection)
        {
            using (var cmd = CreateCommand(MySQL,Connection))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        protected bool DataUpdateMysql(
            string tableName,
            Dictionary<string, string> values,
            string condition = "",
            MySqlConnection? connection = null)
        {
            if (connection == null ||
                connection.State != ConnectionState.Open)
                return false;

            if (!IsSafeIdentifier(tableName))
                throw new ArgumentException(
                    $"Invalid table name: {tableName}");

            foreach (string column in values.Keys)
            {
                if (!IsSafeIdentifier(column))
                    throw new ArgumentException(
                        $"Invalid column name: {column}");
            }

            string fields =
                string.Join(", ",
                    values.Keys.Select(
                        x => $"`{x}`=@{x}"));

            string sql =
                $"UPDATE `{tableName}` SET {fields} {condition}";

            using var cmd = CreateCommand(sql,connection);


            foreach (var item in values)
            {
                cmd.Parameters.AddWithValue(
                    $"@{item.Key}",
                    item.Value ?? string.Empty);
            }

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        // FIX (bug #2): this overload previously opened the shared connection if it
        // happened to be closed, then closed it again afterwards ("closeConnection"
        // logic). That's wrong for this library: the connection is meant to be opened
        // once via MySQLConnection.ConnectionOpen() and reused across calls. Silently
        // closing it here could break whatever the caller does next. Now this method
        // simply requires an already-open connection, like every other method in this
        // class, and no longer touches the connection's open/closed state.
        protected bool DataUpdateMysql(string tableName, Dictionary<string, string> columns, MySqlConnection? Connection = null, Dictionary<string, object> ?parameters = null)
        {
            try
            {
                if (Connection == null)
                    throw new ArgumentNullException(nameof(Connection), "The database connection is null.");

                if (Connection.State != ConnectionState.Open)
                    throw new InvalidOperationException(
                        "The connection must already be open. Call ConnectionOpen() before running queries.");

                string whereClause = parameters != null && parameters.Any()
                    ? " WHERE " + string.Join(" AND ", parameters.Select(p => $"{p.Key} = @{p.Key}"))
                    : string.Empty;

                if (!IsSafeIdentifier(tableName))
                    throw new ArgumentException(
                        $"Invalid table name: {tableName}");

                foreach (string column in columns.Keys)
                {
                    if (!IsSafeIdentifier(column))
                        throw new ArgumentException(
                            $"Invalid column name: {column}");
                }

                string setClause =
                    string.Join(", ",
                        columns.Keys.Select(c => $"`{c}`=@{c}"));

                string sql =
                    $"UPDATE `{tableName}` SET {setClause} {whereClause}";

                using (var cmd = CreateCommand(sql,Connection))
                {
                    // Add column values as parameters for the SET clause
                    foreach (var column in columns)
                    {
                        cmd.Parameters.Add($"@{column.Key}", MySqlDbType.VarChar).Value = column.Value;
                    }

                    // Add parameters for the WHERE clause if provided
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue($"@{param.Key}", param.Value);
                        }
                    }

                    // Execute the update query
                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Use a logging framework here instead of Console.WriteLine for production
                Console.WriteLine($"Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return false;
            }
        }


        // FIX (bug #2): same issue as above — dropped the open/close side effect so
        // this method never changes the connection's open state on its own.
        protected bool DataUpdateMysql(string sql, MySqlConnection connection, Dictionary<string, object>? parameters = null)
        {
            try
            {
                if (connection == null)
                    throw new ArgumentNullException(nameof(connection), "Connection is null.");

                if (connection.State != ConnectionState.Open)
                    throw new InvalidOperationException(
                        "The connection must already be open. Call ConnectionOpen() before running queries.");

                using var cmd = CreateCommand(sql,connection);

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                }

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MySQL UPDATE Error] {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        protected bool DataUpdateMysql(
            string tableName,
            Dictionary<string, object> values,
            string condition,
            MySqlConnection connection)
        {
            try
            {
                if (!IsSafeIdentifier(tableName))
                    throw new ArgumentException(
                        $"Invalid table name: {tableName}");

                foreach (string column in values.Keys)
                {
                    if (!IsSafeIdentifier(column))
                        throw new ArgumentException(
                            $"Invalid column name: {column}");
                }

                string setClause =
                    string.Join(", ",
                        values.Keys.Select(
                            x => $"`{x}`=@{x}"));

                string sql =
                    $"UPDATE `{tableName}` " +
                    $"SET {setClause} {condition}";

                using var cmd = CreateCommand(sql,connection);

                foreach (var item in values)
                {
                    cmd.Parameters.AddWithValue(
                        $"@{item.Key}",
                        item.Value ?? DBNull.Value);
                }

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }


        protected bool DataUpdateMysql(
            string tableName,
            Dictionary<string, object> values,
            MySqlConnection connection,
            Dictionary<string, object>? whereParameters)
        {
            try
            {
                if (whereParameters != null)
                {
                    foreach (string column in whereParameters.Keys)
                    {
                        if (!IsSafeIdentifier(column))
                            throw new ArgumentException(
                                $"Invalid column name: {column}");
                    }
                }

                if (!IsSafeIdentifier(tableName))
                    throw new ArgumentException(
                        $"Invalid table name: {tableName}");

                foreach (string column in values.Keys)
                {
                    if (!IsSafeIdentifier(column))
                        throw new ArgumentException(
                            $"Invalid column name: {column}");
                }

                string setClause =
                    string.Join(", ",
                        values.Keys.Select(
                            x => $"`{x}`=@set_{x}"));

                string whereClause = "";

                if (whereParameters != null &&
                    whereParameters.Count > 0)
                {
                    whereClause =
                        " WHERE " +
                        string.Join(
                            " AND ",
                            whereParameters.Keys.Select(
                                x => $"`{x}`=@where_{x}"));
                }

                string sql =
                    $"UPDATE `{tableName}` " +
                    $"SET {setClause}" +
                    whereClause;

                using var cmd = CreateCommand(sql,connection);

                foreach (var item in values)
                {
                    cmd.Parameters.AddWithValue(
                        $"@set_{item.Key}",
                        item.Value ?? DBNull.Value);
                }

                if (whereParameters != null)
                {
                    foreach (var item in whereParameters)
                    {
                        cmd.Parameters.AddWithValue(
                            $"@where_{item.Key}",
                            item.Value ?? DBNull.Value);
                    }
                }

                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        protected bool DataDeleteMysql(string MySQL, MySqlConnection Connection)
        {
            using (var cmd = CreateCommand(MySQL,Connection))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        protected bool DataDeleteMysql(string MySQL, MySqlConnection Connection, Dictionary<string, object> ?parameters = null)
        {
            using (var cmd = CreateCommand(MySQL,Connection))
            {
                try
                {
                    // Add parameters if provided
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    // Execute the command
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    // Log the exception with stack trace for better debugging
                    Console.WriteLine($"Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    return false;
                }
            }
        }


        protected bool DataExecuteMysql(string MySQL, MySqlConnection Connection)
        {
            using (var cmd = CreateCommand(MySQL,Connection))
            {
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        protected bool DataExecuteMysql(string MySQL, MySqlConnection Connection, Dictionary<string, object>? parameters = null)
        {
            using (var cmd = CreateCommand(MySQL,Connection))
            {
                try
                {
                    // Add parameters if provided
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    // Execute the command
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    // Log the exception with stack trace for better debugging
                    Console.WriteLine($"Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    return false;
                }
            }
        }

        protected bool DataExecuteMysqlReturn(
            string sql,
            MySqlConnection connection)
        {
            //using var cmd = new MySqlCommand(sql, connection);
            using var cmd = CreateCommand(sql,connection);

            try
            {
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        protected bool DataExecuteMysqlReturn(string MySQL, MySqlConnection Connection, Dictionary<string, object>? parameters = null)
        {
            using MySqlCommand mySqlCommand = CreateCommand(MySQL,Connection);
            try
            {
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                        mySqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }

                int rows = mySqlCommand.ExecuteNonQuery();

                // ✅ Return true only if rows were affected
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message + "\nStackTrace: " + ex.StackTrace);
                return false;
            }
        }

        
        protected void DataBeginMysql(MySqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                _Transaction =
                    connection.BeginTransaction();
            }
        }

        protected void DataCommitMysql(MySqlConnection connection)
        {
            _Transaction?.Commit();
            _Transaction?.Dispose();
            _Transaction = null;
        }

        protected void DataRollBackMysql(MySqlConnection connection)
        {
            _Transaction?.Rollback();
            _Transaction?.Dispose();
            _Transaction = null;
        }
        


        

        protected T? ExecuteScalarMysql<T>(
            string sql,
            MySqlConnection connection,
            Dictionary<string, object>? parameters = null)
        {
            /*using var cmd =
                new MySqlCommand(sql, connection);*/
            using var cmd = CreateCommand(sql,connection);

            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.AddWithValue(
                        p.Key,
                        p.Value ?? DBNull.Value);
                }
            }

            object? result =
                cmd.ExecuteScalar();

            if (result == null ||
                result == DBNull.Value)
            {
                return default;
            }

            // FIX (bug #3): Convert.ChangeType throws InvalidCastException when T is
            // a nullable value type (e.g. int?), because typeof(T) is Nullable<int>,
            // which Convert.ChangeType doesn't support directly. MySQL's COUNT()/
            // scalar results also often come back as Int64, so we convert to the
            // underlying type first and let C# box it into the nullable result.
            Type targetType =
                Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            return (T)Convert.ChangeType(
                result,
                targetType);
        }

        protected int ExecuteNonQueryMysql(
            string sql,
            MySqlConnection connection,
            Dictionary<string, object>? parameters = null)
        {
            using var cmd = CreateCommand(sql,connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(
                        param.Key,
                        param.Value ?? DBNull.Value);
                }
            }

            try
            {
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }


        protected async Task<DataTable>
            SelectDataTableMysqlAsync(
                string sql,
                MySqlConnection connection,
                Dictionary<string, object>? parameters = null)
        {
            DataTable dt = new();

            using var cmd =
                CreateCommand(
                    sql,
                    connection);

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
                await cmd.ExecuteReaderAsync();

            dt.Load(reader);

            return dt;
        }


        protected async Task<int> ExecuteNonQueryMysqlAsync(
            string sql,
            MySqlConnection connection,
            Dictionary<string, object>? parameters = null)
        {
            using var cmd =
                CreateCommand(
                    sql,
                    connection);

            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.AddWithValue(
                        p.Key,
                        p.Value ?? DBNull.Value);
                }
            }

            return await cmd.ExecuteNonQueryAsync();
        }

        

        protected async Task<T?> ExecuteScalarMysqlAsync<T>(
            string sql,
            MySqlConnection connection,
            Dictionary<string, object>? parameters = null)
        {
            using var cmd =
                CreateCommand(
                    sql,
                    connection);

            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.AddWithValue(
                        p.Key,
                        p.Value ?? DBNull.Value);
                }
            }

            object? result =
                await cmd.ExecuteScalarAsync();

            if (result == null ||
                result == DBNull.Value)
            {
                return default;
            }

            // FIX (bug #3): same nullable-target issue as the sync version above.
            Type targetType =
                Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            return (T?)Convert.ChangeType(
                result,
                targetType);
        }

        


        protected async Task<DataRow?>
            SelectDataRowMysqlAsync(
                string sql,
                MySqlConnection connection,
                Dictionary<string, object>? parameters = null)
        {
            DataTable dt =
                await SelectDataTableMysqlAsync(
                    sql,
                    connection,
                    parameters);

            return dt.Rows.Count > 0
                ? dt.Rows[0]
                : null;
        }


        protected List<T> QueryReaderMysql<T>(
            string sql,
            MySqlConnection connection,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            List<T> list = new();

            using var cmd =
                CreateCommand(
                    sql,
                    connection);

            if (parameters != null)
            {
                foreach(var p in parameters)
                {
                    cmd.Parameters.AddWithValue(
                        p.Key,
                        p.Value ?? DBNull.Value);
                }
            }

            using var reader =
                cmd.ExecuteReader();

            while(reader.Read())
            {
                list.Add(
                    ReaderMapper.MapReader<T>(
                        reader));
            }

            return list;
        }

        protected T? QueryFirstReaderMysql<T>(
            string sql,
            MySqlConnection connection,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            using var cmd =
                CreateCommand(
                    sql,
                    connection);

            if (parameters != null)
            {
                foreach(var p in parameters)
                {
                    cmd.Parameters.AddWithValue(
                        p.Key,
                        p.Value ?? DBNull.Value);
                }
            }

            using var reader =
                cmd.ExecuteReader();

            if(reader.Read())
            {
                return ReaderMapper.MapReader<T>(
                    reader);
            }

            return default;
        }

        protected async Task<List<T>>
        QueryReaderMysqlAsync<T>(
            string sql,
            MySqlConnection connection,
            Dictionary<string, object>? parameters = null)
            where T : new()
        {
            List<T> list = new();

            using var cmd =
                CreateCommand(
                    sql,
                    connection);

            if (parameters != null)
            {
                foreach(var p in parameters)
                {
                    cmd.Parameters.AddWithValue(
                        p.Key,
                        p.Value ?? DBNull.Value);
                }
            }

            using var reader =
                await cmd.ExecuteReaderAsync();

            while(await reader.ReadAsync())
            {
                list.Add(
                    ReaderMapper.MapReader<T>(
                        reader));
            }

            return list;
        }

        protected async Task<T?>
            QueryFirstReaderMysqlAsync<T>(
                string sql,
                MySqlConnection connection,
                Dictionary<string, object>? parameters = null)
            where T : new()
        {
            using var cmd =
                CreateCommand(sql, connection);

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
                await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return ReaderMapper.MapReader<T>(
                    reader);
            }

            return default;
        }

        /*
        protected void DataBeginMysql(MySqlConnection Connection)
        {
            if (Connection.State == ConnectionState.Open)
            {
                using (var cmd = new MySqlCommand("START TRANSACTION", Connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void DataCommitMysql(MySqlConnection Connection)
        {
            if (Connection.State == ConnectionState.Open)
            {
                using (var cmd = new MySqlCommand("COMMIT", Connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void DataRollBackMysql(MySqlConnection Connection)
        {
            if (Connection.State == ConnectionState.Open)
            {
                using (var cmd = new MySqlCommand("ROLLBACK", Connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    
        */
    
    }
}
