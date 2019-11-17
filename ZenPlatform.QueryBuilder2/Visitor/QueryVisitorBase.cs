﻿using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder.Visitor
{
    public partial class QueryVisitorBase<T>
    {
        private Stack<QuerySyntaxNode> _stack;
        public void Push(QuerySyntaxNode node)
        {
            _stack.Push(node);
        }

        public void Pop()
        {
            _stack.Pop();
        }

        public QuerySyntaxNode Peek()
        {
            return _stack.Peek();
        }

    }
}