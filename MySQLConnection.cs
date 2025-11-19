using System.Data;
using VQuery.Databases.MySQL;

namespace VQuery
{
    public class MySQLConnection:_DBMYSQL
    {
        public DataTable SelectDataTable(string mysql)
        {
            return _SelectDataTableMysql(mysql);
        }

        public DataTable SelectDataTable(string mysql, Dictionary<string, object> ?parameters = null)
        {
            return _SelectDataTableMysql(mysql, parameters);
        }


        public DataRow SelectDataRow(string mysql)
        {
            return _SelectDataRowMysql(mysql);
        }

        public DataRow SelectDataRow(string mysql, Dictionary<string, object> ?parameters = null)
        {
            return _SelectDataRowMysql(mysql, parameters);
        }

        public string SelectDataValue(string mysql)
        {
            return _SelectDataValueMysql(mysql);
        }

        public string SelectDataValue(string mysql, Dictionary<string, object> ?parameters = null)
        {
            return _SelectDataValueMysql(mysql, parameters);
        }


        public int Insert(string mysql)
        {
            return _DataInsertMysql(mysql);
        }

        public int Insert(string TableName, Dictionary<string, string> mysql)
        {
            return _DataInsertMysql(TableName, mysql);
        }

        public bool Update(string mysql)
        {
            return _DataUpdateMysql(mysql);
        }

        public bool Update(string TableName, Dictionary<string, string> mysql, string Condition = " ")
        {
            return _DataUpdateMysql(TableName, mysql, Condition);
        }

        public bool Update(string TableName, Dictionary<string, string> mysql, Dictionary<string, object> ?parameters = null)
        {
            return _DataUpdateMysql(TableName, mysql, parameters);
        }


        public bool Update(string mysql, Dictionary<string, object> ?parameters = null)
        {
            return _DataUpdateMysql(mysql,parameters);
        }

        public bool Delete(string mysql)
        {
            return _DataDeleteMysql(mysql);
        }

        public bool Delete(string mysql, Dictionary<string, object> ?parameters = null)
        {
            return _DataDeleteMysql(mysql, parameters);
        }


        public bool Execute(string mysql)
        {
            return _DataExecuteMysql(mysql);
        }

        public bool Execute(string mysql, Dictionary<string, object>? parameters = null)
        {
            return _DataExecuteMysql(mysql, parameters);
        }


        public bool ExecuteRun(string mysql, Dictionary<string, object>? parameters = null)
        {
            return _DataExecuteMysqlReturn(mysql, parameters);
        }


        

        public void Begin()
        {
            _DataBeginMysql();
        }

        public void Commit()
        {
            _DataCommitMysql();
        }

        public void RollBack()
        {
            _DataRollBackMysql();
        }

    }
}
