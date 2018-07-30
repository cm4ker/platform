using System.Text;
using ZenPlatform.QueryBuilder.Common.Tokens;
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

            VisitNode(Tokens.SelectToken, sb);
            VisitNode(Tokens.SpaceToken, sb);

            VisitChilds(select.SelectNode, sb);

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

        protected override void VisitTopNode(TopNode topNode, StringBuilder sb)
        {
            VisitNode(Tokens.LimitToken, sb);
            VisitNode(Tokens.SpaceToken, sb);
            sb.Append(topNode.Count);
            VisitNode(Tokens.SpaceToken, sb);
        }
    }
}