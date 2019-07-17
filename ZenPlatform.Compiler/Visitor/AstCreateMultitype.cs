using System.Reflection;
using System.Runtime.Serialization;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Infrastructure;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Expressions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Compiler.Visitor
{
    /// <summary>
    /// Визитор для вычисления типа
    /// </summary>
    public class AstCreateMultitype : AstVisitorBase<object>
    {
        private readonly IAssemblyBuilder _asm;
        private readonly ITypeBuilder _defMT;
        private int _mtIndex;
        private SystemTypeBindings _tsb;
        private IConstructorBuilder _ctor;


        public AstCreateMultitype(IAssemblyBuilder asm)
        {
            _asm = asm;

            _tsb = _asm.TypeSystem.GetSystemBindings();

            _defMT = _asm.DefineType("PlatformCustom", "DefinedMultitypes",
                TypeAttributes.Class | TypeAttributes.Public |
                TypeAttributes.BeforeFieldInit | TypeAttributes.AnsiClass
                | TypeAttributes.Abstract | TypeAttributes.Sealed, _tsb.Object);

            _ctor = _defMT.DefineConstructor(true);

            _mtIndex = 0;
        }


        private string GetFieldName()
        {
            return $"MT_{_mtIndex++}";
        }

        public override object VisitMultiType(UnionTypeNode obj)
        {
            var name = GetFieldName();
            obj.DeclName = name;
            var field = _defMT.DefineField(_tsb.MultiType, name, true, true);

            var e = _ctor.Generator;

            e.LdcI4(obj.TypeList.Count);
            e.NewArr(_tsb.Type);

            var typeIndex = 0;

            foreach (var sType in obj.TypeList)
            {
                e.Dup();
                e.LdcI4(typeIndex++);
                //e.LdType(sType.Type);
                e.StElemRef();
            }

            e.NewObj(_tsb.MultiType.FindConstructor(_tsb.Type.MakeArrayType()));
            e.StSFld(field);

            return null;
        }

        private Expression GetValue(Name name)
        {
            return new FieldExpression(name, "Value");
        }

        public void Bake()
        {
            _ctor.Generator.Ret();
            _defMT.EndBuild();
        }
    }
}