using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Language.Ast.Definitions
{
    public enum TypeNodeKind
    {
        Unknown,
        String,
        Void,
        Int,
        Double,
        Char,
        Boolean,
        Object,
        Byte,
        Array,
        Context,
        Type,
        Session,
    }

    public static class TypeNodeKindExtension
    {
        public static bool IsNumeric(this TypeNodeKind tnk)
        {
            return tnk == TypeNodeKind.Int || tnk == TypeNodeKind.Double;
        }

        public static bool IsBoolean(this TypeNodeKind tnk)
        {
            return tnk == TypeNodeKind.Boolean;
        }

        public static bool IsString(this TypeNodeKind tnk)
        {
            return tnk == TypeNodeKind.String;
        }

        public static bool IsNumeric(this TypeSyntax ts) => ts.Kind.IsNumeric();

        public static bool IsString(this TypeSyntax ts) => ts.Kind.IsString();

        public static bool IsBoolean(this TypeSyntax ts) => ts.Kind.IsBoolean();
    }

    /// <summary>
    /// Тип
    /// </summary>
    public abstract partial class TypeSyntax : ICloneable
    {
        public TypeNodeKind Kind { get; protected set; }

        public virtual object Clone()
        {
            throw new NotImplementedException(this.GetType().FullName);
        }
    }

    public partial class PrimitiveTypeSyntax
    {
        public PrimitiveTypeSyntax(ILineInfo lineInfo, TypeNodeKind kind) : this(lineInfo)
        {
            //TODO: Check if kind is not primitive

            Kind = kind;
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }

    public partial class SingleTypeSyntax
    {
        public SingleTypeSyntax(ILineInfo lineInfo, string typeName, TypeNodeKind kind) : this(lineInfo)
        {
            TypeName = typeName;

            Kind = kind;
        }

        public string TypeName { get; }

        /// <summary>
        /// Меняет тип с Unknown
        ///
        /// Если тип уже установлен, то он не будет изменен.
        /// </summary>
        /// <param name="kind"></param>
        public void ChangeKind(TypeNodeKind kind)
        {
            if (Kind == TypeNodeKind.Unknown)
                Kind = kind;
        }
        
        public override object Clone()
        {
            return MemberwiseClone();
        }
    }

    public partial class ArrayTypeSyntax
    {
        public ArrayTypeSyntax(ILineInfo lineInfo, TypeSyntax elementType) : base(lineInfo)
        {
            Kind = TypeNodeKind.Array;

            ElementType = elementType;
        }

        public TypeSyntax ElementType { get; }
    }

    public partial class GenericTypeSyntax
    {
        public GenericTypeSyntax(ILineInfo lineInfo, string typeName, TypeNodeKind kind, TypeList args) : this(lineInfo,
            args)
        {
            TypeName = typeName;

            Kind = kind;
        }

        public string TypeName { get; }
    }
}