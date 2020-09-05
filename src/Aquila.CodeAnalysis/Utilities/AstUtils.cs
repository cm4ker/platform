﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using System.Collections.Immutable;
using System.Net.Mime;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Shared.Tree;
using Aquila.Syntax;
using Aquila.Syntax.Ast;
using Aquila.Syntax.Ast.Functions;
using Aquila.Syntax.Syntax;
using Aquila.Syntax.Text;

namespace Aquila.CodeAnalysis
{
    public static class AstUtils
    {
        internal static Operations CompoundOpToBinaryOp(Operations op)
        {
            switch (op)
            {
                case Operations.AssignAdd: return Operations.Add;
                case Operations.AssignAnd: return Operations.BitAnd;
                case Operations.AssignAppend: return Operations.Concat;
                case Operations.AssignDiv: return Operations.Div;
                case Operations.AssignMod: return Operations.Mod;
                case Operations.AssignMul: return Operations.Mul;
                case Operations.AssignOr: return Operations.BitOr;
                case Operations.AssignPow: return Operations.Pow;
                case Operations.AssignPrepend: return Operations.Concat;
                case Operations.AssignShiftLeft: return Operations.ShiftLeft;
                case Operations.AssignShiftRight: return Operations.ShiftRight;
                case Operations.AssignSub: return Operations.Sub;
                case Operations.AssignXor: return Operations.BitXor;
                case Operations.AssignCoalesce: return Operations.Coalesce;
                default:
                    throw Roslyn.Utilities.ExceptionUtilities.UnexpectedValue(op);
            }
        }

        internal static Operations BinaryToCompoundOp(Operations op)
        {
            switch (op)
            {
                case Operations.Add: return Operations.AssignAdd;
                case Operations.BitAnd: return Operations.AssignAnd;
                case Operations.Concat: return Operations.AssignAppend;
                case Operations.Div: return Operations.AssignDiv;
                case Operations.Mod: return Operations.AssignMod;
                case Operations.Mul: return Operations.AssignMul;
                case Operations.BitOr: return Operations.AssignOr;
                case Operations.Pow: return Operations.AssignPow;
                case Operations.ShiftLeft: return Operations.AssignShiftLeft;
                case Operations.ShiftRight: return Operations.AssignShiftRight;
                case Operations.Sub: return Operations.AssignSub;
                case Operations.BitXor: return Operations.AssignXor;
                case Operations.Coalesce: return Operations.AssignCoalesce;
                default:
                    throw Roslyn.Utilities.ExceptionUtilities.UnexpectedValue(op);
            }
        }

        // /// <summary>
        // /// Fixes <see cref="ItemUse"/> so it propagates correctly through our visitor.
        // /// </summary>
        // /// <remarks><c>IsMemberOf</c> will be set on Array, not ItemUse itself.</remarks>
        // public static void PatchItemUse(ItemUse item)
        // {
        //     if (item.IsMemberOf != null)
        //     {
        //         var varlike = item.Array as VarLikeConstructUse;
        //
        //         Debug.Assert(varlike != null);
        //         Debug.Assert(varlike.IsMemberOf == null);
        //
        //         // fix this ast weirdness:
        //         varlike.IsMemberOf = item.IsMemberOf;
        //         item.IsMemberOf = null;
        //     }
        // }

        // /// <summary>
        // /// Creates new struct with updated <see cref="CompleteToken.TokenText"/>.
        // /// </summary>
        // public static CompleteToken WithTokenText(this CompleteToken t, string text)
        // {
        //     return new CompleteToken(t.Token, t.TokenValue, t.TokenPosition, text);
        // }

        // /// <summary>
        // /// Creates new struct with updated <see cref="CompleteToken.TokenText"/>.
        // /// </summary>
        // public static CompleteToken WithToken(this CompleteToken t, Tokens token)
        // {
        //     return new CompleteToken(token, t.TokenValue, t.TokenPosition, t.TokenText);
        // }

