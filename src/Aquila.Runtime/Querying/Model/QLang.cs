using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aquila.Metadata;

namespace Aquila.Core.Querying.Model
{
    public class QLang
    {
        private LogicStack _logicStack;
        private Stack<LogicScope> _scope;
        private QLangTypeBuilder _tb;

        public QLang()
        {
            _logicStack = new LogicStack();
            _scope = new Stack<LogicScope>();
            _tb = new QLangTypeBuilder();
        }

        public LogicScope CurrentScope => _scope.TryPeek(out var res) ? res : null;


        /// <summary>
        /// Load on the stack list
        /// </summary>
        public void create(QObjectType type)
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
                    _logicStack.Push(new QQueryList(ImmutableArray<QQuery>.Empty));
                    break;
                case QObjectType.OrderList:
                    _logicStack.Push(new QOrderList(ImmutableArray<QOrderExpression>.Empty));
                    break;
                case QObjectType.OrderExpression:
                    _logicStack.Push(new QOrderExpression(_logicStack.PopItem<QSortDirection>(),
                        _logicStack.PopExpression()));
                    break;
                case QObjectType.ResultColumn:
                    new_result_column();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        /// Duplicate item on stack
        /// </summary>
        public void dup()
        {
            _logicStack.Push(_logicStack.Peek());
        }

        public void st_elem()
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
        }

        public void order_by()
        {
            _logicStack.Push(new QOrderBy(_logicStack.PopItem<QOrderList>()));
        }

        private void having()
        {
            _logicStack.Push(new QHaving(_logicStack.PopExpression()));
        }

        public void from()
        {
            _logicStack.Push(new QFrom(_logicStack.PopItem<QJoinList>(), _logicStack.PopDataSource()));
        }

        public void select()
        {
            _logicStack.Push(new QSelect(_logicStack.PopItem<QFieldList>()));
        }

        public void where()
        {
            _logicStack.Push(new QWhere(_logicStack.PopExpression()));
        }

        private void gorup_by()
        {
            _logicStack.Push(new QGroupBy(_logicStack.PopItem<QExpressionList>()));
        }

        public void ld_component(string componentName)
        {
            // var component = _tm.FindComponentByName(componentName);
            // _logicStack.Push(component);

            //TODO: Epic: Implement component support
        }

        public void ld_type(string typeName)
        {
            _logicStack.Push(_tb.Parse(typeName));
        }

        public void ld_object_type(string typeName)
        {
            var metadata = TestMetadata.GetTestMetadata();
            var type = metadata.GetSemanticByName(typeName);

            // var type = _logicStack.PopComponent().FindTypeByName(typeName);
            var ds = new QObjectTable(type);
            _logicStack.Push(ds);
            if (CurrentScope != null)
                CurrentScope.ScopedDataSources.Add(ds);
        }

        public void ld_object_table(string tableName)
        {
            var ds = _logicStack.PopDataSource();
            var table = ds.FindTable(tableName);

            _logicStack.Push(table);
            if (CurrentScope != null)
            {
                CurrentScope.ScopedDataSources.Add(table);
                CurrentScope.ScopedDataSources.Remove(ds);
            }
        }

        /// <summary>
        /// Записать источник данных. Ссылка на него полностью получается из метаданных 
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="typeName"></param>
        /// <param name="alias"></param>
        public void ld_source(string componentName, string typeName, string p_alias = "")
        {
            ld_component(componentName);
            ld_object_type(typeName);

            if (!string.IsNullOrEmpty(p_alias))
            {
                @as(p_alias);
            }
        }

        public void ld_param(string name)
        {
            _logicStack.Push(new QParameter(name));
        }

        public void ld_source_context()
        {
            _logicStack.Push(
                new QCombinedDataSource(new QDataSourceList(CurrentScope.ScopedDataSources.ToImmutableArray())));
        }

        public void ld_name(string name)
        {
            if (CurrentScope.Scope.TryGetValue(name, out var source))
            {
                _logicStack.Push(source);
            }
            else
            {
                throw new Exception($"The name :'{name}' not found in scope");
            }
        }

        public void ld_sort(QSortDirection direction)
        {
            _logicStack.Push(direction);
        }

        public void ld_nothing()
        {
            _logicStack.Push(null);
        }

