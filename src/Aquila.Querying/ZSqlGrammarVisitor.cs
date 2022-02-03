using System;
using Aquila.Core.Querying.Model;

namespace Aquila.Core.Querying
{
    /// <summary>
    /// Visitor for creating AST tree for Platform query language
    /// </summary>
    public class ZSqlGrammarVisitor : ZSqlGrammarBaseVisitor<object>
    {
        private readonly QLang _stack;

        public ZSqlGrammarVisitor(QLang stackMachine)
        {
            _stack = stackMachine;
        }

        public override object VisitSql_stmt_list(ZSqlGrammarParser.Sql_stmt_listContext context)
        {
            _stack.create(QObjectType.QueryList);
            foreach (var query in context.query_stmt())
            {
                VisitQuery_stmt(query);
                _stack.st_elem();
            }

            return null;
        }

        public override object VisitCriterion_stmt(ZSqlGrammarParser.Criterion_stmtContext context)
        {
            _stack.new_scope();
            if (context.from_stmt() != null)
                Visit(context.from_stmt());

            if (context.where_stmt() != null)
                Visit(context.where_stmt());

            _stack.new_criterion();

            return null;
        }

        public override object VisitQuery_stmt(ZSqlGrammarParser.Query_stmtContext context)
        {
            // _stack.dup();
            _stack.new_scope();

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

            if (context.orderby_stmt() != null)
                Visit(context.orderby_stmt());

            _stack.new_select_query();


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
            // base.VisitJoin_clause(context);
            var count = context.table_or_subquery().Length;

            //first item this is inner table
            //count of joins is length - 1 time

            //load first ds
            Visit(context.table_or_subquery(0));

            _stack.create(QObjectType.JoinList);

            for (int i = 1; i < count; i++)
            {
                Visit(context.table_or_subquery(i));
                Visit(context.join_constraint(i - 1));

                var op = context.join_operator(i - 1);
                QJoinType type;

                if (op.LEFT() != null)
                    type = QJoinType.Left;
                else if (op.RIGHT() != null)
                    type = QJoinType.Right;
                else if (op.FULL() != null)
                    type = QJoinType.Full;
                else if (op.INNER() != null)
                    type = QJoinType.Inner;
                else if (op.CROSS() != null)
                    type = QJoinType.Cross;
                else
                    type = QJoinType.Inner;

                _stack.@join(type);
                _stack.st_elem();
            }

            return null;
        }

        public override object VisitSelect_stmt(ZSqlGrammarParser.Select_stmtContext context)
        {
            _stack.create(QObjectType.FieldList);

            base.VisitSelect_stmt(context);

            _stack.select();

            return null;
        }

        public override object VisitOrderby_stmt(ZSqlGrammarParser.Orderby_stmtContext context)
        {
            _stack.create(QObjectType.OrderList);
            base.VisitOrderby_stmt(context);

            _stack.order_by();

            return null;
        }

        public override object VisitGroup_by_stmt(ZSqlGrammarParser.Group_by_stmtContext context)
        {
            _stack.create(QObjectType.GroupList);
            base.VisitGroup_by_stmt(context);

            _stack.group_by();

            return null;
        }

        public override object VisitParameter(ZSqlGrammarParser.ParameterContext context)
        {
            _stack.ld_param(context.IDENTIFIER().GetText());
            return null;
        }

        public override object VisitVariable(ZSqlGrammarParser.VariableContext context)
        {
            _stack.ld_var(context.IDENTIFIER().GetText());
            return null;
        }

        public override object VisitWhere_stmt(ZSqlGrammarParser.Where_stmtContext context)
        {
            base.VisitWhere_stmt(context);
            _stack.where();
            return null;
        }

        public override object VisitFrom_stmt(ZSqlGrammarParser.From_stmtContext context)
        {
            base.VisitFrom_stmt(context);

            _stack.@from();
            return null;
        }

        public override object VisitExprRelational(ZSqlGrammarParser.ExprRelationalContext context)
        {
            base.VisitExprRelational(context);

            if (context.LT() != null)
                _stack.lt();

            if (context.GT() != null)
                _stack.gt();


            return null;
        }

        public override object VisitTable_qualified_name(ZSqlGrammarParser.Table_qualified_nameContext context)
        {
            var list = context.any_name();
            if (list.Length > 1)
            {
                _stack.ld_source(context.GetText());
            }
            else if (list.Length == 1 && list[0].GetText().ToLower() == "subject")
            {
                _stack.ld_subject();
            }

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
            Visit(context.component_name());
            Visit(context.object_name());

            _stack.ld_table(context.table_name().GetText());

            if (context.table_alias() != null)
                Visit(context.table_alias());

            return null;
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

        public override object VisitExprAdditive(ZSqlGrammarParser.ExprAdditiveContext context)
        {
            base.VisitExprAdditive(context);

            if (context.exprAdditive() != null)
            {
                if (context.PLUS() != null) _stack.add();
                if (context.MINUS() != null) throw new Exception("Not implemented");
            }

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
            if (context.STAR() != null)
            {
                _stack.ld_star();
                return null;
            }

            _stack.create(QObjectType.ResultColumn);

            if (context.column_alias() != null)
                _stack.@as(context.column_alias().GetText());

            _stack.st_elem();
            return null;
        }

        public override object VisitOrdered_column(ZSqlGrammarParser.Ordered_columnContext context)
        {
            //duplicate array of fields for load the new result column into it
            //_stack.dup();
            base.VisitOrdered_column(context);

            if (context.DESC() != null)
                _stack.ld_sort(QSortDirection.Descending);
            else
                _stack.ld_sort(QSortDirection.Ascending);

            _stack.create(QObjectType.OrderExpression);
            _stack.st_elem();
            return null;
        }

        public override object VisitGrouped_column(ZSqlGrammarParser.Grouped_columnContext context)
        {
            //duplicate array of fields for load the new result column into it
            //_stack.dup();
            base.VisitGrouped_column(context);

            _stack.create(QObjectType.GroupExpression);
            _stack.st_elem();
            return null;
        }
    }
}