        // /// <summary>
        // /// Gets value indicating the token is an ignored token - whitespace or comment.
        // /// </summary>
        // public static bool IsWhitespace(this CompleteToken t) =>
        //     t.Token == Tokens.T_WHITESPACE || t.Token == Tokens.T_COMMENT; // not T_DOC_COMMENT

        // /// <summary>
        // /// Gets attributes associated with given syntax node.
        // /// </summary>
        // public static bool TryGetCustomAttributes(this Node element, out ImmutableArray<AttributeData> attrs)
        // {
        //     return element.TryGetProperty(out attrs);
        // }

        // /// <summary>
        // /// Associates an attribute with syntax node.
        // /// </summary>
        // public static void AddCustomAttribute(this AstNode element, AttributeData attribute)
        // {
        //     Debug.Assert(attribute != null);
        //
        //     var newattrs = TryGetCustomAttributes(element, out var attrs)
        //         ? attrs.Add(attribute)
        //         : ImmutableArray.Create(attribute);
        //
        //     element.SetProperty(newattrs);
        // }

        /// <summary>
        /// Determines whether method has <c>$this</c> variable.
        /// </summary>
        public static bool HasThisVariable(MethodDecl method)
        {
            return false;
            //return method != null && (method.Modifiers & PhpMemberAttributes.Static) == 0;
        }

        public static Span BodySpanOrInvalid(this Node routine)
        {
            // if (routine is MethodDecl f)
            // {
            //     return f.Block.Span;
            // }

            if (routine is MethodDecl m)
            {
                return m.Block?.Span ?? Span.Invalid;
            }

            // if (routine is LambdaFunctionExpr lambda)
            // {
            //     var body = (ILangElement) lambda.Expression ?? lambda.Body;
            //     return body.Span;
            // }
            else
            {
                return Span.Invalid;
            }
        }

        /// <summary>
        /// Gets <see cref="Microsoft.CodeAnalysis.Text.LinePosition"/> from source position.
        /// </summary>
        public static LinePosition LinePosition(this ILineBreaks lines, int pos)
        {
            lines.GetLineColumnFromPosition(pos, out int line, out int col);

            // https://github.com/dotnet/corefx/blob/master/src/System.Reflection.Metadata/specs/PortablePdb-Metadata.md#sequence-points-blob - column must be less than 0x10000
            return new LinePosition(line, Math.Min(col, 0x09999));
        }

        // /// <summary>
        // /// Determines whether to treat given PHAR entry as a PHP source file (whether to compile it).
        // /// </summary>
        // public static bool IsCompileEntry(this Entry entry)
        // {
        //     // TODO: what entries will be compiled?
        //     if (entry.IsDirectory)
        //     {
        //         return false;
        //     }
        //
        //     if (entry.Name.EndsWith(".php"))
        //     {
        //         return true;
        //     }
        //
        //     if (string.IsNullOrEmpty(entry.Code))
        //     {
        //         return false;
        //     }
        //
        //     var ext = System.IO.Path.GetExtension(entry.Name);
        //     if (string.IsNullOrEmpty(ext) && entry.Code.StartsWith("<?php"))
        //     {
        //         return true;
        //     }
        //
        //     if (ext == ".php5" || ext == ".inc" || ext == ".module")
        //     {
        //         return entry.Code.IndexOf("<?php") >= 0;
        //     }
        //
        //     return false;
        // }

        /// <summary>
        /// Returns the offset of the location specified by (zero-based) line and character from the start of the file.
        /// In the case of invalid line, -1 is returned.
        /// </summary>
        public static int GetOffset(this PhpSyntaxTree tree, LinePosition linePosition)
        {
            throw new NotImplementedException();
            
            // if (linePosition.Line < 0 || linePosition.Line > tree.Source.LineBreaks.Count)
            // {
            //     return -1;
            // }
            //
            // int lineStart = (linePosition.Line == 0) ? 0 : tree.Source.LineBreaks.EndOfLineBreak(linePosition.Line - 1);
            // return lineStart + linePosition.Character;
        }

