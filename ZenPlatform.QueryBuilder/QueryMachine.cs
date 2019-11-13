using System;
using System.Collections.Generic;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder
{

    

    public class QueryMachine: IObserver<MachineContextType>
    {

        private MachineContext _currentContext;

        private Stack<object> _syntaxStack = new Stack<object>();

        public QueryMachine()
        {
            _currentContext = new MachineContext();
        }


        private bool TryPop<T>(out T obj)
        {
            if (_syntaxStack.Count > 0)
                if (_syntaxStack.Peek() is T item)
                {
                    _syntaxStack.Pop();
                    obj = item;
                    return true;
                }

            obj = default(T);
            return false;
        }


        private bool TryPeek<T>(out T obj)
        {
            if (_syntaxStack.Count > 0)
                if (_syntaxStack.Peek() is T item)
                {
                    obj = item;
                    return true;
                }

            obj = default(T);
            return false;
        }

        private T Pop<T>()
        {
            return (T)_syntaxStack.Pop();
        }

        private void Push(object obj)
        {
            _syntaxStack.Push(obj);
        }

        public QueryMachine ld_table(string name)
        {
            Push(new STable(name));
            return this;
        }

        public QueryMachine ld_column(string columnName)
        {
            switch (_currentContext.Type)
            {
                case MachineContextType.Select:
                    Push(new SField(columnName));
                    break;
            }

            ld_column();
            return this;
        }

        public QueryMachine ld_column()
        {
            switch (_currentContext.Type)
            {
                case MachineContextType.Select:
                    Push(new SSelectFieldExpression(Pop<SExpression>()));
                    break;

            }
            return this;

        }

        public QueryMachine @as(string name)
        {

            switch (_currentContext.Type)
            {
                case MachineContextType.Select:
                    if (TryPeek<SAliasedFieldExpression>(out _)) return this ;
                    Push(new SAliasedFieldExpression(Pop<SSelectFieldExpression>().Exp, name));
                    break;
            }
            return this;
        }

        #region Contexts
        public void ChangeContextType(MachineContextType contextType)
        {

        }
        public QueryMachine m_select()
        {
            ChangeContextType(MachineContextType.Select);

            return this;
        }

        public QueryMachine m_having()
        {
            ChangeContextType(MachineContextType.Having);
            return this;
        }

        public QueryMachine m_from()
        {
            ChangeContextType(MachineContextType.From);
            return this;
        }

        public QueryMachine m_group_by()
        {
            ChangeContextType(MachineContextType.GroupBy);
            return this;
        }

        public QueryMachine m_order_by()
        {
            ChangeContextType(MachineContextType.OrderBy);
            return this;
        }

        #endregion

        public QueryMachine st_query()
        {

            _currentContext = Pop<MachineContext>();
            ChangeContextType(MachineContextType.None);
            return this;
        }

        public QueryMachine ct_query()
        {
            Push(_currentContext);

            _currentContext = new MachineContext();

            return this;
        }

        #region Comparers

        /// <summary>
        /// Great then
        /// </summary>
        public void gt()
        {
        }

        /// <summary>
        /// Less then
        /// </summary>
        public void lt()
        {
        }

        /// <summary>
        /// Great then or equals
        /// </summary>
        public void gte()
        {
        }

        /// <summary>
        /// Less then or equals
        /// </summary>
        public void lte()
        {
        }


        /// <summary>
        /// Not equals
        /// </summary>
        public void ne()
        {
        }

        #endregion

        public void on()
        {
        }

        #region Logical operators

        public void and()
        {
        }


        public void or()
        {
        }

        #endregion

        #region Arithmetic operations

        public void add()
        {
        }

        public void sub()
        {
        }

        #endregion

        #region Aggregate functions

        public void sum()
        {
        }

        public void avg()
        {
        }

        #endregion

        #region Joins

        public void @join()
        {
        }

        public void inner_join()
        {
        }

        public void left_join()
        {
        }

        public void right_join()
        {
        }

        public void full_join()
        {
        }

        public void cross_join()
        {
        }

        #endregion

        public object top()
        {
            return null;
        }


        public object pop()
        {
            return null;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(MachineContextType value)
        {
            throw new NotImplementedException();
        }

        /*
            m_from
                ld_table     (Schema) "T1"
                as "A"
                ld_table     (Schema) "T2"
                as "B"
                                
                ld_const     4
                ld_column    "A", "F1"
                on
                join
                ct_query
                    m_select
                        ld_const 1
                        ld_const 1
                        add
                        as "Summ"
                st_query
                ld_column "Summ"
                ld_column "A", "F1"
                on
                left_join
            m_where
                ld_column "A" , "F1"
                ld_column "B" , "F1"
                eq
                ld_column "A" , "F2"
                ld_column "B" , "F2"
                eq
                and
            m_group_by
                ld_column    "A", "F1"
            m_having
                ld_column    "A", "F1"
                sum
                ld_const     1000
                gt                
            m_select
                ld_column    "A", "F1"
                as "MyColumn"
         */
    }
}