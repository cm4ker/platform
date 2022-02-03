using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using Antlr4.Runtime;
using Aquila.Metadata;
using Aquila.Runtime.Querying;

namespace Aquila.Core.Querying.Model
{
    public class QLang
    {
        private readonly EntityMetadataCollection _metadata;

        private LogicStack _logicStack;
        private Stack<LogicScope> _scope;
        private QLangTypeBuilder _tb;

        public QLang(EntityMetadataCollection metadata)
        {
            _metadata = metadata;

            _logicStack = new LogicStack();
            _scope = new Stack<LogicScope>();
            _tb = new QLangTypeBuilder();
        }

        public static QLangElement Parse(string sql, EntityMetadataCollection md)
        {
            var m = new QLang(md);
            return Parse(m, sql);
        }

        public static QLangElement Parse(QLang machine, string sql)
        {
            AntlrInputStream inputStream = new AntlrInputStream(sql);
            ZSqlGrammarLexer speakLexer = new ZSqlGrammarLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            ZSqlGrammarParser parser = new ZSqlGrammarParser(commonTokenStream);
            ZSqlGrammarVisitor visitor = new ZSqlGrammarVisitor(machine);

            visitor.Visit(parser.parse());

            return machine.top() as QLangElement;
        }

        public LogicScope CurrentScope => _scope.TryPeek(out var res) ? res : null;


