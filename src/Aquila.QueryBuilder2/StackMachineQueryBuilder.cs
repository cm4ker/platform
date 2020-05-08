using System;
using System.Collections.Generic;
using Aquila.QueryBuilder.DML.Select;
using Aquila.QueryBuilder.Model;
using SelectNode = Aquila.QueryBuilder.Model.SelectNode;

namespace Aquila.QueryBuilder
{
    public class SyntaxScope
    {
        public SyntaxScope()
        {
        }

        public QueryContext QueryContext;
    }

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
        private Stack<SyntaxScope> _scope;


        public StackMachineQueryBuilder()
        {
            _logicStack = new SyntaxStack();
        }


        public SyntaxScope CurrentScope => _scope.TryPeek(out var res) ? res : null;

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
            _logicStack.Push(new FromNode() { });
        }

        public void m_select_close()
        {
            _logicStack.Push(new SelectNode());
        }

        public void m_where_close()
        {
            _logicStack.Push(new WhereNode());
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

        public void param(string name)
        {
        }

        public void @as(string alias)
        {
            if (CurrentScope.QueryContext == QueryContext.From)
            {
            }
            else if (CurrentScope.QueryContext == QueryContext.Select)
            {
            }
        }

        public void begin_query()
        {
            _scope.Push(new SyntaxScope());
        }

        public void begin_data_request()
        {
            _scope.Push(new SyntaxScope());
        }

        public void st_query()
        {
            if (CurrentScope.QueryContext != QueryContext.Select)
                throw new Exception(
                    $"You can save query only in the 'Select' context. Current context {CurrentScope.QueryContext}");

            ChangeContext(QueryContext.None);

            _scope.Pop();

            _logicStack.Push(new SelectNode());

            if (_scope.Count > 0) //мы находимся во внутреннем запросе
                _logicStack.Push(new SelectNastedQueryNode((SelectQueryNode) _logicStack.Pop()));
        }

        public void on()
        {
        }

        /// <summary>
        /// Внутреннее соединение 
        /// </summary>
        public void join()
        {
            join_with_type(JoinType.Inner);
        }

        /// <summary>
        /// Левое соединение
        /// </summary>
        public void left_join()
        {
            join_with_type(JoinType.Left);
        }

        /// <summary>
        /// Правое соединение
        /// </summary>
        public void right_join()
        {
            join_with_type(JoinType.Right);
        }

        public void cross_join()
        {
            join_with_type(JoinType.Cross);
        }

        /// <summary>
        /// Полное соединение
        /// </summary>
        public void full_join()
        {
            join_with_type(JoinType.Full);
        }

        private void join_with_type(JoinType type)
        {
            //_logicStack.Push();
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
        }

        /// <summary>
        /// Конкатенация
        /// </summary>
        public void add()
        {
        }

        /// <summary>
        /// Равно
        /// </summary>
        public void eq()
        {
        }


        /// <summary>
        /// Равно
        /// </summary>
        public void gt()
        {
        }

        /// <summary>
        /// Равно
        /// </summary>
        public void gte()
        {
        }

        /// <summary>
        /// Равно
        /// </summary>
        public void lte()
        {
        }

        /// <summary>
        /// Равно
        /// </summary>
        public void lt()
        {
        }


        /// <summary>
        /// Не равно
        /// </summary>
        public void ne()
        {
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
        }

        public void case_when()
        {
        }


        public void ld_const(string str)
        {
        }

        public void ld_const(double number)
        {
        }
    }
}