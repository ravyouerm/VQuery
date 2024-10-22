using System.Data;
using Npgsql;

namespace VQuery.Databases.PostgreSQL
{
    public class DBPOSTGRESQL : VariableConverter
    {
        protected DataTable SelectDataTablePostgre(string postgresql, NpgsqlConnection connection)
        {
            string query = postgresql;



            DataTable d = new();
            try
            {
                if (connection.State.ToString() == "Open")
                {
                    //Create Command
                    NpgsqlCommand cmd = new(query, connection);
                    //Create a data reader and Execute the command
                    NpgsqlDataReader dataReader = cmd.ExecuteReader();

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


        protected DataRow SelectDataRowPostgre(string postgresql, NpgsqlConnection connection)
        {
            DataRow? r = null;
            try
            {
                DataTable d = SelectDataTablePostgre(postgresql, connection);

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

        protected string SelectDataValuePostgre(string postgresql, NpgsqlConnection connection)
        {
            try
            {
                DataRow r = SelectDataRowPostgre(postgresql, connection);
                return r[0].ToString();
            }
            catch
            {

                return "";
            }

        }


        protected int DataInsertPostgre(string postgresql, NpgsqlConnection connection)
        {
            int lastId = 0;
            string query = postgresql + "   RETURNING *; ";

            try
            {
                if (connection.State.ToString() == "Open")
                {
                    NpgsqlCommand cmd = new(query, connection);

                    lastId = this.ToInt(cmd.ExecuteScalar());


                }
                return this.ToInt(lastId);
            }
            catch
            {

                return 0;
            }

        }

        protected int DataInsertPostgre(string TableName, Dictionary<string, string> postgresql, NpgsqlConnection connection)
        {
            int lastId = 0;

            string field = "";
            string value = "";

            int k = 0;
            try
            {
                foreach (KeyValuePair<string, string> entry in postgresql)
                {
                    k++;
                    field += (k != 1 ? "," : "") + "" + entry.Key + "";
                    value += (k != 1 ? "," : "") + "@" + entry.Key;
                }
                string query = "INSERT INTO " + TableName + "( " + field + " ) VALUES(" + value + ")  RETURNING *; ";


                if (connection.State.ToString() == "Open")
                {

                    NpgsqlCommand cmd = new(query, connection);
                    cmd.CommandText = query;
                    foreach (KeyValuePair<string, string> entry in postgresql)
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

        protected bool DataUpdatePostgre(string postgresql, NpgsqlConnection connection)
        {
            string query = postgresql;
            try
            {

                if (connection.State.ToString() == "Open")
                {

                    NpgsqlCommand cmd = new();

                    cmd.CommandText = query;

                    cmd.Connection = connection;


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


        protected bool DataUpdatePostgre(string TableName, Dictionary<string, string> postgresql, string Condition = " ", NpgsqlConnection? connection = null)
        {
            string field = "";
            int k = 0;

            try
            {
                foreach (KeyValuePair<string, string> entry in postgresql)
                {
                    k++;
                    field += (k != 1 ? "," : "") + "" + entry.Key + " = @" + entry.Key + " ";
                }
                string query = " UPDATE " + TableName + " SET " + field + " " + Condition;


                if (connection != null && connection.State.ToString() == "Open")
                {

                    NpgsqlCommand cmd = new(query, connection);
                    cmd.CommandText = query;
                    foreach (KeyValuePair<string, string> entry in postgresql)
                    {
                        cmd.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                    }

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

        protected bool DataDeletePostgre(string postgresql, NpgsqlConnection connection)
        {
            string query = postgresql;
            try
            {
                if (connection.State.ToString() == "Open")
                {
                    NpgsqlCommand cmd = new(query, connection);
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


        protected void DataBeginPostgre(NpgsqlConnection connection)
        {
            string query = "BEGIN;";
            try
            {
                if (connection.State.ToString() == "Open")
                {
                    NpgsqlCommand cmd = new(query, connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {

            }

        }


        protected void DataCommitPostgre(NpgsqlConnection connection)
        {

            string query = "COMMIT;";
            try
            {
                if (connection.State.ToString() == "Open")
                {
                    NpgsqlCommand cmd = new(query, connection);
                    cmd.ExecuteNonQuery();

                }
            }
            catch
            {

            }


        }

        protected void DataRollBackPostgre(NpgsqlConnection connection)
        {
            string query = "ROLLBACK;";
            try
            {
                if (connection.State.ToString() == "Open")
                {
                    NpgsqlCommand cmd = new(query, connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {

            }
        }

    }
}
