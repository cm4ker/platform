using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aquila.Language.Ast.Definitions.Expressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StatementSyntax = Aquila.Language.Ast.Definitions.Statements.StatementSyntax;

namespace Aquila.Language.Ast.Definitions
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

        public T Pop<T>()
        {
            return (T) internalStack.Pop();
        }

        public List<T> PopList<T>()
        {
            return (List<T>) internalStack.Pop();
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

        public T ToCollection<T>() where T : SyntaxNode, new()
        {
            var collection = new T();
            collection.AddRange(internalStack.ToArray().Reverse().Cast<SyntaxNode>());
            return collection;
        }

        public object Peek()
        {
            return internalStack.Peek();
        }

        public SyntaxNode PeekNode()
        {
            return internalStack.Peek() as SyntaxNode;
        }

        public SyntaxNode PeekAst()
        {
            return internalStack.Peek() as SyntaxNode;
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

        public TypeSyntax PopType()
        {
            return (TypeSyntax) internalStack.Pop();
        }

        public SingleTypeSyntax PopSingleType()
        {
            return (SingleTypeSyntax) internalStack.Pop();
        }

        public BlockStatementSyntax PopInstructionsBody()
        {
            return (BlockStatementSyntax) internalStack.Pop();
        }

        public ExpressionSyntax PopExpression()
        {
            return (ExpressionSyntax) internalStack.Pop();
        }

        public NameSyntax PopName()
        {
            return (NameSyntax) internalStack.Pop();
        }

        public T PeekType<T>()
        {
            return Stack.OfType<T>().First();
        }

        public IList PeekCollection()
        {
            return (IList) internalStack.Peek();
        }

        public StatementSyntax PopStatement()
        {
            return (StatementSyntax) internalStack.Pop();
        }

        public AssignmentSyntax PopAssignment()
        {
            return (AssignmentSyntax) internalStack.Pop();
        }

        public IdentifierToken PopIdentifier()
        {
            return (IdentifierToken) internalStack.Pop();
        }
    }
}