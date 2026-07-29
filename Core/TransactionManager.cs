using System;
using System.Collections.Generic;
using System.Linq;
using VQuery;

namespace VQuery.Extensions
{
    public sealed class TransactionManager
    {
        private readonly MySQLConnection _db;

        public TransactionManager(
            MySQLConnection db)
        {
            _db = db;
        }

        public void Execute(
            Action action)
        {
            try
            {
                _db.Begin();

                action();

                _db.Commit();
            }
            catch
            {
                _db.RollBack();
                throw;
            }
        }
    }

}