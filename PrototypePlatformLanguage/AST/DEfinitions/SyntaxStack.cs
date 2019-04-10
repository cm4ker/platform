using System.Collections;
using System.Linq;
using PrototypePlatformLanguage.AST.Definitions.Functions;
using PrototypePlatformLanguage.AST.Definitions.Statements;

namespace PrototypePlatformLanguage.AST.Definitions
{
    /// <summary>
    /// Стэк дерева синтаксиса
    /// </summary>
    public class SyntaxStack
    {
        private Stack internalStack = new Stack(100);

        public Stack Stack
        {
            get { return internalStack; }
        }

        public object Pop()
        {
            return internalStack.Pop();
        }

        public object Peek()
        {
            return internalStack.Peek();
        }

        public void Push(object value)
        {
            internalStack.Push(value);
        }

        public void Pop(int count)
        {
            while (count-- > 0)
                internalStack.Pop();
        }

        /// <summary>
        /// Удаляем конкретынй элемент на глубине. 0 - это верхушка стэка
        /// </summary>
        public void Remove(int depth)
        {
            Stack temp = new Stack();
            for (; depth > 0; depth--)
                temp.Push(internalStack.Pop());
            internalStack.Pop();
            while (temp.Count > 0)
                internalStack.Push(temp.Pop());
        }

        /// <summary>
        /// Очищение стэка выражений
        /// </summary>
        public void Clear()
        {
            internalStack.Clear();
        }

        public string PopString()
        {
            return (string) internalStack.Pop();
        }

        public Type PopType()
        {
            return (Type) internalStack.Pop();
        }

        public InstructionsBody PopMethodBody()
        {
            return (InstructionsBody) internalStack.Pop();
        }

        public TypeBody PopTypeBody()
        {
            return (TypeBody) internalStack.Pop();
        }

        public Infrastructure.Expression PopExpression()
        {
            return (Infrastructure.Expression) internalStack.Pop();
        }

        public T PeekType<T>()
        {
            return Stack.OfType<T>().First();
        }

        public IList PeekCollection()
        {
            return (IList) internalStack.Peek();
        }

        public Statement PopStatement()
        {
            return (Statement) internalStack.Pop();
        }

        public Assignment PopAssignment()
        {
            return (Assignment) internalStack.Pop();
        }
    }
}