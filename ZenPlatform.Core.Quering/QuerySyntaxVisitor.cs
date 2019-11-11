using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Language.QueryLanguage.Model;
using ZenPlatform.Core.Quering.QueryLanguage;

namespace ZenPlatform.Core.Language.QueryLanguage
{
    /// <summary>
    /// Обходит дерево запроса платформы и распарсивает все его части. Также отправляет запросы всем комопнентам на разворот
    /// </summary>
    public class ZSqlGrammarVisitor : ZSqlGrammarBaseVisitor<object>
    {
        private readonly QLang _stack;

        public ZSqlGrammarVisitor(XCRoot configuration, DataQueryConstructorContext context)
        {
            _stack = new QLang(configuration);
        }

        public override object VisitQuery_stmt(ZSqlGrammarParser.Query_stmtContext context)
        {
            Visit(context.from_stmt());
            Visit(context.where_stmt());
            Visit(context.group_by_stmt());
            Visit(context.having_stmt());
            Visit(context.select_stmt());

            return null;
        }

        public override object VisitSelect_stmt(ZSqlGrammarParser.Select_stmtContext context)
        {
            _stack.m_select();
            return base.VisitSelect_stmt(context);
        }

        public override object VisitFrom_stmt(ZSqlGrammarParser.From_stmtContext context)
        {
            _stack.m_from();
            return base.VisitFrom_stmt(context);
        }

        public override object VisitTable(ZSqlGrammarParser.TableContext context)
        {
            _stack.ld_source(context.component_name().GetText(), context.object_name().GetText(),
                context.table_alias()?.GetText());
            return base.VisitTable(context);
        }

        public override object VisitObject_name(ZSqlGrammarParser.Object_nameContext context)
        {
            _stack.ld_type(context.GetText());
            return base.VisitObject_name(context);
        }

        public override object VisitComponent_name(ZSqlGrammarParser.Component_nameContext context)
        {
            _stack.ld_component(context.GetText());
            return base.VisitComponent_name(context);
        }

        public override object VisitTable_alias(ZSqlGrammarParser.Table_aliasContext context)
        {
            _stack.alias(context.GetText());
            return base.VisitTable_alias(context);
        }

        public override object VisitColumn_alias(ZSqlGrammarParser.Column_aliasContext context)
        {
            _stack.alias(context.GetText());
            return base.VisitColumn_alias(context);
        }

        public override object VisitExpr_column(ZSqlGrammarParser.Expr_columnContext context)
        {
            base.VisitExpr_column(context);
            _stack.ld_field(context.column_name().GetText());

            return null;
        }
    }
}