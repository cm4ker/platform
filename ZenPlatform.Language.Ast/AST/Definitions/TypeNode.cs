using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions
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
        Array,
        Type,
        UnionType,
    }

    /// <summary>
    /// Тип
    /// </summary>
    public abstract class TypeNode : AstNode
    {
        protected TypeNode(ILineInfo lineInfo) : base(lineInfo)
        {
        }

        public TypeNodeKind Kind { get; protected set; }
    }

    public class PrimitiveTypeNode : TypeNode
    {
        public PrimitiveTypeNode(ILineInfo lineInfo, TypeNodeKind kind) : base(lineInfo)
        {
            //TODO: Check if kind is not primitive

            Kind = kind;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }
    }

    public class SingleTypeNode : TypeNode
    {
        public SingleTypeNode(ILineInfo lineInfo, string typeName, TypeNodeKind kind) : base(lineInfo)
        {
            TypeName = typeName;

            if (kind == TypeNodeKind.UnionType) throw new Exception("Single type can't be a union type");

            Kind = kind;
        }

        public string TypeName { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            //Nothing to do
            throw new NotImplementedException();
        }
    }

    [Obsolete]
    public class UnionTypeNode : TypeNode
    {
        private readonly TypeCollection _types;
        private List<SingleTypeNode> _sTypes;

        public UnionTypeNode(ILineInfo lineInfo, TypeCollection types) : base(lineInfo)
        {
            _types = types;
            Kind = TypeNodeKind.UnionType;
        }

        private IEnumerable<SingleTypeNode> UnwrapSingleTypeNodes()
        {
            foreach (var type in _types)
            {
                if (type is UnionTypeNode mtn)
                    foreach (var internalNode in mtn.UnwrapSingleTypeNodes())
                    {
                        yield return internalNode;
                    }

                if (type is SingleTypeNode stn)
                    yield return stn;
            }
        }

        public IReadOnlyCollection<SingleTypeNode> TypeList => _sTypes ??= UnwrapSingleTypeNodes().ToList();

        /// <summary>
        /// Задекларированное имя свойства, где лежит описание
        /// </summary>
        public string DeclName { get; set; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }
    }

    public class ArrayTypeNode : TypeNode
    {
        public ArrayTypeNode(ILineInfo lineInfo, TypeNode elementType) : base(lineInfo)
        {
            Kind = TypeNodeKind.Array;

            ElementType = elementType;
        }

        public TypeNode ElementType { get; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}