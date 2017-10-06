using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace QueryCompiler.Schema
{
    public class DBSchemaManager
    {
        private DBContext _context;

        private Dictionary<string, List<DBFieldSchema>> _schemas;

        public DBSchemaManager(DBContext context)
        {
            _context = context;
            if (!DBGlobalContext.Insatnce.Exists("schemaCache"))
                DBGlobalContext.Insatnce["schemaCache"] = new Dictionary<string, List<DBFieldSchema>>();
            _schemas = DBGlobalContext.Insatnce["schemaCache"] as Dictionary<string, List<DBFieldSchema>>;
        }


        public List<DBFieldSchema> GetTableSchema(DBTable table)
        {
            var result = new List<DBFieldSchema>();

            if (_schemas.TryGetValue(table.Name, out var cached))
            {
                return cached;
            }

            using (var cmd = _context.CreateCommand($"SELECT * FROM {table.Name} WHERE 1=0"))
            {

                //SqlDataAdapter da = new SqlDataAdapter(cmd);
                //DataSet ds = new DataSet();
                //da.FillSchema(ds, SchemaType.Source, table.Name);
                //da.Fill(ds, table.Name);


                //DataTable dt = ds.Tables[0];
                //for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
                //{
                //    //Console.WriteLine("{0}: AllowDBNull={1}", dt.Columns[colIndex].ColumnName, dt.Columns[colIndex].AllowDBNull.ToString());

                //    var fieldSchema = new DBFieldSchema(GetSqlType(dt.Columns[colIndex].DataType), dt.Columns[colIndex].ColumnName, dt.PrimaryKey.Contains(dt.Columns[colIndex]), dt.Columns[colIndex].Unique, dt.Columns[colIndex].AllowDBNull);
                //    result.Add(fieldSchema);
                //}

                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var tableSchema = reader.GetSchemaTable();
                    var nameColumn = tableSchema.Columns["ColumnName"];
                    var keyColumn = tableSchema.Columns["IsKey"];
                    var uniqueColumn = tableSchema.Columns["IsUnique"];
                    var typeColumn = tableSchema.Columns["DataType"];
                    var isNullableColumn = tableSchema.Columns["AllowDBNull"];

                    foreach (DataRow row in tableSchema.Rows)
                    {
                        var fieldSchema = new DBFieldSchema(GetSqlType((Type)row[typeColumn]),
                            (string)row[nameColumn], (bool)row[keyColumn], (bool)row[uniqueColumn], (bool)row[isNullableColumn]);
                        result.Add(fieldSchema);
                    }
                }
            }

            _schemas[table.Name] = result;
            return result;
        }

        public List<DBFieldSchema> GetTableSchema(string tableName)
        {

            var result = new List<DBFieldSchema>();

            if (_schemas.TryGetValue(tableName, out var cached))
            {
                return cached;
            }

            using (var cmd = _context.CreateCommand())
            {
                cmd.CommandText = $"SELECT * FROM {tableName} WHERE 1=0";

                //SqlDataAdapter da = new SqlDataAdapter(cmd);
                //DataSet ds = new DataSet();
                //da.FillSchema(ds, SchemaType.Source, tableName);
                //da.Fill(ds, tableName);


                //DataTable dt = ds.Tables[0];
                //for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
                //{
                //    var fieldSchema = new DBFieldSchema(GetSqlType(dt.Columns[colIndex].DataType), dt.Columns[colIndex].ColumnName, dt.PrimaryKey.Contains(dt.Columns[colIndex]), dt.Columns[colIndex].Unique, dt.Columns[colIndex].AllowDBNull);
                //    result.Add(fieldSchema);
                //}

                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    var tableSchema = reader.GetSchemaTable();
                    var nameColumn = tableSchema.Columns["ColumnName"];
                    var keyColumn = tableSchema.Columns["IsKey"];
                    var uniqueColumn = tableSchema.Columns["IsUnique"];
                    var typeColumn = tableSchema.Columns["DataType"];
                    var isNullableColumn = tableSchema.Columns["AllowDBNull"];

                    foreach (DataRow row in tableSchema.Rows)
                    {
                        var fieldSchema = new DBFieldSchema(GetSqlType((Type)row[typeColumn]),
                            (string)row[nameColumn], (bool)row[keyColumn], (bool)row[uniqueColumn], (bool)row[isNullableColumn]);
                        result.Add(fieldSchema);
                    }
                }
            }

            _schemas[tableName] = result;
            return result;
        }

        public DBFieldSchema GetFieldSchema(DBTableField field)
        {
            var cmd = _context.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {(field.Owner as DBTable).Name} WHERE 1=0";
            using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
            {
                var tableSchema = reader.GetSchemaTable();
                var nameColumn = tableSchema.Columns["ColumnName"];
                var keyColumn = tableSchema.Columns["IsKey"];
                var uniqueColumn = tableSchema.Columns["IsUnique"];
                var typeColumn = tableSchema.Columns["DataType"];
                var isNullableColumn = tableSchema.Columns["AllowDBNull"];


                foreach (DataRow row in tableSchema.Rows)
                {
                    if (row[nameColumn].ToString().ToLower() == field.Name.ToLower())
                    {
                        var fieldSchema = new DBFieldSchema(GetSqlType((Type)row[typeColumn]), (string)row[nameColumn], (bool)row[keyColumn], (bool)row[uniqueColumn], (bool)row[isNullableColumn]);
                        return fieldSchema;
                    }
                }

            }

            return null;
        }

        /// <summary>
        /// Get the equivalent SQL data type of the given type.
        /// </summary>
        /// <param name="type">Type to get the SQL type equivalent of</param>
        private SqlDbType GetSqlType(Type type)
        {
            if (type == typeof(string))
                return SqlDbType.NVarChar;

            if (type == typeof(byte[]))
                return SqlDbType.VarBinary;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            var param = new SqlParameter("", Activator.CreateInstance(type));
            return param.SqlDbType;
        }
    }
}
