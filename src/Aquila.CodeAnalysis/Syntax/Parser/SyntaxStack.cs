// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Collections.Immutable;
// using System.Linq;
// using Aquila.Syntax.Ast;
//
// namespace Aquila.Syntax
// {
//     /// <summary>
//     /// Стэк дерева синтаксиса
//     /// </summary>
//     public class SyntaxStack
//     {
//         private Stack internalStack = new Stack(100);
//
//         public Stack Stack
//         {
//             get { return internalStack; }
//         }
//
//         public object Pop()
//         {
//             return internalStack.Pop();
//         }
//
//         public T Pop<T>()
//         {
//             return (T)internalStack.Pop();
//         }
//
//         public T TryPop<T>(Func<T> onError)
//         {
//             if (internalStack.Count == 0)
//                 if (onError != null)
//                     return onError();
//                 else
//                     return default;
//
//             return Pop<T>();
//         }
//
//         public List<T> PopList<T>()
//         {
//             return (List<T>)internalStack.Pop();
//         }
//
//
//         public void PopUntil(object marker, IList reciver)
//         {
//             object element = internalStack.Pop();
//
//             while (element != marker)
//             {
//                 reciver.Add(element);
//                 element = internalStack.Pop();
//             }
//         }
//
//         public List<object> PopUntil(object marker)
//         {
//             List<object> result = new List<object>();
//             PopUntil(marker, result);
//             return result;
//         }
//
//         // public T ToCollection<T>() where T : LangCollection, new()
//         // {
//         //     var collection = new T();
//         //     collection.AddRange(internalStack.ToArray().Reverse().Cast<LangElement>());
//         //     return collection;
//         // }
//
//         public ImmutableArray<T> GetElementsReverse<T>()
//         {
//             return internalStack.ToArray().Reverse().Cast<T>().ToImmutableArray();
//         }
//
//         public T ToCollection<T, TC>() where TC : LangElement where T : LangCollection<TC>
//         {
//             var elems = GetElementsReverse<TC>();
//             return (T)Activator.CreateInstance(typeof(T), elems);
//         }
//
//         public object Peek()
//         {
//             return internalStack.Peek();
//         }
//
//         public LangElement PeekNode()
//         {
//             if (internalStack.Count == 0)
//                 return default;
//
//             return internalStack.Peek() as LangElement;
//         }
//
//         public void Push(object value)
//         {
//             internalStack.Push(value);
//         }
//
//         public void Pop(int count)
//         {
//             while (count-- > 0)
//                 internalStack.Pop();
//         }
//
//         /// <summary>
//         /// Удаляем конкретынй элемент на глубине. 0 - это верхушка стэка
//         /// </summary>
//         public void Remove(int depth)
//         {
//             Stack temp = new Stack();
//             for (; depth > 0; depth--)
//                 temp.Push(internalStack.Pop());
//             internalStack.Pop();
//             while (temp.Count > 0)
//                 internalStack.Push(temp.Pop());
//         }
//
//         /// <summary>
//         /// Очищение стэка выражений
//         /// </summary>
//         public void Clear()
//         {
//             internalStack.Clear();
//         }
//
//         public string PopString()
//         {
//             return (string)internalStack.Pop();
//         }
//
//         public TypeRef PopType()
//         {
//             return (TypeRef)internalStack.Pop();
//         }
//
//
//         public BlockStmt PopInstructionsBody()
//         {
//             return (BlockStmt)internalStack.Pop();
//         }
//
//         public Expression PopExpression()
//         {
//             var item = internalStack.Pop();
//
//             if (item is IdentifierToken e)
//                 return new NameEx(e.Span, SyntaxKind.NameExpression, Operations.Empty, e, TypeList.Empty);
//
//             return (Expression)item;
//         }
//
//         public T PeekType<T>()
//         {
//             return Stack.OfType<T>().First();
//         }
//
//         public IList PeekCollection()
//         {
//             return (IList)internalStack.Peek();
//         }
//
//         public Statement PopStatement()
//         {
//             return (Statement)internalStack.Pop();
//         }
//
//         public AssignEx PopAssignment()
//         {
//             return (AssignEx)internalStack.Pop();
//         }
//
//         public IdentifierToken PopIdentifier()
//         {
//             return (IdentifierToken)internalStack.Pop();
//         }
//     }
// }