        /// <summary>
        /// Attribute name determining the field below is app-static instead of context-static.
        /// </summary>
        public const string AppStaticTagName = "@appstatic";

        /// <summary>
        /// Lookups notation determining given field as app-static instead of context-static.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool IsAppStatic(this FieldDecl field)
        {
            // if (field != null && field.Modifiers.IsStatic())
            // {
            //     var phpdoc = field.PHPDoc;
            //     if (phpdoc != null)
            //     {
            //         return phpdoc.Elements
            //             .OfType<PHPDocBlock.UnknownTextTag>()
            //             .Any(t => t.TagName.Equals(AppStaticTagName, StringComparison.OrdinalIgnoreCase));
            //     }
            // }

            return false;
        }

        /// <summary>
        /// Wraps given <see cref="MediaTypeNames.Text.Span"/> into <see cref="Microsoft.CodeAnalysis.Text.TextSpan"/> representing the same value.
        /// </summary>
        public static Microsoft.CodeAnalysis.Text.TextSpan ToTextSpan(this Span span)
        {
            return span.IsValid
                ? new Microsoft.CodeAnalysis.Text.TextSpan(span.Start, span.Length)
                : default;
        }

        /// <summary>
        /// Gets text span of given expression.
        /// </summary>
        public static Microsoft.CodeAnalysis.Text.TextSpan GetTextSpan(this BoundExpression expression)
        {
            return expression != null && expression.AquilaSyntax != null
                ? expression.AquilaSyntax.Span.ToTextSpan()
                : default;
        }

        // /// <summary>
        // /// CLR compliant anonymous class name.
        // /// </summary>
        // public static string GetAnonymousTypeName(this AnonymousTypeDecl tdecl)
        // {
        //     var fname = System.IO.Path.GetFileName(tdecl.ContainingSourceUnit.FilePath)
        //         .Replace('.', '_'); // TODO: relative to app root
        //     // PHP: class@anonymous\0{FULLPATH}{BUFFER_POINTER,X8}
        //     return $"class@anonymous {fname}{tdecl.Span.Start.ToString("X4")}";
        // }

        // /// <summary>
        // /// Builds qualified name for an anonymous PHP class.
        // /// Instead of name provided by parser, we do create our own which is more readable and shorter.
        // /// </summary>
        // /// <remarks>Wherever <see cref="QualifiedName"/> would be used, use this method instead.</remarks>
        // public static QualifiedName GetAnonymousTypeQualifiedName(this AnonymousTypeDecl tdecl)
        // {
        //     return new QualifiedName(new Name(GetAnonymousTypeName(tdecl)));
        // }

        /// <summary>
        /// Traverses AST and finds closest parent element of desired type.
        /// </summary>
        public static T FindParentLangElement<T>(LangElement node) where T : LangElement
        {
            while (node != null && !(node is T))
            {
                node = (LangElement) node.Parent;
            }

            return (T) node;
        }

        /// <summary>
        /// Gets containing routine element (function, method or lambda).
        /// </summary>
        public static LangElement GetContainingRoutine(this LangElement element)
        {
            while (!(element is MethodDecl || element == null))
            {
                element = (LangElement) element.Parent;
            }

            //
            return element;
        }

        // /// <summary>
        // /// Gets value indicating the type refers to a nullable type (<c>?TYPE</c>).
        // /// </summary>
        // public static bool IsNullable(this TypeRef tref)
        // {
        //     return tref is NullableTypeRef; // && tref != null
        // }

