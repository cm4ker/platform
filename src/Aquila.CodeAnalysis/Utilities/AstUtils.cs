using Microsoft.CodeAnalysis.Text;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax.Ast;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis;

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

    /// <summary>
    /// Determines whether method has <c>$this</c> variable.
    /// </summary>
    public static bool HasThisVariable(MethodDecl method)
    {
        return false;
    }

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

    /// <summary>
    /// Gets text span of given expression.
    /// </summary>
    public static TextSpan GetTextSpan(this BoundExpression expression)
    {
        return expression != null && expression.AquilaSyntax != null
            ? expression.AquilaSyntax.Span
            : default;
    }
}