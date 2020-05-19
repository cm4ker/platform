using System;
using System.Collections.Generic;
using Aquila.QueryBuilder.Model;

namespace Aquila.QueryBuilder
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
                pop();
                return true;
            }

            return false;
        }


        private bool TryPeek<T>(out T obj)
        {
            if (_syntaxStack.Count > 0)
                if (peek() is T item)
                {
                    obj = item;
                    return true;
                }

            obj = default(T);
            return false;
        }


        private T Pop<T>()
        {
            return (T) pop();
        }

        private List<T> TryPopList<T>(int count = 0)
        {
            List<T> result = new List<T>();
            int i = 0;
            while ((++i <= count && count > 0 || count == 0) && TryPop(out T item))
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

            return item;
        }

        public void push(object obj)
        {
            _syntaxStack.Push(obj);
        }

        public object peek()
        {
            return _syntaxStack.Peek();
        }

        public object pop()
        {
            return _syntaxStack.Pop();
        }

        public void dup()
        {
            push(_syntaxStack.Peek());
        }

        #endregion


        #region Other

        public QueryMachine reset()
        {
            _syntaxStack.Clear();
            _currentContext = null;
            return this;
        }

        public QueryMachine ld_table(string name)
        {
            push(new STable(name));
            return this;
        }

        public QueryMachine ld_column()
        {
            push(new SField(Pop<string>(), Pop<string>()));
            return this;
        }

        public QueryMachine ld_column(string columnName, string tableName = null)
        {
            ld_str(tableName);
            ld_str(columnName);

            ld_column();
            return this;
        }

        public QueryMachine assign()
        {
            push(new SAssign(Pop<SExpression>(), Pop<SField>()));
            return this;
        }

        public QueryMachine ld_param(string name)
        {
            push(new SParameter(name));
            return this;
        }

        public QueryMachine @as(string name)
        {
            switch (pop())
            {
                case SExpression exp:
                    push(new SAliasedExpression(exp, name));
                    break;
                case SDataSource source:
                    push(new SAliasedDataSource(source, name));
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
            push(new SConstant(value));
            return this;
        }

        public QueryMachine mark()
        {
            push(new SMarker());
            return this;
        }

        public QueryMachine coalese()
        {
            push(new SCoalese(PopList<SExpression>()));
            return this;
        }

        public QueryMachine @case()
        {
            push(new SCase(TryPop<SExpression>(), PopList<SWhen>()));
            return this;
        }

        public QueryMachine when()
        {
            push(new SWhen(Pop<SCondition>(), Pop<SExpression>()));
            return this;
        }

        public QueryMachine ld_type(string type)
        {
            SQLTypeBuilder tb = new SQLTypeBuilder();
            push(tb.Parse(type));
            return this;
        }

        public QueryMachine cast()
        {
            push(new SCast(Pop<ColumnType>(), Pop<SExpression>()));
            return this;
        }

        #endregion

        #region Contexts

        private void ChangeContextType(MachineContextType contextType)
        {
            if (_currentContext == null)
                throw new Exception("You need begin new query. Now the context is null");

            switch (_currentContext.Type)
            {
                case MachineContextType.Select:

                    push(new SSelect(
                        TryPopList<SExpression>(),
                        TryPop<SOrderBy>(),
                        TryPop<STop>(),
                        TryPop<SHaving>(),
                        TryPop<SGroupBy>(),
                        TryPop<SWhere>(),
                        TryPop<SFrom>()
                    ));
                    break;
                case MachineContextType.From:
                    push(new SFrom(TryPopList<SJoin>(), TryPop<SDataSource>()));
                    break;
                case MachineContextType.Where:
                    push(new SWhere(Pop<SCondition>()));
                    break;
                case MachineContextType.Having:
                    push(new SHaving(Pop<SCondition>()));
                    break;
                case MachineContextType.GroupBy:
                    push(new SGroupBy(PopList<SExpression>()));
                    break;
                case MachineContextType.Insert:
                    push(new SInsert(TryPopList<SField>(), Pop<STable>(), Pop<SDataSource>()));
                    break;
                case MachineContextType.Values:
                    push(new SValuesSource(PopList<SExpression>()));
                    break;
                case MachineContextType.Set:
                    push(new SSet(PopList<SAssign>()));
                    break;
                case MachineContextType.Update:
                    push(new SUpdate(Pop<SDataSource>(), Pop<SSet>(), TryPop<SWhere>(), TryPop<SFrom>()));
                    break;
                case MachineContextType.OrderBy:
                    push(new SOrderBy(TryPop<OrderDirection>(), PopList<SExpression>()));
                    break;
                case MachineContextType.Delete:
                    push(new SDelete(
                        TryPop<SWhere>(),
                        Pop<SFrom>()));
                    break;
            }

            _currentContext.Type = contextType;
        }

        public QueryMachine m_select()
        {
            ChangeContextType(MachineContextType.Select);

            return this;
        }

        public QueryMachine m_delete()
        {
            ChangeContextType(MachineContextType.Delete);
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

            push(result);

            return this;
        }

        public QueryMachine bg_query()
        {
            if (_currentContext != null)
                push(_currentContext);

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
            push(new SGreatThen(Pop<SExpression>(), Pop<SExpression>()));
            return this;
        }

        /// <summary>
        /// Less then
        /// </summary>
        public QueryMachine lt()
        {
            push(new SLessThen(Pop<SExpression>(), Pop<SExpression>()));
            return this;
        }

        /// <summary>
        /// Great then or equals
        /// </summary>
        public QueryMachine gte()
        {
            push(new SGreatThenOrEquals(Pop<SExpression>(), Pop<SExpression>()));
            return this;
        }

        /// <summary>
        /// Less then or equals
        /// </summary>
        public QueryMachine lte()
        {
            push(new SLessThenOrEquals(Pop<SExpression>(), Pop<SExpression>()));
            return this;
        }


        /// <summary>
        /// Not equals
        /// </summary>
        public QueryMachine ne()
        {
            push(new SNotEquals(Pop<SExpression>(), Pop<SExpression>()));
            return this;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public QueryMachine eq()
        {
            push(new SEquals(Pop<SExpression>(), Pop<SExpression>()));
            return this;
        }

        #endregion

        #region Logical operators

        public QueryMachine and()
        {
            push(new SAnd(PopList<SExpression>()));
            return this;
        }


        public void or()
        {
            push(new SOr(PopList<SExpression>()));
        }

        #endregion

        #region Arithmetic operations

        public QueryMachine madd()
        {
            push(new SAdd(PopList<SExpression>()));
            return this;
        }

        public QueryMachine msub()
        {
            push(new SSub(PopList<SExpression>()));
            return this;
        }

        public QueryMachine add()
        {
            push(new SAdd(PopList<SExpression>(2)));
            return this;
        }

        public QueryMachine sub()
        {
            push(new SSub(PopList<SExpression>(2)));
            return this;
        }

        #endregion

        #region Aggregate functions

        public QueryMachine sum()
        {
            push(new SSum(Pop<SExpression>()));


            return this;
        }

        public QueryMachine avg()
        {
            push(new SAvg(Pop<SExpression>()));

            return this;
        }

        public QueryMachine count()
        {
            push(new SCount(Pop<SExpression>()));

            return this;
        }

        #endregion

        #region Joins

        private void join_with_type(JoinType joinType)
        {
            push(new SJoin(Pop<SCondition>(), Pop<SDataSource>(), joinType));
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

        #region Premitives

        public QueryMachine ld_str(string arg)
        {
            push(arg);
            return this;
        }

        public QueryMachine ld_null()
        {
            push(new SNull());
            return this;
        }

        public QueryMachine desc()
        {
            push(OrderDirection.DESC);
            return this;
        }

        public QueryMachine asc()
        {
            push(OrderDirection.ASC);
            return this;
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