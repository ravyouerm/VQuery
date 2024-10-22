using System.Data;
using System.Data.SqlClient;
using VQuery.Databases.SQLServer;

namespace VQuery
{
    public class SQLServerConnection : _DBSQL
    {

        public int Insert(string sql)
        {
            return _DataInsertSQL(sql);
        }

        public int Insert(string TableName, Dictionary<string, string> sql)
        {
            return _DataInsertSQL(TableName, sql);
        }

        public bool Update(string sql)
        {
            return _DataUpdateSQL(sql);

        }

        public bool Update(string TableName, Dictionary<string, string> sql, string Condition = " ")
        {
            return _DataUpdateSQL(TableName, sql, Condition);
        }

        public bool Delete(string sql)
        {
            return _DataDeleteSQL(sql);
        }


        public bool DeleteRecord(string TableName, string SQLWhere, string FId = "id")
        {
            bool rebool = true;
            string sql = "SELECT " + FId + " FROM  " + TableName + "  " + SQLWhere;
            DataTable DT = this._SelectDataTableSQL(sql);
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow R in DT.Rows)
                {
                    string sqldel = "DELETE FROM " + TableName + " WHERE  " + FId + " = " + this.SQLToInt(R[FId]);
                    bool del = _DataDeleteSQL(sqldel);
                    if (!del)
                    {
                        rebool = false;
                    }
                }
            }

            return rebool;
        }

        public bool DataExecuteNonQuery(string sql)
        {
            return _DataExecuteNonQuerySQL(sql);
        }

        public bool DataExecuteTRANSACTION(List<string> SQL)
        {
            return _DataExecuteTRANSACTIONSQL(SQL);
        }


        public DataTable SelectDataTable(string sql)
        {
            return _SelectDataTableSQL(sql);
        }

        public DataTable SelectDataTable(string sql, Dictionary<string, string> Field_OVER_ORDER, int from_numrow, int to_numrow)
        {
            return _SelectDataTableSQL(sql, Field_OVER_ORDER, from_numrow, to_numrow);
        }


        public DataRow SelectDataRow(string sql)
        {
            return _SelectDataRowSQL(sql);
        }


        public string SelectDataValue(string sql)
        {
            return _SelectDataValueSQL(sql);
        }


        public void Begin(string Tran_ID = "")
        {
            _DataBeginSQL(Tran_ID);
        }

        public void Commit(string Tran_ID = "")
        {
            _DataCommitSQL(Tran_ID);
        }

        public void RollBack(string Tran_ID = "")
        {
            _DataRollBackSQL(Tran_ID);
        }

    }
}
