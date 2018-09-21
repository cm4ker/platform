using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Data.Contracts.Entity;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.QueryBuilder.Common;
using ZenPlatform.QueryBuilder.DML.Select;
using ZenPlatform.Shared.ParenChildCollection;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.Core.Language.QueryLanguage
{

    /// <summary>
    /// Лист дерева зависимости запроса. Нужно для оптимизаций, маппинга и разбора запроса
    /// </summary>
    public class QueryItem : IChildItem<QueryItem>
    {
        private QueryItem _parent;

        public QueryItem(Guid queryId, Guid itemId, string literal)
        {
            QueryId = queryId;
            ItemId = itemId;
            Dependencies = new ChildItemCollection<QueryItem, QueryItem>(this);
            Literal = literal;
        }

        /// <summary>
        /// Идентификатор 
        /// </summary>
        public Guid ItemId { get; }

        /// <summary>
        /// Идентификатор запроса
        /// </summary>
        public Guid QueryId { get; }

        /// <summary>
        /// Представление в запросе
        /// </summary>
        public string Literal { get; }

        /// <summary>
        /// Связанный элемент
        /// </summary>
        public object Bag { get; set; }

        /// <summary>
        /// Родитель
        /// </summary>
        QueryItem IChildItem<QueryItem>.Parent { get => _parent; set => _parent = value; }

        /// <summary>
        /// Зависимости
        /// </summary>
        private ChildItemCollection<QueryItem, QueryItem> Dependencies { get; }

        /// <summary>
        /// Зарегистрировать новую зависимость для этого элемента. Она будет тоже вычислена, при вычислении данного элемента
        /// </summary>
        public void RegisterDependency(QueryItem item)
        {
            Dependencies.Add(item);
        }

        List<object> Evaluate()
        {
            var result = new List<object>();

            if (Bag != null)
                result.Add(Bag);

            foreach (var dependency in Dependencies)
            {
                result.AddRange(dependency.Evaluate());
            }

            return result;
        }
    }



    /// <summary>
    /// Обходит дерево запроса платформы и распарсивает все его части. Также отправляет запросы всем комопнентам на разворот
    /// </summary>
    public class ZSqlGrammarVisitor : ZSqlGrammarBaseVisitor<SqlNode>
    {
        private SqlNode _result;
        private DataQueryConstructorContext _context;
        private Dictionary<string, string> _aliases;

        private List<QueryItem> _queryItems = new List<QueryItem>();

        private Stack<Guid> _queryStack;

        private XCRoot _conf;


        public ZSqlGrammarVisitor(XCRoot configuration, DataQueryConstructorContext context)
        {
            _result = new SelectQueryNode();
            _aliases = new Dictionary<string, string>();
            _conf = configuration;
            _context = context;
        }

        public override SqlNode VisitSelect_stmt(ZSqlGrammarParser.Select_stmtContext context)
        {
            _queryStack.Push(Guid.NewGuid());
            var result = base.VisitSelect_stmt(context);
            _queryStack.Pop();
            return result;
        }

        public override SqlNode VisitExpr(ZSqlGrammarParser.ExprContext context)
        {
            if (context.object_name() != null && context.component_name() != null)
            {
                if (_aliases.TryGetValue(context.component_name().GetText(), out var componentName))
                {
                    var objectName = context.object_name().GetText();
                    var component = _conf.Data.Components.FirstOrDefault(x => x.Info.ComponentName == componentName);

                    if (component == null)
                    {
                        throw new Exception("Комопнент не найден");
                    }

                    var type = component.Types.FirstOrDefault(x => x.Name == objectName);

                    //TODO: Необходимо отдать компоненту данные либо получить определение от компонента
                }
                else
                {
                    // Нужно как-то поругаться, что не нашли компонент
                    //throw new Exception("if");
                }

            }
            else if (context.object_name() != null)
            {

            }

            return base.VisitExpr(context);
        }

        public override SqlNode VisitTable_or_subquery(ZSqlGrammarParser.Table_or_subqueryContext context)
        {
            if (context.component_name() != null && context.object_name() != null)
            {
                var objectName = context.object_name().GetText();
                var componentName = context.component_name().GetText();

                var component = _conf.Data.Components.FirstOrDefault(x => x.Info.ComponentName == componentName);

                if (component == null)
                {
                    throw new Exception("Комопнент не найден");
                }

                var type = component.Types.FirstOrDefault(x => x.Name == objectName);

                if (type == null)
                {
                    throw new Exception("Объект не найден");
                }

                //TODO: Необходимо отдать компоненту данные либо получить определение от компонента

                var literal = $"{componentName}.{objectName}";
                var queryId = _queryStack.Peek();

                var queryItem = _queryItems.FirstOrDefault(x => x.Literal != literal && x.QueryId != queryId);

                if (queryItem == null)
                {
                    //Регистрируем зависимость
                    queryItem = new QueryItem(queryId, type.Guid, literal);
                    queryItem.Bag = type;
                    _queryItems.Add(queryItem);
                }
            }

            if (context.select_stmt() != null && context.table_alias() != null)
            {

            }

            return base.VisitTable_or_subquery(context);

        }

        public override SqlNode VisitParse(ZSqlGrammarParser.ParseContext context)
        {
            base.VisitParse(context);
            return _result;
        }
    }
}
