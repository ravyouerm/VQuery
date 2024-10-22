using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using VQuery.Databases.PostgreSQL;

namespace VQuery
{
    public class PostgreSQLConnection : _DBPOSTGRESQL
    {
        public DataTable SelectDataTable(string postgresql)
        {
            return _SelectDataTablePostgre(postgresql);
        }

        public DataRow SelectDataRow(string postgresql)
        {
            return _SelectDataRowPostgre(postgresql);
        }

        public string SelectDataValue(string postgresql)
        {
            return _SelectDataValuePostgre(postgresql);
        }


        public int Insert(string postgresql)
        {
            return _DataInsertPostgre(postgresql);
        }

        public int Insert(string TableName, Dictionary<string, string> postgresql)
        {
            return _DataInsertPostgre(TableName, postgresql);
        }

        public bool Update(string postgresql)
        {
            return _DataUpdatePostgre(postgresql);
        }

        public bool Update(string TableName, Dictionary<string, string> postgresql, string Condition = " ")
        {
            return _DataUpdatePostgre(TableName, postgresql, Condition);
        }

        public bool Delete(string postgresql)
        {
            return _DataDeletePostgre(postgresql);
        }

        public void Begin()
        {
            _DataBeginPostgre();
        }

        public void Commit()
        {
            _DataCommitPostgre();
        }

        public void RollBack()
        {
            _DataRollBackPostgre();
        }
    }
}
