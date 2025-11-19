using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;


namespace VQuery.Databases.MySQL
{
    public class DBMYSQL: VariableConverter
    {
        protected DataTable SelectDataTableMysql(string MySQL, MySqlConnection Connection)
        {
            using (var cmd = new MySqlCommand(MySQL, Connection))
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
            using MySqlCommand mySqlCommand = new MySqlCommand(MySQL, Connection);
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
            using (var cmd = new MySqlCommand(query, Connection))
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

        protected int DataInsertMysql(string TableName, Dictionary<string, string> MySQL, MySqlConnection Connection)
        {
            string fields = string.Join(", ", MySQL.Keys);
            string parameters = string.Join(", ", MySQL.Keys.Select(k => "@" + k));

            string query = $"INSERT INTO {TableName} ({fields}) VALUES ({parameters}); SELECT LAST_INSERT_ID();";

            using (var cmd = new MySqlCommand(query, Connection))
            {
                foreach (var entry in MySQL)
                {
                    cmd.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                }

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

        protected bool DataUpdateMysql(string MySQL, MySqlConnection Connection)
        {
            using (var cmd = new MySqlCommand(MySQL, Connection))
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

        protected bool DataUpdateMysql(string TableName, Dictionary<string, string> MySQL, string Condition = "", MySqlConnection? Connection = null)
        {
            if (Connection == null || Connection.State != ConnectionState.Open)
                return false;

            string fields = string.Join(", ", MySQL.Select(entry => $"{entry.Key} = @{entry.Key}"));
            string query = $"UPDATE {TableName} SET {fields} {Condition}";

            using (var cmd = new MySqlCommand(query, Connection))
            {
                foreach (var entry in MySQL)
                {
                    cmd.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                }

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

        protected bool DataUpdateMysql(string tableName, Dictionary<string, string> columns, MySqlConnection? Connection = null, Dictionary<string, object> ?parameters = null)
        {
            try
            {
                if (Connection == null)
                    throw new ArgumentNullException(nameof(Connection), "The database connection is null.");

                // Construct the SET clause for the update query
                string setClause = string.Join(", ", columns.Select(c => $"{c.Key} = @{c.Key}"));

                // Construct the WHERE clause if parameters are provided
                string whereClause = parameters != null && parameters.Any()
                    ? " WHERE " + string.Join(" AND ", parameters.Select(p => $"{p.Key} = @{p.Key}"))
                    : string.Empty;

                // Build the final SQL query
                string sql = $"UPDATE {tableName} SET {setClause}{whereClause}";

                bool closeConnection = false;
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                    closeConnection = true;
                }

                using (var cmd = new MySqlCommand(sql, Connection))
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

                if (closeConnection)
                {
                    Connection.Close();
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
        
        
        protected bool DataUpdateMysql(string sql, MySqlConnection connection, Dictionary<string, object>? parameters = null)
        {
            try
            {
                if (connection == null)
                    throw new ArgumentNullException(nameof(connection), "Connection is null.");

                bool closeConnection = false;

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    closeConnection = true;
                }

                using var cmd = new MySqlCommand(sql, connection);

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                    }
                }

                cmd.ExecuteNonQuery();

                if (closeConnection)
                    connection.Close();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MySQL UPDATE Error] {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }



        protected bool DataDeleteMysql(string MySQL, MySqlConnection Connection)
        {
            using (var cmd = new MySqlCommand(MySQL, Connection))
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
            using (var cmd = new MySqlCommand(MySQL, Connection))
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
            using (var cmd = new MySqlCommand(MySQL, Connection))
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
            using (var cmd = new MySqlCommand(MySQL, Connection))
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

        public bool DataExecuteMysqlReturn(string MySQL, MySqlConnection Connection, Dictionary<string, object>? parameters = null)
        {
            using MySqlCommand mySqlCommand = new MySqlCommand(MySQL, Connection);
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
    }
}
