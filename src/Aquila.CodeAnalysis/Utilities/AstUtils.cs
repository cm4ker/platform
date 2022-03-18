using Microsoft.CodeAnalysis.Text;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax.Ast;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis
{
    public static class AstUtils
    {
        public static MemberModifiers GetModifiers(SyntaxTokenList modifiers)
        {
            MemberModifiers _memberModifers = MemberModifiers.Private;

            foreach (var modifier in modifiers)
            {
                _memberModifers |= modifier.Text switch
                {
                    "pub" => MemberModifiers.Public,
                    "part" => MemberModifiers.Partial,
                    // "static" => MemberModifiers.Static,
                    // "private" => MemberModifiers.Private
                };
            }

            return _memberModifers;
        }


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
        }

        // public static Span BodySpanOrInvalid(this LangElement method)
        // {
        //     if (method is MethodDecl m)
        //     {
        //         return m.Block?.Span ?? Span.Invalid;
        //     }
        //     else
        //     {
        //         return Span.Invalid;
        //     }
        // }


        /// <summary>
        /// Returns the offset of the location specified by (zero-based) line and character from the start of the file.
        /// In the case of invalid line, -1 is returned.
        /// </summary>
        public static int GetOffset(this AquilaSyntaxTree tree, LinePosition linePosition)
        {
            return tree.GetText().Lines.GetPosition(linePosition);
        }

        /// <summary>
        /// Attribute name determining the field below is app-static instead of context-static.
        /// </summary>
        public const string AppStaticTagName = "@appstatic";

        // /// <summary>
        // /// Lookups notation determining given field as app-static instead of context-static.
        // /// </summary>
        // /// <param name="field"></param>
        // /// <returns></returns>
        // public static bool IsAppStatic(this FieldDecl field)
        // {
        //     return false;
        // }


        /// <summary>
        /// Gets text span of given expression.
        /// </summary>
        public static TextSpan GetTextSpan(this BoundExpression expression)
        {
            return expression != null && expression.AquilaSyntax != null
                ? expression.AquilaSyntax.Span
                : default;
        }

        // /// <summary>
        // /// Traverses AST and finds closest parent element of desired type.
        // /// </summary>
        // public static T FindParentLangElement<T>(LangElement node) where T : LangElement
        // {
        //     while (node != null && !(node is T))
        //     {
        //         node = (LangElement)node.Parent;
        //     }
        //
        //     return (T)node;
        // }
        //
        // /// <summary>
        // /// Gets containing method element (function, method or lambda).
        // /// </summary>
        // public static LangElement GetContainingMethod(this LangElement element)
        // {
        //     while (!(element is MethodDecl || element == null))
        //     {
        //         element = (LangElement)element.Parent;
        //     }
        //
        //     //
        //     return element;
        // }

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