        // /// <summary>
        // /// Gets value indicating the type refers to <c>callable</c> or <c>?callable</c>.
        // /// </summary>
        // public static bool IsCallable(this TypeRef tref)
        // {
        //     if (tref is NullableTypeRef nullable)
        //     {
        //         tref = nullable.TargetType;
        //     }
        //
        //     return tref is PrimitiveTypeRef primitiveType &&
        //            primitiveType.PrimitiveTypeName == PrimitiveTypeRef.PrimitiveType.callable;
        // }

        // public static Microsoft.CodeAnalysis.Text.TextSpan GetDeclareClauseSpan(this DeclareStmt declStmt)
        // {
        //     if (declStmt.Statement is EmptyStmt)
        //     {
        //         // declare (...); - return whole span
        //         return declStmt.Span.ToTextSpan();
        //     }
        //     else
        //     {
        //         // declare (...) { ... } - return only the span of declare (...)
        //         int clauseStart = declStmt.Span.Start;
        //         int blockStart = declStmt.Statement.Span.Start;
        //         var searchSpan = new Span(clauseStart, blockStart - clauseStart);
        //         string searchText = declStmt.ContainingSourceUnit.GetSourceCode(searchSpan);
        //         int clauseLength = searchText.LastIndexOf(')') + 1;
        //
        //         return new Microsoft.CodeAnalysis.Text.TextSpan(clauseStart, clauseLength);
        //     }
        // }

        // /// <summary>
        // /// Gets the span of "as" keyword in between enumeree and variables.
        // /// </summary>
        // public static Microsoft.CodeAnalysis.Text.TextSpan GetMoveNextSpan(this ForeachStmt stmt)
        // {
        //     Debug.Assert(stmt != null);
        //
        //     // foreach(enumeree as key => value)
        //     // foreach(enumeree as value)
        //
        //     var enumeree = stmt.Enumeree.Span;
        //     if (enumeree.IsValid)
        //     {
        //         // key => value
        //         // value
        //         var variable = (stmt.KeyVariable ?? stmt.ValueVariable).Span;
        //         if (variable.IsValid)
        //         {
        //             return Span.FromBounds(enumeree.End + 1, variable.Start - 1).ToTextSpan();
        //         }
        //     }
        //
        //     // spans are not available
        //     return default;
        // }
        //
        // sealed class ElementVisitor<TElement> : TreeVisitor
        //     where TElement : LangElement
        // {
        //     readonly Func<LangElement, bool> _acceptPredicate;
        //
        //     public List<TElement> Result { get; } = new List<TElement>();
        //
        //     public ElementVisitor(Func<LangElement, bool> acceptPredicate)
        //     {
        //         _acceptPredicate = acceptPredicate;
        //     }
        //
        //     public override void VisitElement(LangElement element)
        //     {
        //         if (element is TElement x)
        //         {
        //             Result.Add(x);
        //         }
        //
        //         if (_acceptPredicate(element))
        //         {
        //             base.VisitElement(element);
        //         }
        //     }
        // }
        //
        // /// <summary>
        // /// Gets all elements of given type.
        // /// </summary>
        // public static List<TElement> SelectElements<TElement>(this LangElement root,
        //     Func<LangElement, bool> acceptPredicate)
        //     where TElement : LangElement
        // {
        //     var visitor = new ElementVisitor<TElement>(acceptPredicate);
        //     visitor.VisitElement(root);
        //     return visitor.Result;
        // }
        //
        // /// <summary>
        // /// Gets all occurences of <see cref="DirectVarUse"/> in given scope.
        // /// Ignores autoglobals and $this.
        // /// </summary>
        // public static IEnumerable<VarDecl> SelectLocalVariables(this LangElement root)
        // {
        //     return root.SelectElements<VarDeclarator>(
        //             scope => !(scope is FunctionDecl || scope is ILambdaExpression || scope is TypeDecl ||
        //                        scope is MethodDecl) || scope == root)
        //         .Where(dvar =>
        //             dvar.IsMemberOf == null && !dvar.VarName.IsAutoGlobal && !dvar.Identifier.IsThisVariableName);
        // }
    }
}