        public void ld_field(string name)
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
        }

        public void @as(string alias)
        {
            var item = _logicStack.Pop();

            if (item is QDataSource source)
            {
                var ds = new QAliasedDataSource(source, alias);

                _logicStack.Push(ds);

                CurrentScope.ScopedDataSources.Remove(source);
                CurrentScope.ScopedDataSources.Add(ds);

                CurrentScope.Scope.Add(alias, ds);
            }
            else if (item is QSelectExpression expr)
            {
                _logicStack.Push(new QAliasedSelectExpression(expr, alias));
            }
            else if (item is QField field)
                _logicStack.Push(new QAliasedSelectExpression(field, alias));
            else

            {
                throw new Exception("In this element not available for aliasing");
            }
        }

        public void new_scope()
        {
            _scope.Push(new LogicScope());
        }

        public void lookup(string propName)
        {
            _logicStack.Push(new QLookupField(propName, _logicStack.PopExpression()));
        }

        public void st_data_request()
        {
            _scope.Pop();
            _logicStack.Push(new QDataRequest(_logicStack.PopItem<QFieldList>()));
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
        public void new_query()
        {
            _scope.Pop();


            _logicStack.Push(new QQuery(_logicStack.PopItem<QOrderBy>(),
                _logicStack.PopItem<QSelect>(), _logicStack.PopItem<QHaving>(),
                _logicStack.PopItem<QGroupBy>(), _logicStack.PopItem<QWhere>(),
                _logicStack.PopItem<QFrom>()));

            if (_scope.Count > 0) //мы находимся во внутреннем запросе
                _logicStack.Push(new QNestedQuery(_logicStack.PopQuery()));
        }


        public void new_result_column()
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
        }

        /// <summary>
        /// Внутреннее соединение 
        /// </summary>
        public void join()
        {
            join_with_type(QJoinType.Inner);
        }

        /// <summary>
        /// Левое соединение
        /// </summary>
        public void left_join()
        {
            join_with_type(QJoinType.Left);
        }

        /// <summary>
        /// Правое соединение
        /// </summary>
        public void right_join()
        {
            join_with_type(QJoinType.Right);
        }

        public void cross_join()
        {
            join_with_type(QJoinType.Cross);
        }

        /// <summary>
        /// Полное соединение
        /// </summary>
        public void full_join()
        {
            join_with_type(QJoinType.Full);
        }

        private void join_with_type(QJoinType type)
        {
            _logicStack.Push(new QFromItem(_logicStack.PopExpression(), _logicStack.PopDataSource(), type));
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

        /// <summary>
        /// Логическое И
        /// </summary>
        public void and()
        {
            _logicStack.Push(new QAnd(_logicStack.PopExpression(), _logicStack.PopExpression()));
        }

        /// <summary>
        /// Конкатенация
        /// </summary>
        public void add()
        {
            _logicStack.Push(new QAdd(_logicStack.PopExpression(), _logicStack.PopExpression()));
        }


        /// <summary>
        /// Равно
        /// </summary>
        public void eq()
        {
            _logicStack.Push(new QEquals(_logicStack.PopExpression(), _logicStack.PopExpression()));
        }


        /// <summary>
        /// Равно
        /// </summary>
        public void gt()
        {
            _logicStack.Push(new QGreatThen(_logicStack.PopExpression(), _logicStack.PopExpression()));
        }

        /// <summary>
        /// Равно
        /// </summary>
        public void gte()
        {
            _logicStack.Push(new QGreatThenOrEquals(_logicStack.PopExpression(), _logicStack.PopExpression()));
        }

        /// <summary>
        /// Равно
        /// </summary>
        public void lte()
        {
            _logicStack.Push(new QLessThenOrEquals(_logicStack.PopExpression(), _logicStack.PopExpression()));
        }

        /// <summary>
        /// Равно
        /// </summary>
        public void lt()
        {
            _logicStack.Push(new QLessThen(_logicStack.PopExpression(), _logicStack.PopExpression()));
        }


        /// <summary>
        /// Не равно
        /// </summary>
        public void ne()
        {
            _logicStack.Push(new QNotEquals(_logicStack.PopExpression(), _logicStack.PopExpression()));
        }

        /// <summary>
        /// Очистить стэк
        /// </summary>
        public void reset()
        {
            _logicStack.Clear();
            _scope.Clear();
        }


        /// <summary>
        /// Загрузить на стэк пустой аргумент (null для языка)
        /// </summary>
        public void ld_empty()
        {
            _logicStack.Push(null);
        }

        public void @case()
        {
            _logicStack.Push(new QCase(_logicStack.PopExpression(), _logicStack.PopItem<QWhenList>()));
        }

        public void when()
        {
            _logicStack.Push(new QWhen(_logicStack.PopExpression(),
                _logicStack.PopOpExpression()));
        }

        public void cast()
        {
            _logicStack.Push(new QCast(_logicStack.PopType(), _logicStack.PopExpression()));
        }

        public void ld_const(string str)
        {
            _logicStack.Push(new QConst(new SMType(SMType.String), str));
        }

        public void ld_const(double number)
        {
            _logicStack.Push(new QConst(new SMType(SMType.Numeric, 0, 10, 10), number));
        }
    }
}