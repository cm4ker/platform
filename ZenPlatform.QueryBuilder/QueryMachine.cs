using System;
using System.Collections.Generic;
using ZenPlatform.QueryBuilder.Model;

namespace ZenPlatform.QueryBuilder
{
    public class QueryMachine
    {
        private MachineContext _currentContext;

        private Stack<object> _syntaxStack = new Stack<object>();

        public QueryMachine()
        {
            // _currentContext = new MachineContext();
            ///ct_query();
        }


        #region Stack function

        private bool TryPop<T>(out T obj)
        {
            if (TryPeek(out obj))
            {
                Pop();
                return true;
            }
            return false;
        }


        private bool TryPeek<T>(out T obj)
        {
            if (_syntaxStack.Count > 0)
                if (Peek() is T item)
                {
                    obj = item;
                    return true;
                }

            obj = default(T);
            return false;
        }

        public object Peek()
        {
            return _syntaxStack.Peek();
        }

        private T Pop<T>()
        {
            return (T)Pop();
        }

        private List<T> TryPopList<T>(int count = 0)
        {
            List<T> result = new List<T>();
            int i = 0;
            while (TryPop(out T item) && ++i<count && count>0)
            {
                result.Add(item);
                
            }

            TryPop<SMarker>();

            return result;
        }

        private List<T> PopList<T>(int count = 0)
        {
            List<T> result = TryPopList<T>(count);

            if (result.Count > 0)
                return result;
            else throw new InvalidOperationException($"Stack must be contain at least one type of '{typeof(T)}'");
        }

        private T TryPop<T>()
        {
            if (TryPeek(out T item))
            {
                _syntaxStack.Pop();
            }
        
            return  item; 
        }

        private void Push(object obj)
        {
            _syntaxStack.Push(obj);
        }


        public object Pop()
        {
            return _syntaxStack.Pop();
        }

        #endregion


        #region Other

        public QueryMachine ld_table(string name)
        {
            Push(new STable(name));
            return this;
        }

        public QueryMachine ld_column(string columnName, string tableName = null)
        {
            Push(new SField(columnName, tableName));
            return this;
        }

        public QueryMachine ld_param(string name)
        {
            Push(new SParameter(name));
            return this;
        }

