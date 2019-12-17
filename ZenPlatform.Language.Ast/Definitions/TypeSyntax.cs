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
        Type,
        Session,
        UnionType,
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
    public abstract partial class TypeSyntax : SyntaxNode
    {
        public TypeNodeKind Kind { get; protected set; }
    }

    public partial class PrimitiveTypeSyntax
    {
        public PrimitiveTypeSyntax(ILineInfo lineInfo, TypeNodeKind kind) : this(lineInfo)
        {
            //TODO: Check if kind is not primitive

            Kind = kind;
        }
    }

    public partial class SingleTypeSyntax
    {
        public SingleTypeSyntax(ILineInfo lineInfo, string typeName, TypeNodeKind kind) : this(lineInfo)
        {
            TypeName = typeName;

            if (kind == TypeNodeKind.UnionType) throw new Exception("Single type can't be a union type");

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
    }

    [Obsolete]
    public partial class UnionTypeSyntax
    {
        private readonly TypeCollection _types;
        private List<SingleTypeSyntax> _sTypes;

        public UnionTypeSyntax(ILineInfo lineInfo, TypeCollection types) : base(lineInfo)
        {
            _types = types;
            Kind = TypeNodeKind.UnionType;
        }

        private IEnumerable<SingleTypeSyntax> UnwrapSingleTypeNodes()
        {
            foreach (var type in _types)
            {
                if (type is UnionTypeSyntax mtn)
                    foreach (var internalNode in mtn.UnwrapSingleTypeNodes())
                    {
                        yield return internalNode;
                    }

                if (type is SingleTypeSyntax stn)
                    yield return stn;
            }
        }

        public IReadOnlyCollection<SingleTypeSyntax> TypeList => _sTypes ??= UnwrapSingleTypeNodes().ToList();

        /// <summary>
        /// Задекларированное имя свойства, где лежит описание
        /// </summary>
        public string DeclName { get; set; }
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
}