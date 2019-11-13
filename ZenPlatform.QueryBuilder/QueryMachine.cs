using System.Collections.Generic;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder
{

    

    public class QueryMachine
    {

        private Stack<MachineContext> _contexts;
        private MachineContext CurrentContext => _contexts.TryPeek(out var res) ? res : null;
        private Stack<object> _syntaxStack = new Stack<object>();

        public QueryMachine()
        {
            _contexts = new Stack<MachineContext>();
            ct_query();
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
            switch (CurrentContext.Type)
            {
                case MachineContextType.select:
                    Push(new SField(columnName));
                    break;
            }

            ld_column();
            return this;
        }

        public QueryMachine ld_column()
        {
            switch (CurrentContext.Type)
            {
                case MachineContextType.select:
                    Push(new SSelectFieldExpression(Pop<SExpression>()));
                    break;

            }
            return this;

        }

        public QueryMachine @as(string name)
        {

            switch (CurrentContext.Type)
            {
                case MachineContextType.select:
                    if (TryPeek<SAliasedFieldExpression>(out _)) return this ;
                    Push(new SAliasedFieldExpression(Pop<SSelectFieldExpression>().Exp, name));
                    break;
            }
            return this;
        }

        #region Contexts

        private void ChangeContextType(MachineContextType type)
        {

            switch (CurrentContext.Type )
            {
                case
                    MachineContextType.select:

                    break;
            }

            CurrentContext.Type = type;


        }

        public QueryMachine m_select()
        {
            ChangeContextType(MachineContextType.select);

            return this;
        }

        public QueryMachine m_having()
        {
            ChangeContextType(MachineContextType.having);
            return this;
        }

        public QueryMachine m_from()
        {
            ChangeContextType(MachineContextType.from);
            return this;
        }

        public QueryMachine m_group_by()
        {
            ChangeContextType(MachineContextType.groupBy);
            return this;
        }

        public QueryMachine m_order_by()
        {
            ChangeContextType(MachineContextType.orderBy);
            return this;
        }

        #endregion

        public QueryMachine st_query()
        {
            

            ChangeContextType(MachineContextType.none);
            _contexts.Pop();
            return this;
        }

        public QueryMachine ct_query()
        {
            _contexts.Push(new MachineContext());
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