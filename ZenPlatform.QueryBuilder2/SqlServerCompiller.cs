using System.Text;
using ZenPlatform.QueryBuilder.Common.Tokens;
using ZenPlatform.QueryBuilder.DML.Select;

namespace ZenPlatform.QueryBuilder
{
    public class SqlServerCompiller : SqlCompillerBase
    {
        protected override void VisitSelectQueryNode(SelectQueryNode selectQueryNode, StringBuilder sb)
        {
            ISelectQuery select = selectQueryNode;

            if (selectQueryNode.Parent != null)
                sb.Append(OpenBracket);

            VisitNode(Tokens.SelectToken, sb);
            VisitNode(Tokens.SpaceToken, sb);

            //Обрабатываем TOP
            if (select.TopNode != null)
                VisitNode(select.TopNode, sb);

            VisitChilds(select.SelectNode, sb);

            VisitNode(select.FromNode, sb);

            VisitNode(select.WhereNode, sb);

            VisitNode(select.GroupByNode, sb);

            VisitNode(select.HavingNode, sb);

            if (selectQueryNode.Parent != null)
                sb.Append(CloseBracket);
        }
    }
}