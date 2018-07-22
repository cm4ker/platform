using System;
using System.Linq;
using System.Text;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Columns;
using ZenPlatform.QueryBuilder.Common.Operations;
using ZenPlatform.QueryBuilder.Common.Table;
using ZenPlatform.QueryBuilder.Common.Tokens;
using ZenPlatform.QueryBuilder.DML.Delete;
using ZenPlatform.QueryBuilder.DML.From;
using ZenPlatform.QueryBuilder.DML.GroupBy;
using ZenPlatform.QueryBuilder.DML.Having;
using ZenPlatform.QueryBuilder.DML.Insert;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.QueryBuilder.DML.Update;
using ZenPlatform.QueryBuilder.DML.Where;
using ZenPlatform.Shared;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder
{
    public abstract class SqlCompillerBase
    {
        public static SqlCompillerBase FormEnum(SqlDatabaseType databaseType)
        {
            switch (databaseType)
            {
                case SqlDatabaseType.SqlServer: return new SqlServerCompiller();
                case SqlDatabaseType.Postgres: return new PostgresCompiller();
            }

            throw new NotSupportedException();
        }

        public virtual string StartNameLiteral { get; } = "[";
        public virtual string EndNameLiteral { get; } = "]";

        public virtual string StartStringLiteral { get; } = "'";
        public virtual string EndStringLiteral { get; } = "'";

        public virtual string SchemaSeparator { get; } = ".";
        public virtual string Comma { get; } = ",";

        public virtual string OpenBracket { get; } = "(";
        public virtual string CloseBracket { get; } = ")";

        public virtual void Compile(Node node, StringBuilder sb)
        {
            foreach (var nodeChild in node.Childs)
            {
                VisitNode(nodeChild, sb);
            }
        }


        public virtual string Compile(Node node)
        {
            var sb = new StringBuilder();
            Compile(node, sb);

            return sb.ToString();
        }

        protected virtual void VisitNode(Node node, StringBuilder sb)
        {
            ItemSwitch<Node>
                .Switch(node)
                .CaseIs<SelectQueryNode>((i) => { VisitSelectQueryNode(i, sb); })
                //.CaseIs<UpdateQueryNode>(i => VisitUpdateQueryNode(i, sb))
                .CaseIs<DeleteNode>(i => VisitDeleteNode(i, sb))
                .CaseIs<SelectNode>((i) => { VisitSelectNode(i, sb); })
                .CaseIs<UpdateNode>(i => VisitUpdateNode(i, sb))
                .CaseIs<InsertIntoNode>(i => VisitInsertIntoNode(i, sb))
                .CaseIs<TableWithColumnsNode>(i => VisitTableWithColumnsNode(i, sb))
                .CaseIs<OpenBraketNode>(i => VisitOpenBracketNode(i, sb))
                .CaseIs<CloseBracketNode>(i => VisitCloseBracketNode(i, sb))
                .CaseIs<InsertValuesNode>(i => VisitInsertValuesNode(i, sb))
                .CaseIs<TableDefinitionNode>(i => VisitTableDefinitionNode(i, sb))
                .CaseIs<ColumnDefinitionNode>(i => VisitColumnDefinitionNode(i, sb))
                .CaseIs<ColumnListNode>(i => VisitColumnListNode(i, sb))
                .CaseIs<SetNode>(i => VisitSetNode(i, sb))
                .CaseIs<FromNode>((i) => { VisitFromNode(i, sb); })
                .CaseIs<WhereNode>(i => VisitWhereNode(i, sb))
                .CaseIs<HavingNode>(i => VisitHavingNode(i, sb))
                .CaseIs<GroupByNode>(i => VisitGroupByNode(i, sb))
                .CaseIs<SelectFieldNode>(i => VisitSelectFieldNode(i, sb))
                .CaseIs<SetFieldNode>(i => VisitSetFieldNode(i, sb))
                .CaseIs<FieldNode>((i) => { VisitFieldNode(i, sb); })
                .CaseIs<JoinNode>((i) => { VisitJoinNode(i, sb); })
                .CaseIs<AliasedTableNode>((i) => { VisitAliasedTableNode(i, sb); })
                .CaseIs<AliasNode>((i) => { VisitAliasNode(i, sb); })
                .CaseIs<IdentifierNode>((i) => { VisitIdentifierNode(i, sb); })
                .CaseIs<RawSqlNode>((i) => { VisitRawSqlNode(i, sb); })
                .CaseIs<SchemaSeparatorNode>((i) => { VisitSchemaSeparatorNode(i, sb); })
                .CaseIs<SelectNastedQueryNode>((i) => { VisitSelectNastedQueryNode(i, sb); })
                .CaseIs<OnNode>(i => { VisitOnNode(i, sb); })
                .CaseIs<CompareOperatorNode>(i => { VisitCompareOperatorNode(i, sb); })
                .CaseIs<BinaryWhereNode>(i => { VisitBinaryWhereNode(i, sb); })
                .CaseIs<ParameterNode>(i => VisitParameterNode(i, sb))
                .CaseIs<LikeWhereNode>(i => VisitLikeWhereNode(i, sb))
                .CaseIs<StringLiteralNode>(i => VisitStringLiteralNode(i, sb))
                .CaseIs<TableNode>(i => VisitTableNode(i, sb))
                .CaseIs<Token>(i => VisitTokens(i, sb))
                .CaseIs<TypeDefinitionNode>(i => VisitTypeDefinitionNode(i, sb))
                .Case(i => true, () => SimpleVisitor(node, sb));
        }

        private void SimpleVisitor(Node node, StringBuilder sb)
        {
            VisitChilds(node, sb);
        }

        private void VisitColumnDefinitionNode(ColumnDefinitionNode columnDefinitionNode, StringBuilder sb)
        {
            VisitChilds(columnDefinitionNode, sb);
        }

        private void VisitTableDefinitionNode(TableDefinitionNode tableDefinitionNode, StringBuilder sb)
        {
            VisitChilds(tableDefinitionNode, sb);
        }

        private void VisitTypeDefinitionNode(TypeDefinitionNode typeDefinitionNode, StringBuilder sb)
        {
            VisitChilds(typeDefinitionNode, sb);
        }

        private void VisitTableNode(TableNode tableNode, StringBuilder sb)
        {
            VisitChilds(tableNode, sb);
        }

        private void VisitTokens(Token token, StringBuilder sb)
        {
            VisitChilds(token, sb);
            //            ItemSwitch<Token>
            //                .Switch(token)
            //                .CaseIs<RightBracketToken>(i=> sb.Append());
        }

        private void VisitColumnListNode(ColumnListNode columnListNode, StringBuilder sb)
        {
            VisitChilds(columnListNode, sb);
        }

        private void VisitCloseBracketNode(CloseBracketNode closeBracketNode, StringBuilder sb)
        {
            sb.Append(CloseBracket);
        }

        private void VisitOpenBracketNode(OpenBraketNode openBraketNode, StringBuilder sb)
        {
            sb.Append(OpenBracket);
        }

        private void VisitInsertValuesNode(InsertValuesNode insertValuesNode, StringBuilder sb)
        {
            sb.Append("VALUES").Append(OpenBracket);

            var last = insertValuesNode.Childs.Last();

            VisitChildsForeach(insertValuesNode, sb, (n, b) =>
            {
                VisitNode(n, sb);

                if (n != last)
                {
                    sb.Append(Comma).Append(" ");
                }
            });
            sb.Append(CloseBracket);
        }

        private void VisitTableWithColumnsNode(TableWithColumnsNode tableWithColumnsNode, StringBuilder sb)
        {
            VisitChilds(tableWithColumnsNode, sb);
            sb.Append(" ");
        }

        private void VisitInsertIntoNode(InsertIntoNode insertIntoNode, StringBuilder sb)
        {
            sb.Append("INSERT INTO ");
            VisitChilds(insertIntoNode, sb);
        }

        protected virtual void VisitStringLiteralNode(StringLiteralNode stringLiteralNode, StringBuilder sb)
        {
            sb.AppendFormat("{0}{1}{2}", StartStringLiteral, stringLiteralNode.RawString, EndStringLiteral);
        }

        protected virtual void VisitDeleteNode(DeleteNode deleteNode, StringBuilder sb)
        {
            sb.Append("DELETE\n    ");
            VisitChilds(deleteNode, sb);
        }

        protected virtual void VisitGroupByNode(GroupByNode groupByNode, StringBuilder sb)
        {
        }

        protected virtual void VisitHavingNode(HavingNode havingNode, StringBuilder sb)
        {
            if (!havingNode.Childs.Any()) return;
            sb.Append("\nHAVING ");

            sb.Append("\n");
            sb.Append("    ");
            VisitChilds(havingNode, sb);
        }

        protected virtual void VisitLikeWhereNode(LikeWhereNode likeWhereNode, StringBuilder sb)
        {
            VisitChilds(likeWhereNode, sb);
        }

        protected virtual void VisitSetFieldNode(SetFieldNode setFieldNode, StringBuilder sb)
        {
            VisitChilds(setFieldNode, sb);
        }

        protected virtual void VisitSetNode(SetNode setNode, StringBuilder sb)
        {
            sb.Append("SET");

            VisitChildsForeach(setNode, sb, (n, s) =>
            {
                s.Append("\n    ");
                VisitNode(n, s);
            });
        }

        protected virtual void VisitUpdateNode(UpdateNode updateNode, StringBuilder sb)
        {
            sb.Append("UPDATE\n    ");
            VisitChilds(updateNode, sb);
            sb.Append("\n");
        }

        protected virtual void VisitParameterNode(ParameterNode parameterNode, StringBuilder sb)
        {
            sb.Append("@");
            VisitChilds(parameterNode, sb);
        }

        protected virtual void VisitBinaryWhereNode(BinaryWhereNode binaryWhereNode, StringBuilder sb)
        {
            VisitChilds(binaryWhereNode, sb);
        }

        protected virtual void VisitFieldNode(FieldNode fieldNode, StringBuilder sb)
        {
            VisitChilds(fieldNode, sb);
        }

        protected virtual void VisitCompareOperatorNode(CompareOperatorNode compareOperatorNode, StringBuilder sb)
        {
            //Todo: handle compare operator sign
            //sb.Append("=");
            VisitChilds(compareOperatorNode, sb);
        }

        protected virtual void VisitOnNode(OnNode onNode, StringBuilder sb)
        {
            sb.Append(" ON ");
            VisitChilds(onNode, sb);
        }

        protected virtual void VisitSelectQueryNode(SelectQueryNode selectQueryNode, StringBuilder sb)
        {
            if (selectQueryNode.Parent != null)
                sb.Append(OpenBracket);

            VisitChilds(selectQueryNode, sb);

            if (selectQueryNode.Parent != null)
                sb.Append(CloseBracket);
        }

        protected virtual void VisitSelectNastedQueryNode(SelectNastedQueryNode selectNastedQueryNode, StringBuilder sb)
        {
            VisitChilds(selectNastedQueryNode, sb);
        }

        protected virtual void VisitAliasNode(AliasNode aliasNode, StringBuilder sb)
        {
            sb.AppendFormat(" {0} ", "AS");
            VisitChilds(aliasNode, sb);
        }

        protected virtual void VisitAliasedTableNode(AliasedTableNode aliasedTableNode, StringBuilder sb)
        {
            VisitChilds(aliasedTableNode, sb);
        }

        protected virtual void VisitJoinNode(JoinNode joinNode, StringBuilder sb)
        {
            switch (joinNode.JoinType)
            {
                case JoinType.Inner:
                    sb.Append("INNER");
                    break;
                case JoinType.Left:
                    sb.Append("LEFT");
                    break;
                case JoinType.Right:
                    sb.Append("RIGHT");
                    break;
                case JoinType.Full:
                    sb.Append("FULL");
                    break;
                case JoinType.Cross:
                    sb.Append("CROSS");
                    break;
                //TODO: Добавить все
            }

            sb.Append(" JOIN ");
            VisitChilds(joinNode, sb);
        }


        protected virtual void VisitIdentifierNode(IdentifierNode node, StringBuilder sb)
        {
            sb.AppendFormat("{0}{1}{2}", StartNameLiteral, node.Name, EndNameLiteral);
        }

        protected virtual void VisitRawSqlNode(RawSqlNode node, StringBuilder sb)
        {
            sb.Append(node.Raw);
        }

        protected virtual void VisitSchemaSeparatorNode(SchemaSeparatorNode node, StringBuilder sb)
        {
            sb.Append(SchemaSeparator);
        }

        protected virtual void VisitChilds(Node node, StringBuilder sb)
        {
            foreach (var nodeChild in node.Childs)
            {
                VisitNode(nodeChild, sb);
            }
        }

        protected virtual void VisitChildsForeach(Node node, StringBuilder sb,
            Action<Node, StringBuilder> visitChild)
        {
            foreach (var nodeChild in node.Childs)
            {
                visitChild(nodeChild, sb);
            }
        }

        protected virtual void VisitSelectNode(SelectNode selectNode, StringBuilder sb)
        {
            if (!selectNode.HasFields)
                return;

            VisitChilds(selectNode, sb);

            //            foreach (var selectNodeChild in selectNode.Childs)
            //            {
            //                sb.Append("\n    ");
            //
            //                VisitNode(selectNodeChild, sb);
            //                if (selectNodeChild != lastChild)
            //                    sb.AppendFormat("{0}", Comma);
            //            }
        }

        protected virtual void VisitSelectFieldNode(SelectFieldNode selectFieldNode, StringBuilder sb)
        {
            VisitChilds(selectFieldNode, sb);
        }

        protected virtual void VisitFromNode(FromNode fromNode, StringBuilder sb)
        {
            if (!fromNode.Childs.Any()) return;
            sb.Append("\nFROM ");

            foreach (var fromNodeChild in fromNode.Childs)
            {
                sb.Append("\n    ");
                VisitNode(fromNodeChild, sb);
            }
        }


        protected virtual void VisitWhereNode(WhereNode whereNode, StringBuilder sb)
        {
            if (!whereNode.Childs.Any()) return;
            sb.Append("\nWHERE ");

            sb.Append("\n");
            sb.Append("    ");
            VisitChilds(whereNode, sb);
        }
    }
}