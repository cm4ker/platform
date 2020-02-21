﻿using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data.Entity;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Querying.Model;

namespace ZenPlatform.Core.Querying
{
    /// <summary>
    /// Обходит дерево запроса платформы и распарсивает все его части. Также отправляет запросы всем комопнентам на разворот
    /// </summary>
    public class ZSqlGrammarVisitor : ZSqlGrammarBaseVisitor<object>
    {
        private readonly QLang _stack;

        public ZSqlGrammarVisitor(IProject configuration, DataQueryConstructorContext context)
        {
            _stack = new QLang(configuration);
        }

        public ZSqlGrammarVisitor(QLang stackMachine)
        {
            _stack = stackMachine;
        }

        public override object VisitQuery_stmt(ZSqlGrammarParser.Query_stmtContext context)
        {
            _stack.bg_query();

            if (context.from_stmt() != null)
                Visit(context.from_stmt());

            if (context.where_stmt() != null)
                Visit(context.where_stmt());

            if (context.group_by_stmt() != null)
                Visit(context.group_by_stmt());

            if (context.having_stmt() != null)
                Visit(context.having_stmt());

            if (context.select_stmt() != null)
                Visit(context.select_stmt());

            _stack.st_query();

            return null;
        }

        public override object VisitExprEquality(ZSqlGrammarParser.ExprEqualityContext context)
        {
            base.VisitExprEquality(context);

            if (context.ASSIGN() != null)
            {
                _stack.eq();
            }
            else if (context.NOT_EQ2() != null)
            {
                _stack.ne();
            }

            return null;
        }

        public override object VisitJoin_constraint(ZSqlGrammarParser.Join_constraintContext context)
        {
            base.VisitJoin_constraint(context);

            return null;
        }

        public override object VisitJoin_clause(ZSqlGrammarParser.Join_clauseContext context)
        {
            base.VisitJoin_clause(context);

            _stack.@join();

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


            return null;
        }

        public override object VisitLiteral(ZSqlGrammarParser.LiteralContext context)
        {
            if (context.STRING_LITERAL() != null)
                _stack.ld_const(context.STRING_LITERAL().GetText());
            if (context.NUMERIC_LITERAL() != null)
                _stack.ld_const(double.Parse(context.NUMERIC_LITERAL().GetText()));

            return base.VisitLiteral(context);
        }

        public override object VisitTable_property(ZSqlGrammarParser.Table_propertyContext context)
        {
            base.VisitTable_property(context);
            _stack.ld_object_table(context.table_name().GetText());

            return null;
        }

        public override object VisitObject_name(ZSqlGrammarParser.Object_nameContext context)
        {
            _stack.ld_object_type(context.GetText());
            return base.VisitObject_name(context);
        }

        public override object VisitComponent_name(ZSqlGrammarParser.Component_nameContext context)
        {
            _stack.ld_component(context.GetText());
            return base.VisitComponent_name(context);
        }

        public override object VisitTable_alias(ZSqlGrammarParser.Table_aliasContext context)
        {
            _stack.@as(context.GetText());
            return base.VisitTable_alias(context);
        }

        public override object VisitExpr_column(ZSqlGrammarParser.Expr_columnContext context)
        {
            base.VisitExpr_column(context);

            if (context.table_name() != null)
                _stack.ld_name(context.table_name().GetText());
            else
                _stack.ld_source_context();

            _stack.ld_field(context.column_name().GetText());

            return null;
        }

        public override object VisitType_name(ZSqlGrammarParser.Type_nameContext context)
        {
            _stack.ld_type(context.GetText());
            return null;
        }

        public override object VisitExpr_cast(ZSqlGrammarParser.Expr_castContext context)
        {
            base.VisitExpr_cast(context);
            _stack.cast();

            return null;
        }

        public override object VisitResult_column(ZSqlGrammarParser.Result_columnContext context)
        {
            base.VisitResult_column(context);

            if (context.column_alias() != null)
                _stack.@as(context.column_alias().GetText());

            return null;
        }
    }
}