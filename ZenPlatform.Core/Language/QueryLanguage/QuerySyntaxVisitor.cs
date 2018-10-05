using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using MoreLinq.Extensions;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.ParenChildCollection;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Core.Language.QueryLanguage
{
    /// <summary>
    /// Обходит дерево запроса платформы и распарсивает все его части. Также отправляет запросы всем комопнентам на разворот
    /// </summary>
    public class ZSqlGrammarVisitor : ZSqlGrammarBaseVisitor<SqlNode>
    {
        private SqlNode _result;
        private DataQueryConstructorContext _context;
        private XCRoot _conf;
        private Stack<LTItem> _dependencyStack;

        public ZSqlGrammarVisitor(XCRoot configuration, DataQueryConstructorContext context)
        {
            _result = new SelectQueryNode();
            _conf = configuration;
            _context = context;
        }

        public override SqlNode VisitResult_column(ZSqlGrammarParser.Result_columnContext context)
        {
            return base.VisitResult_column(context);
        }

        public override SqlNode VisitSelect_stmt(ZSqlGrammarParser.Select_stmtContext context)
        {
            _dependencyStack.Push(new LTQuery());
            var result = base.VisitSelect_stmt(context);
            return result;
        }

        public override SqlNode VisitExpr(ZSqlGrammarParser.ExprContext context)
        {
            return base.VisitExpr(context);
        }

        public override SqlNode VisitTable_or_subquery(ZSqlGrammarParser.Table_or_subqueryContext context)
        {
            if (context.component_name() != null)
            {
                var componentName = context.component_name().GetText();
                var objectName = context.object_name().GetText();

                return null;
            }
            else return base.VisitTable_or_subquery(context);
        }

        public override SqlNode VisitParse(ZSqlGrammarParser.ParseContext context)
        {
            return base.VisitParse(context);
        }
    }



    public class ZqlLogicalTreeWriter
    {
        private enum StateToken
        {
            OpenQuery,
            CloseQuery,
            ObjectField,
            ObjectSource,
            ExpressionSource,
            Expression,
            Value
        }

        private class State
        {
            public StateToken StateToken { get; set; }
            public object Bag { get; set; }
        }

        private Stack<State> _state;

        private int _queryIdCounter;

        public void WriteStartQuery()
        {

        }

        public void WriteEndQuery()
        {

        }
    }
}
