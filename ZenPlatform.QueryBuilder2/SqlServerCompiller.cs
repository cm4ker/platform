using System.Text;
using ZenPlatform.QueryBuilder.Common.SqlTokens;
using ZenPlatform.QueryBuilder.DDL.Table;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.QueryBuilder.DML.Insert;

namespace ZenPlatform.QueryBuilder
{
    public class SqlServerCompiller : SqlCompillerBase
    {
        public override void VisitRenameTableQueryNode(RenameTableQueryNode renameTableQueryNode, StringBuilder sb)
        {
            sb.Append("sp_rename");
            VisitNode(Tokens.LeftBracketToken, sb);

            VisitNode(renameTableQueryNode.Table, sb);

            VisitNode(Tokens.CommaToken, sb);

            VisitNode(renameTableQueryNode.NewTableName, sb);

            VisitNode(Tokens.RightBracketToken, sb);
        }

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

            VisitNode(select.OrderByNode, sb);

            VisitNode(select.HavingNode, sb);

            if (selectQueryNode.Parent != null)
                sb.Append(CloseBracket);
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