using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;
using ZenPlatform.Core.Entity;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Queries;
using ZenPlatform.QueryBuilder.Schema;

namespace ZenPlatform.DataComponent
{

    public abstract class QueryBuilderComponent
    {
        public virtual DBSelectQuery SelectSingleObject()
        {
            throw new NotImplementedException();
        }
        public virtual DBSelectQuery SelectMultiplyObjects()
        {
            throw new NotImplementedException();
        }
        public virtual DBSelectQuery UpdateSingleObject()
        {
            throw new NotImplementedException();
        }
        public virtual DBSelectQuery UpdateMultiplyObjects()
        {
            throw new NotImplementedException();
        }
        public virtual DBSelectQuery InsertObject()
        {
            throw new NotImplementedException();
        }
        public virtual DBSelectQuery DeleteSingleObject()
        {
            throw new NotImplementedException();
        }
        public virtual DBSelectQuery DeleteMultiplyObjects()
        {
            throw new NotImplementedException();
        }
        //protected PObjectType _objectType;

        //public QueryBuilderComponent(PObjectType objectType)
        //{
        //    _objectType = objectType;
        //}

        //protected DBTable MakeTable(PObjectType objectType)
        //{
        //    DBTable table = new DBTable(objectType.Name);
        //    foreach (var property in objectType.Properties)
        //    {
        //        if (property.Types.Count > 1)
        //            throw new NotSupportedException("objectType не должен содержать свойства с несколькими типами.");

        //        var typeProperty = property.Types.FirstOrDefault() as PPrimetiveType;


        //        var schema = new DBFieldSchema(typeProperty.DBType, property.Name, typeProperty.ColumnSize,
        //            typeProperty.Precision, typeProperty.Scale, false, property.Unique,
        //            false, property.Unique ? false : typeProperty.IsNullable);

        //        var field = new DBTableField(table, property.Name)
        //        {
        //            Schema = schema
        //        };

        //        table.Fields.Add(field);


        //    }

        //    return table;
        //}

        //public virtual DBSelectQuery GetSelect()
        //{
        //    var query = new DBSelectQuery();

        //    try
        //    {
        //        DBTable table = MakeTable(_objectType);

        //        var keyField = table.Fields.First(p => p.Schema.IsKey);

        //        var param = DBClause.CreateParameter(keyField.Name, DBType.UniqueIdentifier);


        //        query.From(table)
        //            .Where(keyField, CompareType.Equals, param);
        //        query.SelectAllFieldsFromSourceTables();

        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        throw new NotSupportedException("В таблице не может быть более одного уникального поля.", ex);
        //    }

        //    return query;
        //}

        //public virtual DBUpdateQuery GetUpdate()
        //{
        //    var query = new DBUpdateQuery();

        //    query.UpdateTable = MakeTable(_objectType);

        //    foreach (var field in query.UpdateTable.Fields.Where(f => !f.Schema.IsKey))
        //    {
        //        query.AddField(field as DBTableField);
        //    }


        //    var keyField = query.UpdateTable.Fields.First(p => p.Schema.IsKey);

        //    query.Where(keyField, CompareType.Equals, DBClause.CreateParameter(keyField.Name, DBType.UniqueIdentifier));

        //    return query;
        //}

        //public virtual DBDeleteQuery GetDelete()
        //{
        //    var query = new DBDeleteQuery();

        //    query.DeleteTable = MakeTable(_objectType);

        //    var keyField = query.DeleteTable.Fields.First(p => p.Schema.IsKey);

        //    query.Where(keyField, CompareType.Equals, DBClause.CreateParameter(keyField.Name, DBType.UniqueIdentifier));

        //    return query;
        //}

        //public virtual DBInsertQuery GetInsert()
        //{
        //    var query = new DBInsertQuery();

        //    query.InsertTable = MakeTable(_objectType);

        //    foreach (var field in query.InsertTable.Fields.Where(f => !f.Schema.IsKey))
        //    {
        //        query.AddField(field as DBTableField);
        //    }

        //    return query;
        //}




    }
}
