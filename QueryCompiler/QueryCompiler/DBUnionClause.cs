﻿using System;
using QueryCompiler.Interfaces;
using IQueryable = QueryCompiler.Queries.IQueryable;

namespace QueryCompiler
{
    public class DBUnionClause : IDBToken
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            return SQLTokens.UNION;
        }

        public string CompileExpression { get; set; }
    }
    public class DBUnionAllClause : IDBToken
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            return $"{SQLTokens.UNION} {SQLTokens.ALL}";
        }

        public string CompileExpression { get; set; }
    }

    public enum DBSelectConjunctionTypes
    {
        None,
        Union,
        UnionAll,
        Intersect,
        Except
    }
}

