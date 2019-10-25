using System;
using System.Linq;
using System.Text;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Visitor
{
    public class SQLQueryVisitor : QueryVisitorBase<string>
    {

        public SQLQueryVisitor()
        {
          
        }

        public override string DefaultVisit(QuerySyntaxNode node)
        {
            return "";
        }

        public override string VisitAddColumn(AddColumn node)
        {
            return string.Format("ALTER TABLE {0}\n ADD COLUMN {1}",
                node.Table.Accept(this),
                node.Column.Accept(this)
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

        public override string VisitConstraint(Constraint node)
        {
            return node.Value;
        }


        public override string VisitCreateTable(CreateTable node)
        {

            return string.Format("CREATE TABLE {0} \n(\n{1}{2}{3}\n)",
                node.Table.Accept(this),
                string.Join(",\n", node.Columns.Select(c => c.Accept(this))),
                node.Constraints.Count > 0 ? ",\n" : "",
                string.Join(",\n", node.Constraints.Select(c => c.Accept(this)))
                );
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

        public override string VisitExpression(Expression node)
        {

            return string.Join(";\n", node.Nodes.Select(n => n.Accept(this)));
        }

        public override string VisitTable(Table node)
        {
            return node.Value;
        }

        public override string VisitCopyTable(CopyTable node)
        {

            return string.Format("SELECT * INTO {0} FROM {1}", node.DstTable.Accept(this), node.Table.Accept(this));
        }

        public override string VisitTableSourceNode(TableSourceNode node)
        {
            return node.Table.Accept(this);
        }

        public override string VisitFromNode(FromNode node)
        {
            return string.Format("FROM\n{0}\n{1}\n",
                    node.DataSource.Accept(this),
                    string.Join("\n", node.Join.Select(j => j.Accept(this)))
                );
        }

        public override string VisitConditionEqualNode(ConditionEqualNode node)
        {
            return string.Format("({0} = {1})",
                    node.Left.Accept(this),
                    node.Reight.Accept(this)
                );
        }

        public override string VisitJoinNode(JoinNode node)
        {
            return string.Format("JOIN {0} ON({1})\n",
                    node.DataSource.Accept(this),
                    node.Condition.Accept(this)
                );
        }

        public override string VisitWhereNode(WhereNode node)
        {
            return string.Format("WHERE\n{0}\n",
                    node.Condition.Accept(this)
                );
        }

        public override string VisitConstNode(ConstNode node)
        {
            return node.Value.ToString();
        }

        public override string VisitTableFieldNode(TableFieldNode node)
        {
            return string.Format("{0}{1}",
                    node.Table == null ? "" : $"{node.Table.Accept(this)}.",
                    node.Field
                );
        }

        public override string VisitSelectNode(SelectNode node)
        {
            return string.Format("SELECT {0}\n{1}{2}",
                string.Join(",\n", node.Fields.Select(f => f.Accept(this))),
                node.From == null ? "" : node.From.Accept(this),
                node.Where == null ? "" : node.Where.Accept(this)
            );

        }

        public override string VisitValuesSourceNode(ValuesSourceNode node)
        {
            return string.Format("VALUES ({0})",
                string.Join(",", node.Values.Select(v => v.Accept(this)))
                );
        }

        public override string VisitInsertNode(InsertNode node)
        {
            return string.Format("INSERT INTO {0}\n{1}",
                node.Into.Accept(this),
                node.DataSource.Accept(this)
                );
        }

        public override string VisitSetNode(SetNode node)
        {
            return string.Format("{0} = {1}",
                node.Field.Accept(this),
                node.Value.Accept(this)
                );
        }

        public override string VisitUpdateNode(UpdateNode node)
        {
            return string.Format("UPDATE {0}\nSET {1}\n{2}{3}",
                node.Update.Accept(this),
                string.Join(", ", node.Set.Select(s => s.Accept(this))),
                node.From==null ? "" : node.From.Accept(this),
                node.Where==null ? "" : node.Where.Accept(this)
                ) ;
        }

        public override string VisitDeleteNode(DeleteNode node)
        {
            return string.Format("DELETE FROM {0}\n{1}",
                node.From == null ? "" : node.From.Accept(this),
                node.Where == null ? "" : node.Where.Accept(this)
                );
        }
    }
}
