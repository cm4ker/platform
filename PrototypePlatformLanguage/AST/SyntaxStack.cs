using System.Collections;
using System.Collections.Generic;

namespace PrototypePlatformLanguage.AST
{
    public class SyntaxStack
    {
        private Stack m_Stack = new Stack(100);

        public Stack Stack
        {
            get { return m_Stack; }
        }

        public object Pop()
        {
            return m_Stack.Pop();
        }

        public object Peek()
        {
            return m_Stack.Peek();
        }

        public void Push(object value)
        {
            m_Stack.Push(value);
        }

        public void Pop(int count)
        {
            while (count-- > 0)
                m_Stack.Pop();
        }

        /// <summary>
        /// Remove item at specified depth off the stack. 0 means top of stack.
        /// </summary>
        public void Remove(int depth)
        {
            Stack temp = new Stack();
            for (; depth > 0; depth--)
                temp.Push(m_Stack.Pop());
            m_Stack.Pop();
            while (temp.Count > 0)
                m_Stack.Push(temp.Pop());
        }

        public string PopString()
        {
            return (string) m_Stack.Pop();
        }

        public Type PopType()
        {
            return (Type) m_Stack.Pop();
        }

        public MethodBody PopMethodBody()
        {
            return (MethodBody) m_Stack.Pop();
        }

        public TypeBody PopTypeBody()
        {
            return (TypeBody) m_Stack.Pop();
        }

        public Expression PopExpression()
        {
            return (Expression) m_Stack.Pop();
        }

        public IList PeekCollection()
        {
            return (IList) m_Stack.Peek();
        }

        public Statement PopStatement()
        {
            return (Statement) m_Stack.Pop();
        }

        public Assignment PopAssignment()
        {
            return (Assignment) m_Stack.Pop();
        }
    }
}