using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Enumeration;
using System.Linq;
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

    public class ZqlLogicalTreeWriter
    {
        private readonly XCRoot _conf;

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

        private struct State
        {
            public State(StateToken stateToken, LTItem item)
            {
                StateToken = stateToken;
                Item = item;
                InSelect = false;
            }

            public StateToken StateToken { get; }
            public LTItem Item { get; }

            public bool InSelect { get; }
        }

        private Stack<State> _state;

        private int _queryIdCounter;


        public ZqlLogicalTreeWriter(XCRoot conf)
        {
            _conf = conf;
        }

        public void WriteQuery()
        {
            _state.Push(new State(StateToken.Query, new LTQuery()));
        }

        public void WriteCloseQuery()
        {
            while (_state.TryPop(out var state) && !(state.Item is LTQuery))
            {
            }
        }

        public void WriteField(string name, string ownerAlias)
        {
            var state = _state.Pop();

            if (state.Item is LTQuery query)
                HandleFieldBelongsQuery(name, ownerAlias, query);
            else if (state.Item is LTExpression)
            {
                query = (LTQuery) _state.First(x => x.Item is LTQuery).Item;
                HandleFieldBelongsQuery(name, ownerAlias, query);
            }
        }

        public void WriteSource(string componentName, string typeName, string alias)
        {
            var state = _state.Pop();
            if (!(state.Item is LTQuery query)) throw new Exception("The source must belongs to the query");

            var component = _conf.Data.Components.First(x => x.Info.ComponentName == componentName);
            var type = component.GetTypeByName(typeName);

            query.From.Add(new LTObjectTable(type, alias));
        }

        private void HandleFieldInExpression(LTExpression expression)
        {
        }

        private void HandleFieldBelongsQuery(string name, string ownerAlias, LTQuery query)
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

                            _state.Push(new State(StateToken.Field, new LTObjectField(prop)));
                            return;
                        }
                    }
                    else
                    {
                    }
                }
            }
        }


        public void WriteSource()
        {
            _state.Push(new State(StateToken.Source, new LTObjectTable()));
        }

        public void WriteEndQuery()
        {
            _state.Pop();
        }
    }
}