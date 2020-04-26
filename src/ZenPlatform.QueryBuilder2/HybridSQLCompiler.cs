using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Visitor;

namespace ZenPlatform.QueryBuilder
{
    public class HybridSQLCompiler : ISqlCompiler
    {
        private QueryVisitorBase<string>  _queryVisitor;
        private SqlCompillerBase _oldCompiler;
        public HybridSQLCompiler(QueryVisitorBase<string> queryVisitor, SqlCompillerBase oldCompiler)
        {
            _queryVisitor = queryVisitor;
            _oldCompiler = oldCompiler;
        }
        public string Compile(SqlNode node)
        {

            if (node is QuerySyntaxNode q)
                return _queryVisitor.Visit(q);
            else return _oldCompiler.Compile(node);
               
        }
    }
}
