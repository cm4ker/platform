using System;

namespace QueryCompiler.Queries
{
    public class DBCreateDatabase : IQueryable
    {
        private readonly string _databaseName;

        public DBCreateDatabase(string databaseName)
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