using System;
using System.Collections.Generic;

namespace ZenPlatform.QueryBuilder
{
    public enum QueryContext
    {
        None = 0,
        From = 1,
        GroupBy = 2,
        Having = 3,
        Where = 4,
        Select = 5,
    }
    
    public class SyntaxStack : Stack<object>
    {
    }

    public class StackMachineQueryBuilder
    {
        private SyntaxStack _logicStack;

        public StackMachineQueryBuilder()
        {
            _logicStack = new SyntaxStack();
        }


        #region Context modifiers

        private void ChangeContext(QueryContext nContext)
        {
            if (nContext < CurrentScope.QueryContext && nContext != QueryContext.None)
                throw new Exception($"Can't change context {CurrentScope.QueryContext} to {nContext}");

            if (CurrentScope.QueryContext == QueryContext.From)
                m_from_close();
            else if (CurrentScope.QueryContext == QueryContext.Where)
                m_where_close();
            else if (CurrentScope.QueryContext == QueryContext.Select)
                m_select_close();

            CurrentScope.QueryContext = nContext;
        }

        public void m_from()
        {
            ChangeContext(QueryContext.From);
        }

        public void m_from_close()
        {
            _logicStack.Push(new QFrom(_logicStack.PopItems<QFromItem>(), _logicStack.PopDataSource()));
        }

        public void m_select_close()
        {
            _logicStack.Push(new QSelect(_logicStack.PopFields()));
        }

        public void m_where_close()
        {
            _logicStack.Push(new QWhere(_logicStack.PopExpression()));
        }

        public void m_select()
        {
            ChangeContext(QueryContext.Select);
        }

        public void m_where()
        {
            ChangeContext(QueryContext.Where);
        }

        public void m_group_by()
        {
            ChangeContext(QueryContext.GroupBy);
        }

        public void m_having()
        {
            ChangeContext(QueryContext.Having);
        }

        #endregion

        #region Load instructions

        public void ld_component(string componentName)
        {
            var component = _conf.Data.Components.First(x => x.Info.ComponentName == componentName);
            _logicStack.Push(component);
        }

        public void ld_type(string typeName)
        {
            var type = _logicStack.PopComponent().GetTypeByName(typeName);
            var ds = new QObjectTable(type);
            _logicStack.Push(ds);
            CurrentScope.ScopedDataSources.Add(ds);
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
            ld_type(typeName);

            if (!string.IsNullOrEmpty(p_alias))
            {
                alias(p_alias);
            }
        }

        public void ld_param(string name)
        {
            _logicStack.Push(new QParameter(name));
        }

        public void ld_source_context()
        {
            _logicStack.Push(new QCombinedDataSource(CurrentScope.ScopedDataSources));
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

        #endregion

        public void alias(string alias)
        {
            if (CurrentScope.QueryContext == QueryContext.From)
            {
                var source = _logicStack.PopDataSource();
                var ds = new QAliasedDataSource(source, alias);
                _logicStack.Push(ds);
                CurrentScope.Scope.Add(alias, ds);
            }
            else if (CurrentScope.QueryContext == QueryContext.Select)
            {
                var expr = _logicStack.PopExpression();
                _logicStack.Push(new QAliasedSelectExpression(expr, alias));
            }
        }

        public void begin_query()
        {
            _scope.Push(new LogicScope());
        }

        public void begin_data_request()
        {
            _scope.Push(new LogicScope());
        }

        public void lookup(string propName)
        {
            _logicStack.Push(new QLookupField(propName, _logicStack.PopExpression()));
        }

        public void st_data_request()
        {
            if (CurrentScope.QueryContext != QueryContext.None)
                throw new Exception(
                    $"You can save query only in the 'Select' context. Current context {CurrentScope.QueryContext}");
            _scope.Pop();

            _logicStack.Push(new QDataRequest(_logicStack.PopItems<QField>()));
        }

        public void st_query()
        {
            if (CurrentScope.QueryContext != QueryContext.Select)
                throw new Exception(
                    $"You can save query only in the 'Select' context. Current context {CurrentScope.QueryContext}");

            ChangeContext(QueryContext.None);

            _scope.Pop();

            _logicStack.Push(new QQuery(_logicStack.PopItem<QOrderBy>(),
                _logicStack.PopItem<QSelect>(), _logicStack.PopItem<QHaving>(),
                _logicStack.PopItem<QGroupBy>(), _logicStack.PopItem<QWhere>(),
                _logicStack.PopFrom()));

            if (_scope.Count > 0) //мы находимся во внутреннем запросе
                _logicStack.Push(new QNestedQuery(_logicStack.PopQuery()));
        }

        public void on()
        {
            _logicStack.Push(new QOn(_logicStack.PopExpression()));
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
            _logicStack.Push(new QFromItem(_logicStack.PopOn(), _logicStack.PopDataSource(), type));
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
            _logicStack.Push(new QCase(_logicStack.PopItems<QCaseWhen>()));
        }

        public void case_when()
        {
            _logicStack.Push(new QCaseWhen(_logicStack.PopExpression(), _logicStack.PopExpression(),
                _logicStack.PopOpExpression()));
        }


        public void ld_const(string str)
        {
            _logicStack.Push(new QConst(new XCString(), str));
        }

        public void ld_const(double number)
        {
            _logicStack.Push(new QConst(new XCNumeric(), number));
        }
    }
}