using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using ZenPlatform.QueryBuilder.Interfaces;
using ZenPlatform.QueryBuilder.Schema;

namespace ZenPlatform.QueryBuilder
{
    /// <summary>
    /// Represents any accepted token in query
    /// SelectExpression, ParameterExpression, WhereExpression, GroupByExpression etc...
    /// </summary>
    public abstract class DBClause : IDBToken
    {
        private List<DBCompileTransformation> _transformations;

        protected DBClause()
        {
            _transformations = new List<DBCompileTransformation>();
        }

        public virtual object Clone()
        {
            var clone = this.MemberwiseClone() as DBClause;
            clone.ClearTransformations();

            return clone;
        }

        public virtual string Compile(bool recompile = false)
        {
            throw new NotImplementedException();
        }

        private void ClearTransformations()
        {
            _transformations.Clear();
        }

        protected List<DBCompileTransformation> Transformations => _transformations;

        public virtual void AddTransormation(DBCompileTransformation transformation)
        {
            _transformations.Add(transformation);
        }

        public virtual string CompileExpression { get; set; }


        public static DBClause CreateTableField(DBTable owner, string name)
        {
            return new DBTableField(owner, name);
        }

        public static DBClause CreateFieldClause(IDBAliasedFieldContainer owner, string name)
        {
            return new DBClauseField(owner, name);
        }

        public static DBClause CreateSelectField(IDBTableDataSource owner, string name, string alias = "")
        {
            return new DBSelectField(owner, name, alias);
        }

        public static DBClause CreateSelectField(IDBTableDataSource owner, DBFieldSchema schema, string name, string alias = "")
        {
            return new DBSelectField(owner, name, schema, alias);
        }

        public static DBClause CreateSelectField(IDBTableDataSource owner, object obj, string alias = "")
        {
            string quotedObject = "'" + obj + "'";
            return new DBSelectField(owner, quotedObject, alias);
        }

        public static DBParameter CreateParameter(string name)
        {
            return new DBParameter(name);
        }

        public static DBParameter CreateParameter(string name, DBType type)
        {
            return new DBParameter(name, type);
        }

        public static DBClause CreateParameter(string name, object value)
        {
            return new DBParameter(name, value);
        }

        public static DBClause CreateConstant(object value)
        {
            if (value is IList)
                return new DBArrayConstant(value);
            return new DBConstant(value);
        }
    }
}