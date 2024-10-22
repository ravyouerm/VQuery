using System.Data;
using System.Data.SqlClient;

namespace VQuery.Databases.SQLServer
{
    public class DBSQL : SQLVariableConvert
    {

        //Insert statement
        protected int DataInsertSQL(string sql, SqlConnection CN)
        {
            int lastId = 0;
            string query = sql + "  ; SELECT SCOPE_IDENTITY() ; ";

            try
            {
                if (CN.State.ToString() == "Open")
                {
                    SqlCommand cmd = new(query, CN);
                    lastId = this.ToInt(cmd.ExecuteScalar());
                }
                return this.ToInt(lastId);
            }
            catch
            {

                return 0;
            }

        }

        protected int DataInsertSQL(string TableName, Dictionary<string, string> sql, SqlConnection CN)
        {
            int lastId = 0;

            string field = "";
            string value = "";

            int k = 0;
            try
            {
                foreach (KeyValuePair<string, string> entry in sql)
                {
                    k++;
                    field += (k != 1 ? "," : "") + "[" + entry.Key + "]";
                    value += (k != 1 ? "," : "") + "@" + entry.Key;
                }
                string query = "INSERT INTO " + TableName + "( " + field + " ) VALUES(" + value + ") ; SELECT IDENT_CURRENT('" + TableName + "'); ";


                if (CN.State.ToString() == "Open")
                {

                    SqlCommand cmd = new(query, CN);
                    cmd.CommandText = query;
                    foreach (KeyValuePair<string, string> entry in sql)
                    {
                        cmd.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                    }

                    lastId = this.ToInt(cmd.ExecuteScalar());
                }
                return this.ToInt(lastId);
            }
            catch
            {
                return 0;
            }
        }

        //Update statement
        protected bool DataUpdateSQL(string sql, SqlConnection CN)
        {
            string query = sql;
            try
            {

                if (CN.State.ToString() == "Open")
                {

                    SqlCommand cmd = new();

                    cmd.CommandText = query;

                    cmd.Connection = CN;


                    cmd.ExecuteNonQuery();
                    return true;

                }
                return false;
            }
            catch
            {
                return false;
            }

        }

