using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Тип
    /// </summary>
    public class TypeNode : AstNode
    {
        private IType _type;

        public TypeNode(ILineInfo lineInfo, string typeName) : base(lineInfo)
        {
            _type = new UnknownType(typeName);
        }

        public TypeNode(ILineInfo lineInfo, IType type) : base(lineInfo)
        {
            _type = type;
        }

        public IType Type => _type;


        public void SetType(IType type)
        {
            _type = type;
        }


        public override void Accept(IVisitor visitor)
        {
            //Nothing to do
        }
    }
}