using System;
using Aquila.QueryBuilder.Common.Table;
using Aquila.QueryBuilder.DML.From;
using Aquila.QueryBuilder.DML.Functions;
using Aquila.QueryBuilder.DML.Select;
using Aquila.QueryBuilder.DML.Where;

namespace Aquila.QueryBuilder.Common.Factoryes
{
    /// <summary>
    /// Фабрика общих типов, таких как поля, таблицы, литералы, параметры
    /// </summary>
    public class SqlNodeFactory
    {
        private static SqlNodeFactory _instance = new SqlNodeFactory();

        public static SqlNodeFactory Get() => _instance;

        public ColumnNode Field(string name)
        {
            return new ColumnNode(name);
        }

        public ColumnNode Field(string tableName, string name)
        {
            return (new ColumnNode(name)).WithParent(tableName);
        }

        public AliasedTableNode Table(string tableName)
        {
            return new AliasedTableNode(tableName);
        }

        public TableWithColumnsNode InsertTable(string tableName)
        {
            return new TableWithColumnsNode(tableName);
        }

        public AliasedTableNode Table(string schemaName, string tableName)
        {
            return new AliasedTableNode(tableName).WithSchema(schemaName);
        }

        public ParameterNode Parameter(string name)
        {
            return new ParameterNode(name);
        }

        public StringLiteralNode String(string str)
        {
            return new StringLiteralNode(str);
        }

        public RawSqlNode Raw(string raw)
        {
            return new RawSqlNode(raw);
        }
    }

    /// <summary>
    /// Фабрика функций SQL
    /// </summary>
    public class SqlFunctionsFactory
    {
        public SumFunctionNode Sum(Func<SqlNodeFactory, SqlNode> expression)
        {
            return new SumFunctionNode(expression(new SqlNodeFactory()));
        }

        public SumFunctionNode Sum(string fieldName)
        {
            return Sum(x => x.Field(fieldName));
        }
    }
}