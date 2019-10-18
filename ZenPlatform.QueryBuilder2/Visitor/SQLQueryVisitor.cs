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
/*
        public override string VisitConstraintDefinitionDefaultMethod(ConstraintDefinitionDefaultMethod node)
        {
            return string.Format("{0}DEFAULT {1} FOR {2}",
                string.IsNullOrEmpty(node.Name) ? "" : $"CONSTRAINT {node.Name} ",
                node.Method, //TODO 
                node.Column.Accept(this)
                );
        }

        public override string VisitConstraintDefinitionDefaultValue(ConstraintDefinitionDefaultValue node)
        {
            return string.Format("{0}DEFAULT {1} {2}",
                string.IsNullOrEmpty(node.Name) ? "" : $"CONSTRAINT {node.Name} ",
                node.Value,
                node.Column.Accept(this)
                );
        }




        
        public override string VisitAlterTableNode(AlterTableNode node)
        {
            return string.Join(";\n", node.Nodes.Select(c => $"ALTER TABLE {node.Table.Accept(this)} \n {c.Accept(this)}"));
        }

        public override string VisitAlterTypedActionNode(AlterTypedActionNode node)
        {
            string action = "";
            switch (node.Type)
            {
                case NodeActionType.Add:
                    action = "ADD";
                    break;
                case NodeActionType.Drop:
                    action = "DROP";
                    break;
                case NodeActionType.Alter:
                    action = "ALTER COLUMN";
                    break;
            }

            return string.Format("{0} {1}", action, node.Node.Accept(this));
        }

        public override string VisitColumnDefinitionNode(ColumnDefinitionNode node)
        {

            return string.Format("{0} {1} {2} {3}", 
                node.Column.Accept(this), 
                node.Type.Accept(this),
                string.Join(",", node.Constraints.Select(c=>c.Accept(this))),
                node.Identity ? "IDENTITY" : "");

        }

        public override string VisitColumnNode(ColumnNode node)
        {

            return node.Name;
        }

        public override string VisitConstraintNode(ConstraintNode node)
        {
            return node.Name;
        }

        public override string VisitForeignKeyColumnConstraintDefinitionNode(ForeignKeyColumnConstraintDefinitionNode node)
        {
            return base.VisitForeignKeyColumnConstraintDefinitionNode(node);
        }

        public override string VisitForeignKeyConstraintDefinitionNode(ForeignKeyConstraintDefinitionNode node)
        {
           
            return string.Format("{0}FOREIGN KEY{1}REFERENCES {2}({3})",
                node.Constraint!=null ? $"CONSTRAINT {node.Constraint.Accept(this)} " : "",
                node.Columns.Count > 0 ? $" ({string.Join(",", node.Columns.Select(c => c.Accept(this)))}) " : " ",
                node.PrimaryTable.Accept(this),
                string.Join(",", node.PrimaryColumns.Select(c => c.Accept(this)))
                );
        }

        public override string VisitIntTypeDefinitionNode(IntTypeDefinitionNode node)
        {
            return "INT";
        }

        public override string VisitNotNullConstraintDefinitionNode(NotNullConstraintDefinitionNode node)
        {
            return "NOT NULL";
        }

        public override string VisitPrimaryKeyColumnConstraintDefinitionNode(PrimaryKeyColumnConstraintDefinitionNode node)
        {
            return "PRIMARY KEY";
        }

        public override string VisitPrimaryKeyConstraintDefinitionNode(PrimaryKeyConstraintDefinitionNode node)
        {
            return string.Format("{0}PRIMARY KEY {1}",
                node.Constraint != null ? $"CONSTRAINT {node.Constraint.Accept(this)} " : "",
                node.Columns.Count > 0 ? $" ({string.Join(",", node.Columns.Select(c => c.Accept(this)))}) " : " "
                );

        }

        public override string VisitTableDefinitionNode(TableDefinitionNode node)
        {

            return string.Format("CREATE TABLE {0} \n(\n{1}{2}{3}\n)",
                node.Table.Accept(this),
                string.Join(",\n", node.Columns.Select(c => c.Accept(this))),
                node.Constraints.Count > 0 ? ",\n" : "",
                string.Join(",\n", node.Constraints.Select(c => c.Accept(this)))
                );

        }

        public override string VisitTableNode(TableNode node)
        {
            return $"[{node.Name}]"; 
        }

        public override string VisitTextTypeDefinitionNode(TextTypeDefinitionNode node)
        {
            return "TEXT";
        }

        public override string VisitUniqueColumnConstraintDefinitionNode(UniqueColumnConstraintDefinitionNode node)
        {
            return "UNIQUE";
        }

        public override string VisitUniqueConstraintDefinitionNode(UniqueConstraintDefinitionNode node)
        {
            return string.Format("{0}UNIQUE {1}",
                node.Constraint != null ? $"CONSTRAINT {node.Constraint.Accept(this)} " : "",
                node.Columns.Count > 0 ? $" ({string.Join(",", node.Columns.Select(c => c.Accept(this)))}) " : " "
                );
        }
        */
    }
}
