using System;

namespace ZenPlatform.QueryBuilder.Queries
{
    public class DBCreateDatabaseQuery : IQueryable
    {
        private readonly string _databaseName;

        

        public DBCreateDatabaseQuery(string databaseName)
        {
            _databaseName = databaseName;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            return $"CREATE DATABASE [{_databaseName}]";
        }

        public string CompileExpression { get; set; }
    }
}