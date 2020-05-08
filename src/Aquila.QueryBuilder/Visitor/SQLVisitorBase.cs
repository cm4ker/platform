using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder.Visitor
{
    public class SQLVisitorBase : QueryVisitorBase<string>
    {
        #region DML

        public override string VisitSAdd(SAdd node)
        {
            return string.Join(" + ", node.Expressions.Select(n => n.Accept(this)));
        }

        public override string VisitSAliasedDataSource(SAliasedDataSource node)
        {
            return string.Format("{0} as {1}",
                node.DataSource.Accept(this),
                node.Name);
        }

        public override string VisitSAliasedExpression(SAliasedExpression node)
        {
            return string.Format("{0} as {1}",
                node.Expression.Accept(this),
                node.Name);
        }

        public override string VisitSAnd(SAnd node)
        {
            return string.Join(" and ", node.Expressions.Select(n => n.Accept(this)));
        }

        public override string VisitSAvg(SAvg node)
        {
            return string.Format("avg({0})", node.Argument.Accept(this));
        }

        public override string VisitSCoalese(SCoalese node)
        {
            return string.Format("coalease({0})", string.Join(", ", node.Expressions.Select(e => e.Accept(this))));
        }

        public override string VisitSConstant(SConstant node)
        {
            return node.Value switch
            {
                string str => string.Format("'{0}'", str),
                bool b => Convert.ToByte(b).ToString(),
                Guid g => string.Format("'{0}'", g.ToString()),
                _ => node.Value.ToString()
            };
        }

        public override string VisitSCount(SCount node)
        {
            return string.Format("count({0})", node.Argument.Accept(this));
        }

        public override string VisitSDataSourceNestedQuery(SDataSourceNestedQuery node)
        {
            return $"({node.Query.Accept(this)})";
        }

        public override string VisitSEquals(SEquals node)
        {
            return string.Format("{0} = {1}", node.Left.Accept(this), node.Right.Accept(this));
        }

        public override string VisitSField(SField node)
        {
            return string.Format("{0}{1}",
                string.IsNullOrEmpty(node.Table) ? "" : node.Table + ".",
                node.Name
            );
        }

        public override string VisitSCast(SCast node)
        {
            return string.Format("CAST({0} AS {1})", Visit(node.Expression), Visit(node.Type));
        }

        public override string VisitSFrom(SFrom node)
        {
            return string.Format("FROM\n{0}{1}{2}\n",
                node.DataSource.Accept(this),
                node.Join.Count > 0 ? "\n" : "",
                string.Join("\n", node.Join.Select(j => j.Accept(this))));
        }

        public override string VisitSGreatThen(SGreatThen node)
        {
            return string.Format("{0} > {1}", node.Left.Accept(this), node.Right.Accept(this));
        }

        public override string VisitSGreatThenOrEquals(SGreatThenOrEquals node)
        {
            return string.Format("{0} >= {1}", node.Left.Accept(this), node.Right.Accept(this));
        }

        public override string VisitSGroupBy(SGroupBy node)
        {
            return string.Format("GROUP BY\n{0}\n",
                string.Join(", ", node.Fields.Select(f => f.Accept(this))));
        }

        public override string VisitSHaving(SHaving node)
        {
            return string.Format("HAVING\n{0}\n",
                node.Condition.Accept(this));
        }

        public override string VisitSInsert(SInsert node)
        {
            return string.Format("INSERT INTO {0}{1}\n{2}",
                node.Into.Accept(this),
                node.Fields.Count > 0 ? $"({string.Join(", ", node.Fields.Select(f => f.Accept(this)))})" : "",
                node.DataSource.Accept(this)
            );
        }

        public override string VisitSJoin(SJoin node)
        {
            return string.Format("JOIN {0} ON {1}",
                node.DataSource.Accept(this),
                node.Condition.Accept(this)
            );
        }


        public override string VisitSLessThen(SLessThen node)
        {
            return string.Format("({0} < {1})", node.Left.Accept(this), node.Right.Accept(this));
        }

        public override string VisitSLessThenOrEquals(SLessThenOrEquals node)
        {
            return string.Format("({0} <= {1})", node.Left.Accept(this), node.Right.Accept(this));
        }

        public override string VisitSNotEquals(SNotEquals node)
        {
            return string.Format("({0} <> {1})", node.Left.Accept(this), node.Right.Accept(this));
        }

        public override string VisitSOr(SOr node)
        {
            return string.Format("({0})",
                string.Join(" OR ", node.Expressions.Select(e => e.Accept(this)))
            );
        }

        public override string VisitSOrderBy(SOrderBy node)
        {
            return string.Format("ORDER BY {0}{1}\n",
                string.Join(", ", node.Fields.Select(f => f.Accept(this))),
                node.Direction == OrderDirection.DESC ? " DESC" : ""
            );
        }

        public override string VisitSParameter(SParameter node)
        {
            return string.Format("@{0}", node.Name);
        }

        public override string VisitSSelect(SSelect node)
        {
            return string.Format("SELECT {6}{0}\n{1}{2}{3}{4}{5}",
                string.Join(",\n", node.Fields.Select(f => f.Accept(this))),
                node.From == null ? "" : node.From.Accept(this),
                node.Where == null ? "" : node.Where.Accept(this),
                node.GroupBy == null ? "" : node.GroupBy.Accept(this),
                node.Having == null ? "" : node.Having.Accept(this),
                node.OrderBy == null ? "" : node.OrderBy.Accept(this),
                node.Top == null ? "" : node.OrderBy.Accept(this)
            );
        }

        public override string VisitSDelete(SDelete node)
        {
            return string.Format("DELETE\n{0}{1}",
                node.From == null ? "" : node.From.Accept(this),
                node.Where == null ? "" : node.Where.Accept(this)
            );
        }

        public override string VisitSAssign(SAssign node)
        {
            return string.Format("{0} = {1}",
                node.Variable.Accept(this),
                node.Expression.Accept(this)
            );
        }

        public override string VisitSSub(SSub node)
        {
            return string.Format("({0})",
                string.Join(" - ", node.Expressions.Select(e => e.Accept(this))));
        }

        public override string VisitSSum(SSum node)
        {
            return string.Format("sum({0})", node.Argument.Accept(this));
        }

        public override string VisitSTable(STable node)
        {
            return string.Format("{0}", node.Name);
        }

        public override string VisitSUpdate(SUpdate node)
        {
            return string.Format("UPDATE {0}\nSET {1}\n{2}{3}",
                node.Update.Accept(this),
                string.Join(", ", node.Set.Items.Select(s => s.Accept(this))),
                node.From == null ? "" : node.From.Accept(this),
                node.Where == null ? "" : node.Where.Accept(this)
            );
        }

        public override string VisitSValuesSource(SValuesSource node)
        {
            return string.Format("VALUES\n({0})\n",
                string.Join(", ", node.Values.Select(s => s.Accept(this)))
            );
        }

        public override string VisitSWhere(SWhere node)
        {
            return string.Format("WHERE\n{0}\n",
                node.Condition.Accept(this)
            );
        }

        public override string VisitSCase(SCase node)
        {
            return string.Format("CASE {0} \n {1} END",
                string.Join("\n", node.Whens.Select(w => w.Accept(this))),
                node.Else != null ? $"ELSE {node.Else.Accept(this)}\n" : ""
            );
        }

        public override string VisitSWhen(SWhen node)
        {
            return string.Format("WHEN  {0} THEN {1}",
                node.Condition.Accept(this),
                node.Then.Accept(this)
            );
        }

        #endregion

        public override string VisitQuerys(Querys node)
        {
            return string.Join(";\n", node.QueryList.Select(q => q.Accept(this)));
        }

        #region DDL

        public override string VisitAddColumn(AddColumn node)
        {
            return string.Format("ALTER TABLE {0}\n ADD {1}",
                node.Table.Accept(this),
                node.Column.Accept(this)
            );
        }

        public override string VisitCopyTable(CopyTable node)
        {
            return string.Format("SELECT * INTO {0} FROM {1}",
                node.DstTable.Accept(this),
                node.Table.Accept(this)
            );
        }

        public override string VisitAddConstraint(AddConstraint node)
        {
            return string.Format("ALTER TABLE {0}\n ADD {1}",
                node.Table.Accept(this),
                node.Constraint.Accept(this)
            );
        }

        public override string VisitAlterColumn(AlterColumn node)
        {
            return string.Format("ALTER TABLE {0}\n ALTER COLUMN {1}",
                node.Table.Accept(this),
                node.Column.Accept(this)
            );
        }

        public override string VisitColumn(Column node)
        {
            return node.Value;
        }

        public override string VisitColumnDefinition(ColumnDefinition node)
        {
            return string.Format("{0} {1}{2}{3}",
                node.Column.Accept(this),
                node.Type.Accept(this),
                node.IsNotNull ? " NOT NULL" : "",
                node.DefaultValue != null ? $" DEFAULT {node.DefaultValue}" : ""
            );
        }

        public override string VisitColumnTypeInt(ColumnTypeInt node)
        {
            return "INT";
        }

        public override string VisitColumnTypeVarChar(ColumnTypeVarChar node)
        {
            return $"VARCHAR({((node.Size == 0) ? "MAX" : node.Size.ToString())})";
        }

        public override string VisitColumnTypeBigInt(ColumnTypeBigInt node)
        {
            return "BIGINT";
        }

        public override string VisitColumnTypeSmallInt(ColumnTypeSmallInt node)
        {
            return "SMALLINT";
        }

        public override string VisitColumnTypeBlob(ColumnTypeBlob node)
        {
            return $"BLOB({node.Size})";
        }

        public override string VisitColumnTypeBool(ColumnTypeBool node)
        {
            return "BIT";
        }

        public override string VisitColumnTypeChar(ColumnTypeChar node)
        {
            return "CHAR";
        }

        public override string VisitColumnTypeDataTime(ColumnTypeDataTime node)
        {
            return "DATETIME";
        }

        public override string VisitColumnTypeDecimal(ColumnTypeDecimal node)
        {
            return $"DECIMAL({node.Precision},{node.Scale})";
        }


        public override string VisitColumnTypeFloat(ColumnTypeFloat node)
        {
            return "FLOAT";
        }

        public override string VisitColumnTypeNumeric(ColumnTypeNumeric node)
        {
            return $"NUMERIC({node.Precision},{node.Scale})";
        }

        public override string VisitColumnTypeGuid(ColumnTypeGuid node)
        {
            return "UNIQUEIDENTIFIER";
        }

        public override string VisitColumnTypeText(ColumnTypeText node)
        {
            return string.Format("VARCHAR{0}", node.Size > 0 ? $"({node.Size})" : "");
        }

        public override string VisitColumnTypeBinary(ColumnTypeBinary node)
        {
            return string.Format("BINARY{0}", node.Size > 0 ? $"({node.Size})" : "");
        }

        public override string VisitColumnTypeVarBinary(ColumnTypeVarBinary node)
        {
            return string.Format("VARBINARY{0}", node.Size > 0 ? $"({node.Size})" : "(MAX)");
        }

        public override string VisitConstraint(Constraint node)
        {
            return node.Value;
        }

        public override string VisitSNull(SNull node)
        {
            return "NULL";
        }

        public override string VisitCreateTable(CreateTable node)
        {
            var ct = string.Format("CREATE TABLE {0} \n(\n{1}{2}{3}\n)",
                node.Table.Accept(this),
                string.Join(",\n", node.Columns.Select(c => c.Accept(this))),
                node.Constraints.Count > 0 ? ",\n" : "",
                string.Join(",\n", node.Constraints.Select(c => c.Accept(this)))
            );

            if (node.CheckExists)
                ct = $"IF (OBJECT_ID('{node.Table.Accept(this)}', N'U') IS NULL) \n" + ct;
            
            return ct;
        }

        public override string VisitConstraintDefinitionForeignKey(ConstraintDefinitionForeignKey node)
        {
            return string.Format("{0}FOREIGN KEY ({1}) REFERENCES {2}({3})",
                string.IsNullOrEmpty(node.Name) ? "" : $"CONSTRAINT {node.Name} ",
                string.Join(",", node.Columns.Select(c => c.Accept(this))),
                node.ForeignTable.Accept(this),
                string.Join(",", node.ForeignColumns.Select(c => c.Accept(this)))
            );
        }

        public override string VisitConstraintDefinitionPrimaryKey(ConstraintDefinitionPrimaryKey node)
        {
            return string.Format("{0}PRIMARY KEY ({1})",
                string.IsNullOrEmpty(node.Name) ? "" : $"CONSTRAINT {node.Name} ",
                string.Join(",", node.Columns.Select(c => c.Accept(this)))
            );
        }

        public override string VisitConstraintDefinitionUnique(ConstraintDefinitionUnique node)
        {
            return string.Format("{0}UNIQUE ({1})",
                string.IsNullOrEmpty(node.Name) ? "" : $"CONSTRAINT {node.Name} ",
                string.Join(",", node.Columns.Select(c => c.Accept(this)))
            );
        }

        public override string VisitDropColumn(DropColumn node)
        {
            return string.Format("ALTER TABLE {0} DROP COLUMN {1}",
                node.Table.Accept(this),
                node.Column.Accept(this)
            );
        }

        public override string VisitDropConstraint(DropConstraint node)
        {
            return string.Format("ALTER TABLE {0} DROP CONSTRAINT {1}",
                node.Table.Accept(this),
                node.Constraint.Accept(this)
            );
        }

        public override string VisitTable(Table node)
        {
            return node.Value;
        }

        public override string VisitDropTable(DropTable node)
        {
            return string.Format("{0}DROP TABLE {1}",
                node.IfExists ? string.Format("IF OBJECT_ID('{0}', 'U') IS NOT NULL\n", node.Table.Accept(this)) : "",
                node.Table.Accept(this));
        }

        public override string VisitRenameTableNode(RenameTableNode node)
        {
            return string.Format("EXEC sp_rename '{0}', '{1}'", node.From.Accept(this), node.To.Accept(this));
        }

        public override string VisitRenameColumnNode(RenameColumnNode node)
        {
            return string.Format("EXEC sp_rename '{0}.{1}', '{2}', 'COLUMN'", node.Table.Accept(this),
                node.From.Accept(this), node.To.Accept(this));
        }

        #endregion
    }
}