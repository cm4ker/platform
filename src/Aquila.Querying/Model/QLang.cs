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

        public void group_by()
        {
            _logicStack.Push(new QGroupBy(_logicStack.PopItem<QGroupList>()));
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


        /// <summary>
        /// used for load subject then criterion build
        /// need pass CriterionContext parameter to constructor for using this feature
        /// </summary>
        public void ld_subject()
        {
            var ds = (QDataSource)top();

            if (ds != null)
            {
                _logicStack.Push(ds);
                if (CurrentScope != null)
                    CurrentScope.AddDS(ds);
            }
        }

        public void ld_table(string tableName)
        {
            var ds = _logicStack.PopDataSource();
            var table = ds.FindTable(tableName);

            _logicStack.Push(table);
            if (CurrentScope != null)
            {
                CurrentScope.AddDS(table);
                CurrentScope.RemoveDS(ds);
            }
        }

        public void ld_ref(QLangElement element)
        {
            _logicStack.Push(element);
        }


        /// <summary>
        /// Load some data source from current context
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="typeName"></param>
        /// <param name="alias"></param>
        public void ld_source(string qualifiedName)
        {
            //load entity type
            var type = _metadata.GetSemanticByName(qualifiedName);
            var ds = new QObjectTable(type);
            _logicStack.Push(ds);
            if (CurrentScope != null)
                CurrentScope.AddDS(ds);

            // if (string.IsNullOrEmpty(p_alias))
            // {
            //     //force aliasing the sources because we need it at adding security level
            //     p_alias = RandomString(10);
            // }
            //
            // @as(p_alias);

            //NOTE: Important create subject context after aliasing the source
            // if (_ust != null)
            // {
            //     if (!_ust.TryClaimPermission(type, SecPermission.Read, out var claim))
            //     {
            //         //access denied!
            //         // throw new Exception("Access denied");
            //     }
            //
            //     // /*
            //     //  Need register query
            //     //  
            //     //  SELECT ( CASE WHEN EXISTS(SELECT 1 FROM (QUERY)) THEN 0x01 (ALLOW) ELSE 0x00 (DENIED) END  ) SEC_FLAG
            //     //  
            //     //  FROM (SOURCE_TABLE)
            //     //
            //     //  NOTE: where SOURCE_TABLE - table subject                            
            //     // */
            //
            //     if (claim != null)
            //     {
            //         var qCriterial = claim.Criteria.SelectMany(x => x.Value).Select(x =>
            //         {
            //             var c = x.cString;
            //
            //             new_scope();
            //
            //             Parse(this, c);
            //             pop_scope();
            //
            //             return (QCriterion)pop();
            //         });
            //         CurrentScope.Criteria.AddRange(qCriterial);
            //     }
            // }
        }


        public void ld_param(string name)
        {
            _logicStack.Push(new QParameter(name));
        }

        public void ld_var(string name)
        {
            _logicStack.Push(new QVar(name));
        }

        public void ld_source_context()
        {
            _logicStack.Push(
                new QCombinedDataSource(new QDataSourceList(CurrentScope.GetScopedDS())));
        }

        public void ld_name(string name)
        {
            if (CurrentScope.TryGetDS(name, out var source))
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

        public void ld_star()
        {
            var fields = CurrentScope.GetScopedDS().SelectMany(x => x.GetFields());

            foreach (var field in fields)
            {
                _logicStack.Push(field);
                st_elem();
            }
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

            if (item is QDataSource aqds && aqds.Substituted)
            {
                CurrentScope.RemoveDS(aqds);
                CurrentScope.AddDS(aqds, alias);
                return;
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
        }

        /// <summary>
        /// creates new visibility scope for creating objects
        /// scope automatically close then you invoke closing-scope instruction
        /// like new_query()
        /// </summary>
        public void new_scope()
        {
            _scope.Push(new LogicScope());
        }

        private LogicScope pop_scope()
        {
            return _scope.Pop();
        }

        public void lookup(string propName)
        {
            _logicStack.Push(new QLookupField(propName, _logicStack.PopExpression()));
        }

        public void st_data_request()
        {
            pop_scope();
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
            var scope = pop_scope();

            var query = new QQuery(_logicStack.PopItem<QOrderBy>(),
                _logicStack.PopItem<QSelect>(), _logicStack.PopItem<QHaving>(),
                _logicStack.PopItem<QGroupBy>(), _logicStack.PopItem<QWhere>(),
                _logicStack.PopItem<QFrom>(), QCriterionList.Empty);

            //we need validate query before push it to the stack            

            _logicStack.Push(query);


            if (_scope.Count > 0) //we are inside the nested query
                _logicStack.Push(new QNestedQuery(_logicStack.PopQuery()));
        }

        public void new_criterion()
        {
            var scope = pop_scope();
            var criterion = new QCriterion(_logicStack.PopItem<QWhere>(), _logicStack.PopItem<QFrom>());

            //we need validate query before push it to the stack            

            _logicStack.Push(criterion);
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

        public T top<T>()
        {
            return (T)top();
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
        /// Great or equals then
        /// Great or equals then
        /// </summary>
        public void gte()
        {
            _logicStack.Push(new QGreatThenOrEquals(_logicStack.PopExpression(), _logicStack.PopExpression()));
        }

        /// <summary>
        /// Less or equals then
        /// </summary>
        public void lte()
        {
            _logicStack.Push(new QLessThenOrEquals(_logicStack.PopExpression(), _logicStack.PopExpression()));
        }

        /// <summary>
        /// Less then
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