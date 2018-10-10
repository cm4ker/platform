using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Enumeration;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MoreLinq.Extensions;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
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
    }

    /// <summary>
    /// Построитель логического дерева запроса
    /// </summary>
    public class ZqlLogicalTreeWriter
    {
        private readonly XCRoot _conf;
        private LTQuery _result;

        private enum StateToken
        {
            /// <summary>
            /// Открытие запроса
            /// </summary>
            Query,

            /// <summary>
            /// Открыть поле 
            /// </summary>
            Field,

            /// <summary>
            /// Начало источника
            /// </summary>
            Source,

            /// <summary>
            /// Выражение
            /// </summary>
            Expression
        }

        private enum ClauseType
        {
            From,
            Where,
            GroupBy,
            Having,
            Select,
            OrderBy
        }


        private class State
        {
            public State(StateToken stateToken, LTItem item)
            {
                StateToken = stateToken;
                Item = item;
                ClauseType = ClauseType.From;
            }

            public State(StateToken stateToken, LTItem item, ClauseType clauseType) : this(stateToken, item)
            {
                ClauseType = clauseType;
            }

            public StateToken StateToken { get; }

            public ClauseType ClauseType { get; set; }

            public LTItem Item { get; }
        }

        private Stack<State> _stack;

        private int _queryIdCounter;


        public ZqlLogicalTreeWriter(XCRoot conf)
        {
            _conf = conf;
            _stack = new Stack<State>();
        }

        public void WriteQuery()
        {
            LTQuery newQuery = new LTQuery();

            if (!_stack.Any())
            {
                _result = newQuery;
            }

            _stack.Push(new State(StateToken.Query, newQuery));
        }

        public void WriteCloseQuery()
        {
            while (_stack.TryPop(out var state) && !(state.Item is LTQuery))
            {
            }
        }

        private void WriteClauseType(ClauseType type)
        {
            var state = _stack.Peek();
            if (!(state.Item is LTQuery))
                throw new Exception(
                    $"The root state can't change on object type {state.Item.GetType()}");

            state.ClauseType = type;
        }

        public void WriteWhere()
        {
            WriteClauseType(ClauseType.Where);
        }

        public void WriteGroupBy()
        {
            WriteClauseType(ClauseType.GroupBy);
        }

        public void WriteHaving()
        {
            WriteClauseType(ClauseType.Having);
        }

        public void WriteSelect()
        {
            WriteClauseType(ClauseType.Select);
        }

        public void WriteOrderBy()
        {
            WriteClauseType(ClauseType.OrderBy);
        }

        /// <summary>
        /// Написать поле
        /// </summary>
        /// <param name="name"></param>
        /// <param name="alias"></param>
        /// <param name="ownerAlias"></param>
        /// <exception cref="Exception"></exception>
        public void WriteObjectField(string name, string alias, string ownerAlias)
        {
            var state = _stack.Peek();

            if (state.Item is LTQuery query)
            {
                var prop = GetPropertyFromQuery(name, ownerAlias, query);

                switch (state.ClauseType)
                {
                    case ClauseType.Select:
                        query.Select.Add(
                            new LTSelectExpression() {Aliase = alias, Expression = new LTObjectField(prop)});
                        break;
                    case ClauseType.GroupBy:
                        query.GroupBy.Add(new LTObjectField(prop));
                        break;
                    case ClauseType.OrderBy:
                        query.OrderBy.Add(new LTObjectField(prop));
                        break;
                    default:
                        throw new Exception("In this state you can't add field to the query");
                }
            }
            else if (state.Item is LTOperationExpression expr)
            {
                query = (LTQuery) _stack.First(x => x.Item is LTQuery).Item;
                var prop = GetPropertyFromQuery(name, ownerAlias, query);

                expr.PushArgument(new LTObjectField(prop));

                /*
                 * Stack (Select):
                 * Query
                 * CaseExpression
                 * LogicalAnd
                 * Condition
                 */
            }
        }

        public void WriteSource(string componentName, string typeName, string alias = "")
        {
            var state = _stack.Peek();
            if (!(state.Item is LTQuery query)) throw new Exception("The source must belongs to the query");

            var component = _conf.Data.Components.First(x => x.Info.ComponentName == componentName);
            var type = component.GetTypeByName(typeName);

            query.From.Add(new LTObjectTable(type, alias));
        }

        public void WriteEqualsOperator(string alias = "")
        {
            var state = _stack.Peek();
            var eqExpr = new LTEquals();

            _stack.Push(new State(StateToken.Expression, eqExpr, state.ClauseType));

            if (state.Item is LTQuery query)
            {
                switch (state.ClauseType)
                {
                    case ClauseType.Where:
                        query.Where = eqExpr;
                        break;
                    default:
                        throw new Exception("In this state you can't add field to the query");
                }
            }
            else if (state.Item is LTOperationExpression expr)
            {
                expr.PushArgument(eqExpr);
            }
        }

        private XCObjectPropertyBase GetPropertyFromQuery(string name, string ownerAlias, LTQuery query)
        {
            foreach (var source in query.From)
            {
                if (source is LTObjectTable objectTable)
                {
                    if (!string.IsNullOrEmpty(ownerAlias))
                    {
                        if (objectTable.Alias.ToLowerInvariant() == ownerAlias)
                        {
                            var type = objectTable.ObjectType;
                            var prop = type.GetPropertyByName(name);

                            return prop;
                        }
                    }
                    else
                    {
                    }
                }
            }

            return null;
        }

        public void WriteEndQuery()
        {
            _stack.Pop();
        }

        public void WriteEndExpression()
        {
            _stack.Pop();
        }


        public LTQuery Result => _result;
    }
}