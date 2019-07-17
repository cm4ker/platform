using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Language.Ast.AST.Definitions.Expressions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions
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

        public void PopUntil(object marker, IList reciver)
        {
            object element = internalStack.Pop();

            while (element != marker)
            {
                reciver.Add(element);
                element = internalStack.Pop();
            }
        }

        public List<object> PopUntil(object marker)
        {
            List<object> result = new List<object>();
            PopUntil(marker, result);
            return result;
        }

        public object Peek()
        {
            return internalStack.Peek();
        }

        public AstNode PeekAst()
        {
            return internalStack.Peek() as AstNode;
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

        public TypeNode PopType()
        {
            return (TypeNode) internalStack.Pop();
        }

        public SingleTypeNode PopSingleType()
        {
            return (SingleTypeNode) internalStack.Pop();
        }

        public BlockNode PopInstructionsBody()
        {
            return (BlockNode) internalStack.Pop();
        }


        public TypeBody PopTypeBody()
        {
            return (TypeBody) internalStack.Pop();
        }

        public Expression PopExpression()
        {
            return (Expression) internalStack.Pop();
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