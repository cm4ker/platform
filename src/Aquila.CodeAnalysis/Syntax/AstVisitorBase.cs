// using System;
// using System.CodeDom.Compiler;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Text;
//
// namespace Aquila.Syntax
// {
//     public abstract partial class AstVisitorBase<T>
//     {
//         private Stack<LangElement> _visitStack;
//         private bool _break;
//
//         protected Stack<LangElement> VisitStack => _visitStack;
//
//         public AstVisitorBase()
//         {
//             _visitStack = new Stack<LangElement>();
//         }
//
//         public virtual T Visit(LangElement visitable)
//         {
//             if (visitable is null) return default;
//
//             return visitable.Accept(this);
//         }
//
//         public virtual T DefaultVisit(LangElement node)
//         {
//             return default;
//         }
//
//         public virtual T VisitSyntaxToken(SyntaxToken obj)
//         {
//             return DefaultVisit(obj);
//         }
//     }
//
//
//     public abstract partial class AstVisitorBase
//     {
//         private Stack<LangElement> _visitStack;
//         private bool _break;
//
//         protected Stack<LangElement> VisitStack => _visitStack;
//
//         public AstVisitorBase()
//         {
//             _visitStack = new Stack<LangElement>();
//         }
//
//         public virtual void Visit(LangElement visitable)
//         {
//             visitable?.Accept(this);
//         }
//
//         public virtual void DefaultVisit(LangElement node)
//         {
//         }
//
//         public virtual void VisitSyntaxToken(SyntaxToken obj)
//         {
//             DefaultVisit(obj);
//         }
//     }
//
//
//     public class AstWalker<T> : AstVisitorBase<T>
//     {
//         public override T DefaultVisit(LangElement node)
//         {
//             Console.WriteLine($"We are visit: {node}");
//
//             foreach (var child in node.GetChildren())
//             {
//                 Visit(child);
//             }
//
//             return base.DefaultVisit(node);
//         }
//     }
//
//     public class AstWalker : AstVisitorBase
//     {
//         public override void DefaultVisit(LangElement node)
//         {
//             Console.WriteLine($"We are visit: {node}");
//
//             foreach (var child in node.GetChildren())
//             {
//                 Visit(child);
//             }
//
//             base.DefaultVisit(node);
//         }
//     }
//     
//     
//     public class AstPrinter
//     {
//         private IndentedTextWriter sw = new IndentedTextWriter(new StringWriter());
//         private List<char> _indentPrefix = new List<char>();
//
//         public static string Print(LangElement node)
//         {
//             return new AstPrinter().PrintCore(node);
//         }
//
//         private string PrintCore(LangElement node)
//         {
//             PrintNode(node, indent: "");
//             return sw.InnerWriter.ToString();
//         }
//
//         // Constants for drawing lines and spaces
//         private const string _cross = " ├─";
//         private const string _corner = " └─";
//         private const string _vertical = " │ ";
//         private const string _space = "   ";
//
//         void PrintNode(LangElement node, string indent)
//         {
//             sw.WriteLine(node.GetType().Name);
//
//             // Loop through the children recursively, passing in the
//             // indent, and the isLast parameter
//
//             var children = node.GetChildren().OrderBy(x=>x.GetChildren().Any()).ToArray();
//             var numberOfChildren = children.Count();
//
//             for (var i = 0; i < numberOfChildren; i++)
//             {
//                 var child = children[i];
//                 var isLast = (i == (numberOfChildren - 1));
//                 PrintChildNode(child, indent, isLast);
//             }
//         }
//
//         void PrintChildNode(LangElement node, string indent, bool isLast)
//         {
//             // Print the provided pipes/spaces indent
//             sw.Write(indent);
//
//             // Depending if this node is a last child, print the
//             // corner or cross, and calculate the indent that will
//             // be passed to its children
//             if (isLast)
//             {
//                 sw.Write(_corner);
//                 indent += _space;
//             }
//             else
//             {
//                 sw.Write(_cross);
//                 indent += _vertical;
//             }
//
//             PrintNode(node, indent);
//         }
//     }
// }