        public QueryMachine @as(string name)
        {
            switch (Pop())
            {
                case SExpression exp:
                    Push(new SAliasedExpression(exp, name));
                    break;
                case SDataSource source:
                    Push(new SAliasedDataSource(source, name));
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return this;
        }

        public QueryMachine limit(int limint, int offset)
        {
            switch (_currentContext.Type)
            {
                case MachineContextType.Select:
                    break;
            }
                    return this;
        }

        public QueryMachine ld_const(object value)
        {
            Push(new SConstant(value));
            return this;
        }

        public QueryMachine mark()
        {
            Push(new SMarker());
            return this;
        }

        public QueryMachine coalese()
        {
            Push(new SCoalese(PopList<SExpression>()));
            return this;
        }

        public QueryMachine @case()
        {
            Push(new SCase(TryPop<SExpression>(),PopList<SWhen>()));
            return this;
        }

        public QueryMachine when()
        {
            Push(new SWhen(Pop<SCondition>(),Pop<SExpression>()));
            return this;
        }

        #endregion

        #region Contexts

        private void ChangeContextType(MachineContextType contextType)
        {
            switch (_currentContext.Type)
            {
                case MachineContextType.Select:

                    Push(new SSelect(
                        TryPopList<SExpression>(),
                        TryPop<STop>(),
                        TryPop<SHaving>(),
                        TryPop<SOrderBy>(),
                        TryPop<SGroupBy>(),
                        TryPop<SWhere>(),
                        TryPop<SFrom>()
                    ));
                    break;
                case MachineContextType.From:
                    Push(new SFrom(TryPopList<SJoin>(), TryPop<SDataSource>()));
                    break;
                case MachineContextType.Where:
                    Push(new SWhere(Pop<SCondition>()));
                    break;
                case MachineContextType.Having:
                    Push(new SHaving(Pop<SCondition>()));
                    break;
                case MachineContextType.GroupBy:
                    Push(new SGroupBy(PopList<SExpression>()));
                    break;
                case MachineContextType.Insert:
                    Push(new SInsert(TryPopList<SField>(), Pop<STable>(), Pop<SDataSource>()));
                    break;
                case MachineContextType.Values:
                    Push(new SValuesSource(PopList<SExpression>()));
                    break;
                case MachineContextType.Set:
                    List<SSetItem> items = new List<SSetItem>();
                    while (TryPeek(out SExpression exp))
                    {
                        Pop();
                        items.Add(new SSetItem(Pop<SField>(), exp));
                    }

                    Push(new SSet(items));
                    break;
                case MachineContextType.Update:

                    Push(new SUpdate(Pop<SDataSource>(), Pop<SSet>(), TryPop<SWhere>(), TryPop<SFrom>()));
                    break;
            }
            _currentContext.Type = contextType;
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

        public QueryMachine m_where()
        {
            ChangeContextType(MachineContextType.Where);
            return this;
        }

        public QueryMachine m_insert()
        {
            ChangeContextType(MachineContextType.Insert);
            return this;
        }

        public QueryMachine m_values()
        {
            ChangeContextType(MachineContextType.Values);
            return this;
        }

        public QueryMachine m_update()
        {
            ChangeContextType(MachineContextType.Update);
            return this;
        }

        public QueryMachine m_set()
        {
            ChangeContextType(MachineContextType.Set);
            return this;
        }

        #endregion

        #region Query

        public QueryMachine st_query()
        {
            ChangeContextType(MachineContextType.None);

            var result = Pop<object>();
            _currentContext = TryPop<MachineContext>();

            if (_currentContext != null && result is SSelect select)
                result = new SDataSourceNestedQuery(select);

            Push(result);

            return this;
        }

        public QueryMachine ct_query()
        {
            if (_currentContext != null)
                Push(_currentContext);

            _currentContext = new MachineContext();

            return this;
        }

        #endregion

        #region Comparers

        /// <summary>
        /// Great then
        /// </summary>
        public QueryMachine gt()
        {
            Push(new SGreatThen(Pop<SExpression>(), Pop<SExpression>()));
            return this;
        }

        /// <summary>
        /// Less then
        /// </summary>
        public QueryMachine lt()
        {
            Push(new SLessThen(Pop<SExpression>(), Pop<SExpression>()));
            return this;
        }

        /// <summary>
        /// Great then or equals
        /// </summary>
        public QueryMachine gte()
        {
            Push(new SGreatThenOrEquals(Pop<SExpression>(), Pop<SExpression>()));
            return this;
        }

        /// <summary>
        /// Less then or equals
        /// </summary>
        public QueryMachine lte()
        {
            Push(new SLessThenOrEquals(Pop<SExpression>(), Pop<SExpression>()));
            return this;
        }


        /// <summary>
        /// Not equals
        /// </summary>
        public QueryMachine ne()
        {
            Push(new SNotEquals(Pop<SExpression>(), Pop<SExpression>()));
            return this;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public QueryMachine eq()
        {
            Push(new SEquals(Pop<SExpression>(), Pop<SExpression>()));
            return this;
        }

        #endregion

        #region Logical operators

        public QueryMachine and()
        {
            Push(new SAnd(PopList<SExpression>()));
            return this;
        }


        public void or()
        {
            Push(new SOr(PopList<SExpression>()));
        }

        #endregion

        #region Arithmetic operations

        public QueryMachine madd()
        {
            Push(new SAdd(PopList<SExpression>()));
            return this;
        }

        public QueryMachine msub()
        {
            Push(new SSub(PopList<SExpression>()));
            return this;
        }

        public QueryMachine add()
        {
            Push(new SAdd(PopList<SExpression>(2)));
            return this;
        }

        public QueryMachine sub()
        {
            Push(new SSub(PopList<SExpression>(2)));
            return this;
        }

        #endregion

        #region Aggregate functions

        public QueryMachine sum()
        {
            Push(new SSum(Pop<SExpression>()));


            return this;
        }

        public QueryMachine avg()
        {
            Push(new SAvg(Pop<SExpression>()));

            return this;
        }

        public QueryMachine count()
        {
            Push(new SCount(Pop<SExpression>()));

            return this;
        }

        #endregion

        #region Joins

        private void join_with_type(JoinType joinType)
        {
            Push(new SJoin(Pop<SCondition>(), Pop<SDataSource>(), joinType));
        }

        public QueryMachine @join()
        {
            inner_join();
            return this;
        }

        public QueryMachine inner_join()
        {
            join_with_type(JoinType.Inner);
            return this;
        }

        public QueryMachine left_join()
        {
            join_with_type(JoinType.Left);
            return this;
        }

        public QueryMachine right_join()
        {
            join_with_type(JoinType.Right);
            return this;
        }

        public QueryMachine full_join()
        {
            throw new NotSupportedException();
        }

        public QueryMachine cross_join()
        {
            throw new NotSupportedException();
        }

        #endregion



        /* DML
            m_from
                ld_table     (Schema) "T1"
                as "A"
                ld_table     (Schema) "T2"
                as "B"
                                
                ld_const     4
                ld_column    "A", "F1"
                eq
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