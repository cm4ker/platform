using System;
using System.Collections.Generic;
using System.Data;

namespace ZenPlatform.QueryBuilder.Schema
{
    //public class DBSchemaManager
    //{
    //    private DBContext _context;

    //    private Dictionary<string, List<DBFieldSchema>> _schemas;

    //    public DBSchemaManager(DBContext context)
    //    {
    //        _context = context;
    //        if (!DBGlobalContext.Insatnce.Exists("schemaCache"))
    //            DBGlobalContext.Insatnce["schemaCache"] = new Dictionary<string, List<DBFieldSchema>>();
    //        _schemas = DBGlobalContext.Insatnce["schemaCache"] as Dictionary<string, List<DBFieldSchema>>;
    //    }


    //    public List<DBFieldSchema> GetTableSchema(DBTable table)
    //    {
    //        return GetTableSchema(table.Name);
    //    }

    //    public List<DBFieldSchema> GetTableSchema(string tableName)
    //    {

    //        var result = new List<DBFieldSchema>();

    //        if (_schemas.TryGetValue(tableName, out var cached))
    //        {
    //            return cached;
    //        }

    //        using (var cmd = _context.CreateCommand())
    //        {
    //            cmd.CommandText = $"SELECT * FROM {tableName} WHERE 1=0";

    //            using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
    //            {
    //                result.AddRange(GetFieldsSchema(reader));
    //            }
    //        }

    //        _schemas[tableName] = result;
    //        return result;
    //    }

    //    public List<DBFieldSchema> GetFieldsSchema(SqlDataReader reader)
    //    {
    //        List<DBFieldSchema> schemas = new List<DBFieldSchema>();
    //        var tableSchema = reader.GetSchemaTable();
    //        var nameColumn = tableSchema.Columns["ColumnName"];
    //        var keyColumn = tableSchema.Columns["IsKey"];
    //        var uniqueColumn = tableSchema.Columns["IsUnique"];
    //        var identityColumn = tableSchema.Columns["IsIdentity"];
    //        var typeColumn = tableSchema.Columns["DataType"];
    //        var isNullableColumn = tableSchema.Columns["AllowDBNull"];
    //        var columnSizeColumn = tableSchema.Columns["ColumnSize"];
    //        var numericPrecisionColumn = tableSchema.Columns["NumericPrecision"];
    //        var numericScaleColumn = tableSchema.Columns["NumericScale"];
    //        foreach (DataRow row in tableSchema.Rows)
    //        {
    //            var fieldSchema =
    //                new DBFieldSchema(
    //                    DBHelper.GetSqlType((Type)row[typeColumn]),
    //                    (string)row[nameColumn],
    //                    (int)row[columnSizeColumn],
    //                    (short)row[numericPrecisionColumn],
    //                    (short)row[numericScaleColumn],
    //                    (bool)row[identityColumn],
    //                    (bool)row[keyColumn],
    //                    (bool)row[uniqueColumn],
    //                    (bool)row[isNullableColumn]);
    //            schemas.Add(fieldSchema);
    //        }
    //        return schemas;
    //    }

    //}
}
