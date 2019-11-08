﻿using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Enumeration;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MoreLinq.Extensions;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Core.Language.QueryLanguage.Model;
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
    public class ZSqlGrammarVisitor : ZSqlGrammarBaseVisitor<LTItem>
    {
        private readonly QLang _stack;

        public ZSqlGrammarVisitor(XCRoot configuration, DataQueryConstructorContext context)
        {
            _stack = new QLang(configuration);
        }

        public override LTItem VisitQuery_stmt(ZSqlGrammarParser.Query_stmtContext context)
        {
            Visit(context.from_stmt());
            Visit(context.where_stmt());
            Visit(context.group_by_stmt());
            Visit(context.having_stmt());
            Visit(context.select_stmt());

            return null;
        }

        public override LTItem VisitSelect_stmt(ZSqlGrammarParser.Select_stmtContext context)
        {
            _stack.m_select();
            return base.VisitSelect_stmt(context);
        }

        public override LTItem VisitFrom_stmt(ZSqlGrammarParser.From_stmtContext context)
        {
            _stack.m_from();
            return base.VisitFrom_stmt(context);
        }

        public override LTItem VisitTable(ZSqlGrammarParser.TableContext context)
        {
            _stack.ld_source(context.component_name().GetText(), context.object_name().GetText(),
                context.table_alias()?.GetText());
            return base.VisitTable(context);
        }

        public override LTItem VisitObject_name(ZSqlGrammarParser.Object_nameContext context)
        {
            _stack.ld_type(context.GetText());
            return base.VisitObject_name(context);
        }

        public override LTItem VisitComponent_name(ZSqlGrammarParser.Component_nameContext context)
        {
            _stack.ld_component(context.GetText());
            return base.VisitComponent_name(context);
        }

        public override LTItem VisitTable_alias(ZSqlGrammarParser.Table_aliasContext context)
        {
            _stack.alias(context.GetText());
            return base.VisitTable_alias(context);
        }

        public override LTItem VisitColumn_alias(ZSqlGrammarParser.Column_aliasContext context)
        {
            _stack.alias(context.GetText());
            return base.VisitColumn_alias(context);
        }

        public override LTItem VisitExpr_column(ZSqlGrammarParser.Expr_columnContext context)
        {
            base.VisitExpr_column(context);
            _stack.ld_field(context.column_name().GetText());

            return null;
        }
    }
}