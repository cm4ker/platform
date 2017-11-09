﻿using System;
using System.Text;

namespace ZenPlatform.QueryBuilder.Queries
{
    public class DBDropTableQuery : IQueryable
    {
        private string _tableName;

        internal DBDropTableQuery()
        {

        }

        public void Table(string tableName)
        {
            _tableName = tableName;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} {2}", SQLTokens.DROP, SQLTokens.TABLE, _tableName);
            return sb.ToString();
        }

        public string CompileExpression { get; set; }
    }
}
