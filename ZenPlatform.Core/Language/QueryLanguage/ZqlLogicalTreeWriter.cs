using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;

namespace ZenPlatform.Core.Language.QueryLanguage
{
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
        /// Записать поле объекта(которое уже имеет доступ к данным)
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="alias">Псевдоним поля</param>
        /// <param name="ownerAlias">Псевдоним владельца</param>
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
                            new LTSelectExpression(query) {Aliase = alias, Child = new LTObjectField(prop)});
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
            }
            else if (state.Item is LTExpression field)
            {
                query = (LTQuery) _stack.First(x => x.Item is LTQuery).Item;
                var prop = GetPropertyFromQuery(name, ownerAlias, query);

                field.Child = new LTObjectField(prop);
                field.Child.Parent = field;
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

        public void WriteCaseOperator(string alias = "")
        {
            var state = _stack.Peek();
            var caseExpr = new LTCase();

            _stack.Push(new State(StateToken.Expression, caseExpr, state.ClauseType));

            if (state.Item is LTQuery query)
            {
                switch (state.ClauseType)
                {
                    case ClauseType.Where:
                        query.Where = caseExpr;
                        break;
                    case ClauseType.Select:
                        query.Select.Add(new LTSelectExpression(query) {Aliase = alias, Child = caseExpr});
                        break;
                    case ClauseType.OrderBy:
                        query.OrderBy.Add(caseExpr);
                        break;
                    case ClauseType.GroupBy:
                        query.GroupBy.Add(caseExpr);
                        break;
                    default:
                        throw new Exception("In this state you can't add field to the query");
                }
            }
            else if (state.Item is LTOperationExpression expr)
            {
                expr.PushArgument(caseExpr);
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