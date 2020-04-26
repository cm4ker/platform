using System;
using System.Linq;
using System.Text;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Common.Columns;
using ZenPlatform.QueryBuilder.Common.Conditions;
using ZenPlatform.QueryBuilder.Common.Operations;
using ZenPlatform.QueryBuilder.Common.SqlTokens;
using ZenPlatform.QueryBuilder.Common.Table;
using ZenPlatform.QueryBuilder.DDL.CreateDatabase;
using ZenPlatform.QueryBuilder.DDL.CreateTable;
using ZenPlatform.QueryBuilder.DDL.Index;
using ZenPlatform.QueryBuilder.DDL.Table;
using ZenPlatform.QueryBuilder.DML.Delete;
using ZenPlatform.QueryBuilder.DML.From;
using ZenPlatform.QueryBuilder.DML.GroupBy;
using ZenPlatform.QueryBuilder.DML.Having;
using ZenPlatform.QueryBuilder.DML.Insert;
using ZenPlatform.QueryBuilder.DML.OrderBy;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.QueryBuilder.DML.Update;
using ZenPlatform.QueryBuilder.DML.Where;
using ZenPlatform.QueryBuilder.Visitor;
using ZenPlatform.Shared;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.QueryBuilder
{
    public abstract class SqlCompillerBase: ISqlCompiler
    {
        public static ISqlCompiler FormEnum(SqlDatabaseType databaseType)
        {
            switch (databaseType)
            {
                //case SqlDatabaseType.SqlServer: return new SqlServerCompiller();
                case SqlDatabaseType.SqlServer: return new HybridSQLCompiler(new SQLServerVisitor(), new SqlServerCompiller());
                case SqlDatabaseType.Postgres: return new HybridSQLCompiler(new PostgreVisitor(), new PostgresCompiller());
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

        public virtual void Compile(SqlNode node, StringBuilder sb)
        {
            VisitNode(node, sb);

            foreach (SqlNode nodeChild in node.Childs)
            {
                VisitNode(nodeChild, sb);
            }
        }


        public virtual string Compile(SqlNode node)
        {
            var sb = new StringBuilder();
            Compile(node, sb);

            return sb.ToString();
        }

        protected virtual void VisitNode(SqlNode node, StringBuilder sb)
        {
            ItemSwitch<Node>
                .Switch(node)
                .CaseIs<SelectQueryNode>((i) => { VisitSelectQueryNode(i, sb); })
                .CaseIs<UpdateQueryNode>(i => VisitUpdateQueryNode(i, sb))
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
                .CaseIs<OrderByNode>(i => VisitOrderByNode(i, sb))
                .CaseIs<SelectColumnNode>(i => VisitSelectFieldNode(i, sb))
                .CaseIs<SetFieldNode>(i => VisitSetFieldNode(i, sb))
                .CaseIs<ColumnNode>((i) => { VisitFieldNode(i, sb); })
                .CaseIs<JoinNode>((i) => { VisitJoinNode(i, sb); })
                .CaseIs<AliasedTableNode>((i) => { VisitAliasedTableNode(i, sb); })
                .CaseIs<AliasNode>((i) => { VisitAliasNode(i, sb); })
                .CaseIs<IdentifierNode>((i) => { VisitIdentifierNode(i, sb); })
                .CaseIs<RawSqlNode>((i) => { VisitRawSqlNode(i, sb); })
                .CaseIs<SchemaSeparatorNode>((i) => { VisitSchemaSeparatorNode(i, sb); })
                .CaseIs<SelectNastedQueryNode>((i) => { VisitSelectNastedQueryNode(i, sb); })
                .CaseIs<OnNode>(i => { VisitOnNode(i, sb); })
                .CaseIs<CompareOperatorNode>(i => { VisitCompareOperatorNode(i, sb); })
                .CaseIs<BinaryConditionNode>(i => { VisitBinaryWhereNode(i, sb); })
                .CaseIs<ParameterNode>(i => VisitParameterNode(i, sb))
                .CaseIs<LikeConditionNode>(i => VisitLikeWhereNode(i, sb))
                .CaseIs<StringLiteralNode>(i => VisitStringLiteralNode(i, sb))
                .CaseIs<TableNode>(i => VisitTableNode(i, sb))
                .CaseIs<Token>(i => VisitTokens(i, sb))
                .CaseIs<TypeDefinitionNode>(i => VisitTypeDefinitionNode(i, sb))
                .CaseIs<TopNode>(i => VisitTopNode(i, sb))
                .CaseIs<InsertQueryNode>(i => VisitInsertQueryNode(i, sb))
                .CaseIs<CreateDatabaseQueryNode>(i => VisitCreateDatabaseQueryNode(i, sb))
                .CaseIs<DropDatabaseQueryNode>(i => VisitDropDatabaseQueryNode(i, sb))
                .CaseIs<CreateIndexQueryNode>(i => VisitCreateIndexQueryNode(i, sb))
                .CaseIs<IndexTableNode>(i => VisitIndexTableNode(i, sb))
                .CaseIs<IndexTableColumnNode>(i => VisitIndexTableColumnNode(i, sb))
                .CaseIs<AndConditionNode>(i => VisitAndConditionNode(i, sb))
                .CaseIs<IsNullConditionNode>(i => VisitIsNullConditionNode(i, sb))
                .CaseIs<RenameTableQueryNode>(i => VisitRenameTableQueryNode(i, sb))
                .CaseIs<CreateTableQueryNode>(i=>VisitCreateTableQueryNode(i, sb))
                .Case(i => true, () => SimpleVisitor(node, sb));
        }

        public virtual void VisitRenameTableQueryNode(RenameTableQueryNode renameTableQueryNode, StringBuilder sb)
        {
            //All the RDBMS has different mechanics for rename the table
            throw new NotImplementedException();
        }

        public virtual  void VisitCreateTableQueryNode(CreateTableQueryNode createTableQueryNode, StringBuilder sb)
        {
           
        }

        private void VisitIsNullConditionNode(IsNullConditionNode isNullConditionNode, StringBuilder sb)
        {
            VisitChilds(isNullConditionNode, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(Tokens.IsToken, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(Tokens.NullToken, sb);
        }

        private void VisitAndConditionNode(AndConditionNode andConditionNode, StringBuilder sb)
        {
            VisitNode(Tokens.LeftBracketToken, sb);

            VisitChildsForeach(andConditionNode, sb, (node, builder, arg3) =>
            {
                if (arg3 > 0)
                {
                    VisitNode(Tokens.SpaceToken, sb);
                    VisitNode(Tokens.AndToken, sb);
                    VisitNode(Tokens.SpaceToken, sb);
                }

                VisitNode(node, sb);
            });

            VisitNode(Tokens.RightBracketToken, sb);
        }

        private void VisitIndexTableColumnNode(IndexTableColumnNode indexTableColumnNode, StringBuilder sb)
        {
            VisitChilds(indexTableColumnNode, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(indexTableColumnNode.Direction, sb);
        }

        private void VisitIndexTableNode(IndexTableNode indexTableNode, StringBuilder sb)
        {
            VisitChilds(indexTableNode, sb);
        }

        private void VisitCreateIndexQueryNode(CreateIndexQueryNode createIndexQueryNode, StringBuilder sb)
        {
            ICreateIndexQuery q = createIndexQueryNode;
            VisitNode(Tokens.CreateToken, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(Tokens.IndexToken, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(q.IndexName, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(Tokens.OnToken, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(q.TargetTable, sb);
        }

        private void VisitDropDatabaseQueryNode(DropDatabaseQueryNode dropDatabaseQueryNode, StringBuilder sb)
        {
            IDropDatabaseQuery q = dropDatabaseQueryNode;

            VisitNode(Tokens.DropToken, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(Tokens.DatabaseToken, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(q.Name, sb);
        }

        private void VisitCreateDatabaseQueryNode(CreateDatabaseQueryNode createDatabaseQueryNode, StringBuilder sb)
        {
            ICreateDatabaseQuery dbQuery = createDatabaseQueryNode;

            VisitNode(Tokens.CreateToken, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(Tokens.DatabaseToken, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(dbQuery.Name, sb);
        }

        protected virtual void VisitInsertQueryNode(InsertQueryNode insertQueryNode, StringBuilder sb)
        {
        }

        protected virtual void VisitTopNode(TopNode topNode, StringBuilder sb)
        {
        }

        protected virtual void VisitUpdateQueryNode(UpdateQueryNode updateQueryNode, StringBuilder sb)
        {
        }

        protected virtual void SimpleVisitor(SqlNode node, StringBuilder sb)
        {
            VisitChilds(node, sb);
        }

        protected virtual void VisitColumnDefinitionNode(ColumnDefinitionNode columnDefinitionNode, StringBuilder sb)
        {
            VisitChilds(columnDefinitionNode, sb);
        }

        protected virtual void VisitTableDefinitionNode(TableDefinitionNode tableDefinitionNode, StringBuilder sb)
        {
            VisitChilds(tableDefinitionNode, sb);
        }

        protected virtual void VisitTypeDefinitionNode(TypeDefinitionNode typeDefinitionNode, StringBuilder sb)
        {
            VisitChilds(typeDefinitionNode, sb);
        }

        protected virtual void VisitTableNode(TableNode tableNode, StringBuilder sb)
        {
            VisitChilds(tableNode, sb);
        }

        protected virtual void VisitTokens(Token token, StringBuilder sb)
        {
            VisitChilds(token, sb);
        }

        protected virtual void VisitColumnListNode(ColumnListNode columnListNode, StringBuilder sb)
        {
            VisitNode(Tokens.LeftBracketToken, sb);
            VisitChildsForeach(columnListNode, sb, (node, isb, index) =>
            {
                if (index > 0)
                {
                    VisitNode(Tokens.CommaToken, sb);
                    VisitNode(Tokens.SpaceToken, sb);
                }

                VisitNode(node, sb);
            });
            VisitNode(Tokens.RightBracketToken, sb);
        }

        protected virtual void VisitCloseBracketNode(CloseBracketNode closeBracketNode, StringBuilder sb)
        {
            sb.Append(CloseBracket);
        }

        protected virtual void VisitOpenBracketNode(OpenBraketNode openBraketNode, StringBuilder sb)
        {
            sb.Append(OpenBracket);
        }

        protected virtual void VisitInsertValuesNode(InsertValuesNode insertValuesNode, StringBuilder sb)
        {
            sb.Append("VALUES").Append(OpenBracket);

            var last = insertValuesNode.Childs.Last();

            VisitChildsForeach(insertValuesNode, sb, (n, b, index) =>
            {
                VisitNode(n, sb);

                if (n != last)
                {
                    sb.Append(Comma).Append(" ");
                }
            });
            sb.Append(CloseBracket);
        }

        protected virtual void VisitTableWithColumnsNode(TableWithColumnsNode tableWithColumnsNode, StringBuilder sb)
        {
            VisitChilds(tableWithColumnsNode, sb);
        }

        protected virtual void VisitInsertIntoNode(InsertIntoNode insertIntoNode, StringBuilder sb)
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

        protected virtual void VisitOrderByNode(OrderByNode orderByNode, StringBuilder sb)
        {
            if (!orderByNode.Childs.Any()) return;
            sb.Append("\nORDER BY ");
            sb.Append("\n");
            sb.Append("    ");
            VisitChilds(orderByNode, sb);

            if (orderByNode.IsDesc)
                sb.Append("\nDESC");

        }

        protected virtual void VisitGroupByNode(GroupByNode groupByNode, StringBuilder sb)
        {
            if (!groupByNode.Childs.Any()) return;
            sb.Append("\nGROUP BY ");
            sb.Append("\n");
            sb.Append("    ");
            VisitChilds(groupByNode, sb);
        }

        protected virtual void VisitHavingNode(HavingNode havingNode, StringBuilder sb)
        {
            if (!havingNode.Childs.Any()) return;
            sb.Append("\nHAVING ");

            sb.Append("\n");
            sb.Append("    ");
            VisitChilds(havingNode, sb);
        }

        protected virtual void VisitLikeWhereNode(LikeConditionNode likeConditionNode, StringBuilder sb)
        {
            VisitNode(likeConditionNode.Expression, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(Tokens.LikeToken, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(likeConditionNode.Pattern, sb);
        }

        protected virtual void VisitSetFieldNode(SetFieldNode setFieldNode, StringBuilder sb)
        {
            VisitChilds(setFieldNode, sb);
        }

        protected virtual void VisitSetNode(SetNode setNode, StringBuilder sb)
        {
            sb.Append("SET");

            VisitChildsForeach(setNode, sb, (n, s, index) =>
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

        protected virtual void VisitBinaryWhereNode(BinaryConditionNode binaryConditionNode, StringBuilder sb)
        {
            VisitChilds(binaryConditionNode, sb);
        }

        protected virtual void VisitFieldNode(ColumnNode columnNode, StringBuilder sb)
        {
            VisitChilds(columnNode, sb);
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

        protected virtual void VisitChilds(SqlNode node, StringBuilder sb)
        {
            foreach (SqlNode nodeChild in node.Childs)
            {
                VisitNode(nodeChild, sb);
            }
        }

        protected virtual void VisitChildsForeach(SqlNode node, StringBuilder sb,
            Action<SqlNode, StringBuilder, int> visitChild)
        {
            var index = 0;
            foreach (SqlNode nodeChild in node.Childs)
            {
                visitChild(nodeChild, sb, index);
                index++;
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

        protected virtual void VisitSelectFieldNode(SelectColumnNode selectColumnNode, StringBuilder sb)
        {
            VisitChilds(selectColumnNode, sb);
        }

        protected virtual void VisitFromNode(FromNode fromNode, StringBuilder sb)
        {
            if (!fromNode.Childs.Any()) return;
            sb.Append($"\n");
            VisitNode(Tokens.FromToken, sb);
            sb.Append(" ");

            foreach (SqlNode fromNodeChild in fromNode.Childs)
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