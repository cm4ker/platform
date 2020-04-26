using System;
using System.Collections.Generic;
using ZenPlatform.QueryBuilder.Interfaces;
using ZenPlatform.QueryBuilder.Queries;

namespace ZenPlatform.QueryBuilder
{
    public class DBSubSelectQuery : IDBDataSource, IDataReturnQuery
    {
        private readonly IDBFieldContainer _query;


        private string _compileExpression;
        private string _alias;
        private readonly List<DBField> _fields;

        public DBSubSelectQuery(IDBFieldContainer query, string alias)
        {
            _query = query;
            _fields = new List<DBField>();
            SetAliase(alias);
            _compileExpression = "({Query})";

          //  UpdateFields();
        }
        /*
        private void UpdateFields()
        {
            foreach (var field in _query.Fields)
            {
                if (field is DBSelectField)
                {

                    var dbField = field as DBSelectField;

                    DBSelectField newField =
                        DBClause.CreateSelectField(this, dbField.Schema, dbField.Name) as DBSelectField;

                    if (!string.IsNullOrEmpty(dbField.Alias))
                        newField = DBClause.CreateSelectField(this, dbField.Schema, dbField.Alias) as DBSelectField;

                    _fields.Add(newField);
                }
                else if (field is DBField)
                {
                    var dbField = field as DBField;

                    DBSelectField newField = dbField.ToSelectField(this);

                    _fields.Add(newField);
                }
            }
        }
        */
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            if (string.IsNullOrEmpty(_alias)) throw new Exception("The Subquery must have alias.");
            return $"({_query.Compile()}) {SQLTokens.AS} {_alias}";
            //return StandartCompilers.CompileAliasedObject(_query.Compile(), _alias);
        }

        public string CompileExpression
        {
            get { return _compileExpression; }
            set { _compileExpression = value; }
        }

        public void SetAliase(string alias)
        {
            _alias = alias;
            //foreach (DBSelectField field in _fields)
           // {
           //     field.SetAliase($"{field.Owner.Alias}.{field.Name}");
            //}
        }

        public string Alias
        {
            get { return _alias; }
        }

        public List<DBField> Fields

        {
            get { return _fields; }
        }

        public DBField GetField(string name)
        {
            foreach (var field in _fields)
            {
                if (field is DBSelectField)
                {
                    if ((field as DBSelectField).Name.ToLower() == name.ToLower()) return field;
                }
                else if (field is DBSelectConstantField)
                {
                    if ((field as DBSelectConstantField).Alias.ToLower() == name.ToLower()) return field;
                }
            }
            throw new FieldNotFoundException(name);
        }

        public DBParameterCollection Parameters
        {
            get
            {
                if (_query is IParametrized)
                    return new DBParameterCollection((_query as IParametrized).Parameters);
                return new DBParameterCollection();
            }
        }
    }
}