        protected bool DataUpdateSQL(string TableName, Dictionary<string, string> sql, string Condition = " ", SqlConnection? CN = null)
        {
            string field = "";
            int k = 0;

            try
            {
                foreach (KeyValuePair<string, string> entry in sql)
                {
                    k++;
                    field += (k != 1 ? "," : "") + "[" + entry.Key + "] = @" + entry.Key + " ";
                }
                string query = " UPDATE " + TableName + " SET " + field + " " + Condition;

                if (CN != null)
                {
                    if (CN.State.ToString() == "Open")
                    {

                        SqlCommand cmd = new(query, CN);
                        cmd.CommandText = query;
                        foreach (KeyValuePair<string, string> entry in sql)
                        {
                            cmd.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                        }

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        //Delete statement
        protected bool DataDeleteSQL(string sql, SqlConnection CN)
        {
            string query = sql;
            try
            {
                if (CN.State.ToString() == "Open")
                {
                    SqlCommand cmd = new(query, CN);
                    cmd.ExecuteNonQuery();
                    return true;

                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        //Execute SQL
        protected bool DataExecuteNonQuerySQL(string sql, SqlConnection CN)
        {
            string query = sql;
            try
            {
                if (CN.State.ToString() == "Open")
                {
                    SqlCommand cmd = new(query, CN);
                    cmd.ExecuteNonQuery();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        //===Execute SQL TRANSACTION
        protected bool DataExecuteTRANSACTIONSQL(List<string> SQL, SqlConnection CN)
        {
            bool b = false;
            SqlCommand command = CN.CreateCommand();
            SqlTransaction transaction;

            // Start a local transaction.
            transaction = CN.BeginTransaction();

            // Must assign both transaction object and connection 
            // to Command object for a pending local transaction
            command.Connection = CN;
            command.Transaction = transaction;
            try
            {

                foreach (string Q in SQL)
                {
                    if (this.ToString(Q) != "")
                    {
                        command.CommandText = this.ToString(Q);
                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                b = true;
            }
            catch
            {
                b = false;


                try
                {
                    transaction.Rollback();
                }
                catch
                {
                    b = false;
                }

            }
            return b;
        }


        //=== Function Select Statement SQL ===============
        protected DataTable SelectDataTableSQL(string sql, SqlConnection CN)
        {
            string query = sql;


            DataTable d = new();
            SqlDataReader? dataReader = null;
            try
            {
                if (CN.State.ToString() == "Open")
                {
                    //Create Command
                    SqlCommand cmd = new(query, CN);
                    //Create a data reader and Execute the command
                    dataReader = cmd.ExecuteReader();

                    d.Load(dataReader);

                    //close Data Reader
                    dataReader.Close();

                    //return list to be displayed
                    return d;
                }
                else
                {
                    return d;
                }
            }
            catch
            {
                return d;
            }
        }


        protected DataTable SelectDataTableSQL(string sql, Dictionary<string, string> Field_OVER_ORDER, int from_numrow, int to_numrow, SqlConnection CN)
        {


            string Field_ORDER = "";

            int k = 0;
            DataTable d = new();

            try
            {
                foreach (KeyValuePair<string, string> entry in Field_OVER_ORDER)
                {
                    k++;
                    Field_ORDER += (k != 1 ? "," : "") + "[" + entry.Key + "] " + entry.Value;
                }
                from_numrow++;
                to_numrow++;
                string query = "SELECT * FROM ( SELECT *,ROW_NUMBER() OVER (ORDER BY " + Field_ORDER + ") AS RowNo  FROM (" + sql + ") x ) d_limit WHERE d_limit.RowNo BETWEEN " + from_numrow + " AND " + to_numrow + " ";



                if (CN.State.ToString() == "Open")
                {
                    //Create Command
                    SqlCommand cmd = new(query, CN);
                    //Create a data reader and Execute the command
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    d.Load(dataReader);

                    //close Data Reader
                    dataReader.Close();


                    //return list to be displayed
                    return d;
                }
                else
                {
                    return d;
                }
            }
            catch
            {

                return d;
            }
        }



        protected DataRow SelectDataRowSQL(string sql, SqlConnection CN)
        {
            DataRow? r = null;
            try
            {
                DataTable d = SelectDataTableSQL(sql, CN);

                if (d.Rows.Count > 0)
                {
                    r = d.Rows[0];
                }
                return r;
            }
            catch
            {

                return null;
            }
        }


        protected string SelectDataValueSQL(string sql, SqlConnection CN)
        {
            try
            {
                DataRow r = SelectDataRowSQL(sql, CN);
                return r[0].ToString();
            }
            catch
            {

                return "";
            }

        }


        protected void DataBeginSQL(SqlConnection CN, string Tran_ID = "")
        {
            string query = "BEGIN TRANSACTION " + Tran_ID;
            try
            {
                if (CN.State.ToString() == "Open")
                {
                    SqlCommand cmd = new(query, CN);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {

            }

        }

        protected void DataCommitSQL(SqlConnection CN, string Tran_ID = "")
        {

            string query = "COMMIT TRANSACTION " + Tran_ID;
            try
            {
                if (CN.State.ToString() == "Open")
                {
                    SqlCommand cmd = new(query, CN);
                    cmd.ExecuteNonQuery();

                }
            }
            catch
            {

            }


        }

        protected void DataRollBackSQL(SqlConnection CN, string Tran_ID = "")
        {
            string query = "ROLLBACK TRANSACTION " + Tran_ID;
            try
            {
                if (CN.State.ToString() == "Open")
                {
                    SqlCommand cmd = new(query, CN);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {

            }
        }


    }
}
