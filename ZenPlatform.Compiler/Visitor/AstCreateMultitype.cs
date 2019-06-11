using System.Reflection;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Infrastructure;
using ZenPlatform.Language.Ast.AST.Definitions;

namespace ZenPlatform.Compiler.Visitor
{
    /// <summary>
    /// Визитор для вычисления типа
    /// </summary>
    public class AstCreateMultitype : AstVisitorBase
    {
        private readonly IAssemblyBuilder _asm;
        private readonly ITypeBuilder _defMT;
        private int _mtIndex;
        private SystemTypeBindings _tsb;


        public AstCreateMultitype(IAssemblyBuilder asm)
        {
            _asm = asm;

            _tsb = new SystemTypeBindings(_asm.TypeSystem);

            _defMT = _asm.DefineType("PlatformCustom", "DefinedMultitypes",
                TypeAttributes.Class | TypeAttributes.Public |
                TypeAttributes.BeforeFieldInit | TypeAttributes.AnsiClass
                | TypeAttributes.Abstract | TypeAttributes.Sealed, _tsb.Object);

            _mtIndex = 0;
        }


        private string GetFieldName()
        {
            return $"MT_{_mtIndex++}";
        }

        public override void VisitMultiType(MultiTypeNode obj)
        {
            var name = GetFieldName();
            obj.DeclName = name;
            var field = _defMT.DefineField(_tsb.MultiType, name, true, true);
        }

        public void Bake()
        {
            _defMT.EndBuild();
        }
    }
}