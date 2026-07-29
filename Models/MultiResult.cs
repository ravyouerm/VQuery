using System.Data;
using VQuery.Core;

namespace VQuery.Models
{
    public class MultiResult
    {
        private readonly List<DataTable> _tables = new();

        private int _index;

        internal void AddTable(DataTable table)
        {
            _tables.Add(table);
        }

        internal int Count
        {
            get
            {
                return _tables.Count;
            }
        }

        public List<T> Read<T>()
            where T : new()
        {
            if (_index >= _tables.Count)
                return new();

            DataTable table =
                _tables[_index++];

            return table.AsEnumerable()
                .Select(QueryMapper.MapRow<T>)
                .ToList();
        }
    }
}