using System.Collections.Generic;

namespace ZenPlatform.QueryBuilder
{
    public class QueryMachine
    {
        public Stack<object> _syntaxStack;

        public QueryMachine()
        {
            _syntaxStack = new Stack<object>();
        }

        public void ld_table()
        {
        }

        public void @as()
        {
        }

        #region Contexts

        public void m_select()
        {
        }

        public void m_having()
        {
        }

        public void m_from()
        {
        }

        public void m_group_by()
        {
        }

        public void m_order_by()
        {
        }

        #endregion

        public void st_query()
        {
        }

        public void ct_query()
        {
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