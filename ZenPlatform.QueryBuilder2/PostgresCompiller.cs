﻿using System.Text;
using ZenPlatform.QueryBuilder.Common.Tokens;
using ZenPlatform.QueryBuilder.DML.Insert;
using ZenPlatform.QueryBuilder.DML.Select;

namespace ZenPlatform.QueryBuilder
{
    public class PostgresCompiller : SqlCompillerBase
    {
        public override string StartNameLiteral { get; } = "\"";
        public override string EndNameLiteral { get; } = "\"";


        protected override void VisitSelectQueryNode(SelectQueryNode selectQueryNode, StringBuilder sb)
        {
            ISelectQuery select = selectQueryNode;

            if (selectQueryNode.Parent != null)
                sb.Append(OpenBracket);

            VisitNode(select.SelectNode, sb);

            VisitNode(select.FromNode, sb);

            VisitNode(select.WhereNode, sb);

            VisitNode(select.GroupByNode, sb);

            VisitNode(select.HavingNode, sb);

            //Обрабатываем LIMIT
            if (select.TopNode != null)
                VisitNode(select.TopNode, sb);

            if (selectQueryNode.Parent != null)
                sb.Append(CloseBracket);
        }

        protected override void VisitSelectNode(SelectNode selectNode, StringBuilder sb)
        {
            VisitNode(Tokens.SelectToken, sb);
            VisitNode(Tokens.SpaceToken, sb);

            VisitChilds(selectNode, sb);
        }

        protected override void VisitTopNode(TopNode topNode, StringBuilder sb)
        {
            VisitNode(Tokens.LimitToken, sb);
            VisitNode(Tokens.SpaceToken, sb);
            sb.Append(topNode.Count);
            VisitNode(Tokens.SpaceToken, sb);
        }

        protected override void VisitInsertQueryNode(InsertQueryNode insertQueryNode, StringBuilder sb)
        {
            IInsertQuery insert = insertQueryNode;

            VisitNode(Tokens.InsertToken, sb);
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(Tokens.IntoToken, sb);

            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(insert.TableWithColumnsNode, sb);
            
            VisitNode(Tokens.SpaceToken, sb);
            VisitNode(insert.InsertValuesNode, sb);
        }
    }
}