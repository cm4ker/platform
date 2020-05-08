using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.QueryBuilder.Queries
{
    public class DBMultiQuery : IQueryable
    {
        public string CompileExpression { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private List<IQueryable> _queryList;

        public DBMultiQuery()
        {
            List<IQueryable> _queryList = new List<IQueryable>();
        }

        public void AddQuery(IQueryable query)
        {
            _queryList.Add(query);
        }


        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            foreach (var query in _queryList)
            {
                sb.AppendFormat("{0};\n", query.Compile(recompile));
            }

            return sb.ToString();
        }
    }
}
