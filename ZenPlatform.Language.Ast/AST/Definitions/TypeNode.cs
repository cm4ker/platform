using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Тип
    /// </summary>
    public abstract class TypeNode : AstNode
    {
        protected TypeNode(ILineInfo lineInfo) : base(lineInfo)
        {
        }

        public virtual IType Type { get; }
    }

    public class SingleTypeNode : TypeNode
    {
        private IType _type;

        public SingleTypeNode(ILineInfo lineInfo, string typeName) : base(lineInfo)
        {
            _type = new UnknownType(typeName);
        }

        public SingleTypeNode(ILineInfo lineInfo, IType type) : base(lineInfo)
        {
            _type = type;
        }

        public override IType Type => _type;

        public void SetType(IType type)
        {
            _type = type;
        }

        public override void Accept(IVisitor visitor)
        {
            //Nothing to do
        }
    }

    public class MultiTypeNode : TypeNode
    {
        private readonly TypeCollection _types;
        private List<SingleTypeNode> _sTypes;

        public MultiTypeNode(ILineInfo lineInfo, TypeCollection types) : base(lineInfo)
        {
            _types = types;
        }

        private IEnumerable<SingleTypeNode> UnwrapSingleTypeNodes()
        {
            foreach (var type in _types)
            {
                if (type is MultiTypeNode mtn)
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

        public override IType Type => throw new NotImplementedException();

        public override void Accept(IVisitor visitor)
        {
            _types.ForEach(visitor.Visit);
        }
    }
}