        /// <summary>
        /// Load on the stack list
        /// </summary>
        public QLang create(QObjectType type)
        {
            switch (type)
            {
                case QObjectType.FieldList:
                    _logicStack.Push(new QFieldList(ImmutableArray<QField>.Empty));
                    break;
                case QObjectType.DataSourceList:
                    _logicStack.Push(new QDataSourceList(ImmutableArray<QDataSource>.Empty));
                    break;
                case QObjectType.WhenList:
                    _logicStack.Push(new QWhenList(ImmutableArray<QWhen>.Empty));
                    break;
                case QObjectType.ExpressionList:
                    _logicStack.Push(new QExpressionList(ImmutableArray<QExpression>.Empty));
                    break;
                case QObjectType.JoinList:
                    _logicStack.Push(new QJoinList(ImmutableArray<QFromItem>.Empty));
                    break;
                case QObjectType.QueryList:
                    _logicStack.Push(new QQueryList(ImmutableArray<QQueryBase>.Empty));
                    break;
                case QObjectType.OrderList:
                    _logicStack.Push(new QOrderList(ImmutableArray<QOrderExpression>.Empty));
                    break;
                case QObjectType.OrderExpression:
                    _logicStack.Push(new QOrderExpression(_logicStack.PopItem<QSortDirection>(),
                        _logicStack.PopExpression()));
                    break;

                case QObjectType.GroupList:
                    _logicStack.Push(new QGroupList(ImmutableArray<QGroupExpression>.Empty));
                    break;
                case QObjectType.GroupExpression:
                    _logicStack.Push(new QGroupExpression(_logicStack.PopExpression()));
                    break;
                case QObjectType.ResultColumn:
                    new_result_column();

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return this;
        }

        /// <summary>
        /// Duplicate item on stack
        /// </summary>
        public QLang dup()
        {
            _logicStack.Push(_logicStack.Peek());
            return this;
        }

        public QLang st_elem()
        {
            var element = _logicStack.Pop();
            var objColl = _logicStack.Pop();

            if (!(objColl is QLangCollection coll))
                throw new Exception(
                    $"Stack corrupted. The collection is not an instance of the IQCollection. Current type is ({objColl.GetType()})");

            if (!(element is QLangElement elem))
                throw new Exception(
                    $"Stack corrupted. The collection is not an instance of the IQCollection. Current type is ({objColl.GetType()})");

            _logicStack.Push(coll.Add(elem));
            return this;
        }

        public QLang order_by()
        {
            _logicStack.Push(new QOrderBy(_logicStack.PopItem<QOrderList>()));
            return this;
        }

        private QLang having()
        {
            _logicStack.Push(new QHaving(_logicStack.PopExpression()));
            return this;
        }

        public QLang from()
        {
            _logicStack.Push(new QFrom(_logicStack.PopItem<QJoinList>(), _logicStack.PopDataSource()));
            return this;
        }

        public QLang select()
        {
            _logicStack.Push(new QSelect(_logicStack.PopItem<QFieldList>()));
            return this;
        }

        public QLang where()
        {
            _logicStack.Push(new QWhere(_logicStack.PopExpression()));
            return this;
        }

        public QLang group_by()
        {
            _logicStack.Push(new QGroupBy(_logicStack.PopItem<QGroupList>()));
            return this;
        }

        public QLang ld_component(string componentName)
        {
            // var component = _tm.FindComponentByName(componentName);
            // _logicStack.Push(component);

            //TODO: Epic: Implement component support

            return this;
        }

        public QLang ld_type(string typeName)
        {
            _logicStack.Push(_tb.Parse(typeName));
            return this;
        }


        /// <summary>
        /// used for load subject then criterion build
        /// need pass CriterionContext parameter to constructor for using this feature
        /// </summary>
        public QLang ld_subject()
        {
            var ds = (QDataSource)top();

            if (ds != null)
            {
                _logicStack.Push(ds);
                if (CurrentScope != null)
                    CurrentScope.AddDS(ds);
            }

            return this;
        }

        public QLang ld_table(string tableName)
        {
            var ds = _logicStack.PopDataSource();
            var table = ds.FindTable(tableName);

            _logicStack.Push(table);
            if (CurrentScope != null)
            {
                CurrentScope.AddDS(table);
                CurrentScope.RemoveDS(ds);
            }

            return this;
        }

        public QLang ld_ref(QLangElement element)
        {
            _logicStack.Push(element);
            return this;
        }


        /// <summary>
        /// Load some data source from current context
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="typeName"></param>
        /// <param name="alias"></param>
        public QLang ld_source(string qualifiedName)
        {
            //load entity type
            var type = _metadata.GetSemanticByName(qualifiedName);
            var ds = new QObjectTable(type);
            _logicStack.Push(ds);
            if (CurrentScope != null)
                CurrentScope.AddDS(ds);

            return this;
        }

        public QLang ld_param(string name)
        {
            _logicStack.Push(new QParameter(name));

            return this;
        }

        public QLang ld_var(string name)
        {
            _logicStack.Push(new QVar(name));
            return this;
        }

        public QLang ld_source_context()
        {
            _logicStack.Push(
                new QCombinedDataSource(new QDataSourceList(CurrentScope.GetScopedDS())));

            return this;
        }

        public QLang ld_name(string name)
        {
            if (CurrentScope.TryGetDS(name, out var source))
            {
                _logicStack.Push(source);
            }
            else
            {
                throw new Exception($"The name :'{name}' not found in scope");
            }

            return this;
        }

        public QLang ld_sort(QSortDirection direction)
        {
            _logicStack.Push(direction);

            return this;
        }

        public QLang ld_nothing()
        {
            _logicStack.Push(null);

            return this;
        }

        public QLang ld_star()
        {
            var fields = CurrentScope.GetScopedDS().SelectMany(x => x.GetFields());

            foreach (var field in fields)
            {
                _logicStack.Push(field);
                st_elem();
            }

            return this;
        }

        public QLang ld_field(string name)
        {
            var ds = _logicStack.PopDataSource();

            var result = ds.GetFields().Where(x => x.GetName() == name).ToList();

            if (result.Count() > 1)
            {
                throw new Exception($"Ambiguous field with name {name}");
            }

            if (!result.Any())
            {
                throw new Exception($"Field with name {name} not found");
            }

            _logicStack.Push(result.First());

            return this;
        }

        public QLang @as(string alias)
        {
            var item = _logicStack.Pop();

            if (item is QDataSource aqds && aqds.Substituted)
            {
                CurrentScope.RemoveDS(aqds);
                CurrentScope.AddDS(aqds, alias);
                return this;
            }

            if (item is QAliasedDataSource || item is QAliasedSelectExpression)
            {
                throw new Exception("You can't alias object twice");
            }

            if (item is QDataSource ds)
            {
                var ads = new QAliasedDataSource(ds, alias);
                _logicStack.Push(ads);
                CurrentScope.ReplaceDS(ds, ads);
            }
            else if (item is QSelectExpression expr)
                _logicStack.Push(new QAliasedSelectExpression(expr, alias));
            else if (item is QField field)
                _logicStack.Push(new QAliasedSelectExpression(field, alias));
            else
            {
                throw new Exception("Element on stack not available for aliasing");
            }

            return this;
        }

        /// <summary>
        /// creates new visibility scope for creating objects
        /// scope automatically close then you invoke closing-scope instruction
        /// like new_query()
        /// </summary>
        public QLang new_scope()
        {
            _scope.Push(new LogicScope());
            return this;
        }

        private LogicScope pop_scope()
        {
            return _scope.Pop();
        }

        public QLang lookup(string propName)
        {
            _logicStack.Push(new QLookupField(propName, _logicStack.PopExpression()));
            return this;
        }

        public QLang st_data_request()
        {
            pop_scope();
            _logicStack.Push(new QDataRequest(_logicStack.PopItem<QFieldList>()));

            return this;
        }

        /// <summary>
        /// Pop from stack
        /// QOrderBy
        /// QSelect
        /// QHaving
        /// QGroupBy
        /// QWhere
        /// QFrom
        ///
        /// in that order
        /// </summary>
        public QLang new_select_query()
        {
            var scope = pop_scope();

            var query = new QSelectQuery(_logicStack.PopItem<QOrderBy>(),
                _logicStack.PopItem<QSelect>(), _logicStack.PopItem<QHaving>(),
                _logicStack.PopItem<QGroupBy>(), _logicStack.PopItem<QWhere>(),
                _logicStack.PopItem<QFrom>(), QCriterionList.Empty);

            //we need validate query before push it to the stack            

            _logicStack.Push(query);


            if (_scope.Count > 0) //we are inside the nested query
                _logicStack.Push(new QNestedQuery(_logicStack.PopQuery()));

            return this;
        }

        public QLang new_insert_query()
        {
            pop_scope();

            var insert = new QInsertQuery(
                _logicStack.PopItem<QExpressionSet>(),
                _logicStack.PopItem<QInsert>(),
                QCriterionList.Empty);

            return this;
        }

        public QLang new_criterion()
        {
            var scope = pop_scope();
            var criterion = new QCriterion(_logicStack.PopItem<QWhere>(), _logicStack.PopItem<QFrom>());

            //we need validate query before push it to the stack            

            _logicStack.Push(criterion);

            return this;
        }

        public QLang new_result_column()
        {
            var expr = pop();
            if (expr is QField item)
            {
                _logicStack.Push(new QSelectExpression(item));
            }
            else if (expr is QExpression exp)
            {
                _logicStack.Push(new QSelectExpression(exp));
            }

            return this;
        }

        /// <summary>
        /// Creates inner join 
        /// </summary>
        public QLang join(QJoinType joinType)
        {
            return join_with_type(QJoinType.Inner);
        }

        private QLang join_with_type(QJoinType type)
        {
            _logicStack.Push(new QFromItem(_logicStack.PopExpression(), _logicStack.PopDataSource(), type));
            return this;
        }

        /// <summary>
        /// Снять со стэка
        /// </summary>
        /// <returns></returns>
        public object pop()
        {
            return _logicStack.Pop();
        }

        /// <summary>
        /// Получить верхний элемент без снятия
        /// </summary>
        /// <returns></returns>
        public object top()
        {
            return _logicStack.Peek();
        }

        public T top<T>()
        {
            return (T)top();
        }

        /// <summary>
        /// And operator
        /// </summary>
        public QLang and()
        {
            _logicStack.Push(new QAnd(_logicStack.PopExpression(), _logicStack.PopExpression()));

            return this;
        }

        /// <summary>
        /// Additional operator
        /// </summary>
        public QLang add()
        {
            _logicStack.Push(new QAdd(_logicStack.PopExpression(), _logicStack.PopExpression()));
            return this;
        }


        /// <summary>
        /// Equals operator
        /// </summary>
        public QLang eq()
        {
            _logicStack.Push(new QEquals(_logicStack.PopExpression(), _logicStack.PopExpression()));
            return this;
        }


        /// <summary>
        /// Equals
        /// </summary>
        public QLang gt()
        {
            _logicStack.Push(new QGreatThen(_logicStack.PopExpression(), _logicStack.PopExpression()));
            return this;
        }

        /// <summary>
        /// Great or equals then
        /// </summary>
        public QLang gte()
        {
            _logicStack.Push(new QGreatThenOrEquals(_logicStack.PopExpression(), _logicStack.PopExpression()));
            return this;
        }

        /// <summary>
        /// Less or equals then
        /// </summary>
        public QLang lte()
        {
            _logicStack.Push(new QLessThenOrEquals(_logicStack.PopExpression(), _logicStack.PopExpression()));
            return this;
        }

        /// <summary>
        /// Less then
        /// </summary>
        public QLang lt()
        {
            _logicStack.Push(new QLessThen(_logicStack.PopExpression(), _logicStack.PopExpression()));
            return this;
        }


        /// <summary>
        /// Not equals operator
        /// </summary>
        public QLang ne()
        {
            _logicStack.Push(new QNotEquals(_logicStack.PopExpression(), _logicStack.PopExpression()));
            return this;
        }

        /// <summary>
        /// Reset machine to the initial state
        /// </summary>
        public QLang reset()
        {
            _logicStack.Clear();
            _scope.Clear();

            return this;
        }


        /// <summary>
        /// load null on top of the stack
        /// </summary>
        public QLang ld_empty()
        {
            _logicStack.Push(null);

            return this;
        }

        //Create case
        public QLang @case()
        {
            _logicStack.Push(new QCase(_logicStack.PopExpression(), _logicStack.PopItem<QWhenList>()));

            return this;
        }

        /// <summary>
        /// Create when 
        /// </summary>
        /// <returns></returns>
        public QLang when()
        {
            _logicStack.Push(new QWhen(_logicStack.PopExpression(),
                _logicStack.PopOpExpression()));

            return this;
        }

        /// <summary>
        /// Casting argument on top of stack
        /// </summary>
        /// <returns></returns>
        public QLang cast()
        {
            _logicStack.Push(new QCast(_logicStack.PopType(), _logicStack.PopExpression()));

            return this;
        }

        /// <summary>
        /// Load constant value on top of the stack
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns></returns>
        public QLang ld_const(string str)
        {
            _logicStack.Push(new QConst(new SMType(SMType.String), str));

            return this;
        }

        /// <summary>
        /// Load constant value on top of the stack
        /// </summary>
        /// <param name="number">numeric value. Note: default format is (scale = 10, precision = 10)</param>
        /// <returns></returns>
        public QLang ld_const(double number)
        {
            _logicStack.Push(new QConst(new SMType(SMType.Numeric, 0, 10, 10), number));

            return this;
        }
    }
}