using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using SreTA = System.Reflection.TypeAttributes;

namespace ZenPlatform.Compiler.Generation
{
    public interface ISyntaxContext
    {
        IAssemblyBuilder Assembly { get; set; }
        ITypeBuilder Type { get; set; }
        IMethodBuilder Method { get; set; }
        IConstructorBuilder Constructor { get; set; }
        IEmitter Emitter { get; set; }

        ILabel ReturnLabel { get; set; }
        ILocal Result { get; set; }
        AstNode AstNode { get; set; }
        CompilationMode Mode { get; set; }

        bool IsClass { get; set; }

        ISyntaxContext Copy(ISyntaxContext context);
    }

    public interface ITransform
    {
        void Transform(ISyntaxContext context);
    }

    public class PreGenerator : AstVisitorBase
    {
        private ISyntaxContext _context;
        private SystemTypeBindings _bindings;

        private const string DEFAULT_ASM_NAMESPACE = "CompileNamespace";

        public PreGenerator(GeneratorParameters parameters)
        {
            _context.Assembly = parameters.Builder;
            _context.Mode = parameters.Mode;
        }

        public override void VisitCompilationUnit(CompilationUnit cu)
        {
            _context.AstNode = cu;
            base.VisitCompilationUnit(cu);
        }

        public override void VisitModule(Module module)
        {
            _context.Type = _context.Assembly.DefineType(DEFAULT_ASM_NAMESPACE, module.Name,
                SreTA.Class | SreTA.Public | SreTA.Abstract |
                SreTA.BeforeFieldInit | SreTA.AnsiClass, _bindings.Object);
        }

        public override void VisitFunction(Function function)
        {
            //На сервере никогда не может существовать клиентских процедур
            if (((int) function.Flags & (int) _context.Mode) == 0 && !_context.IsClass)
            {
                Break();
            }

            var method = _context.Type.DefineMethod(function.Name, function.IsPublic, !_context.IsClass, false)
                .WithReturnType(function.Type.Type);

            var symTable = function.GetParent<IScoped>().SymbolTable;
            symTable.ConnectCodeObject(function, method);
        }

        public override void VisitField(Field obj)
        {
            if (!_context.IsClass) Break();

            var fld = _context.Type.DefineField(obj.Type.Type, obj.Name, false, false);
            obj.GetParent<IScoped>().SymbolTable.ConnectCodeObject(obj, fld);
        }

        public override void VisitProperty(Property property)
        {
            var tb = _context.Type;

            if (!_context.IsClass) Break();

            var propBuilder = tb.DefineProperty(property.Type.Type, property.Name);

            IField backField = null;

            var getMethod = tb.DefineMethod($"get_{property.Name}", true, false, false);
            var setMethod = tb.DefineMethod($"set_{property.Name}", true, false, false);

            setMethod.WithReturnType(_bindings.Void);
            getMethod.WithReturnType(property.Type.Type);

            if (property.Setter == null && property.Getter == null)
            {
                backField = tb.DefineField(property.Type.Type, $"{property.Name}_backingField", false,
                    false);

                getMethod.Generator.LdArg_0().LdFld(backField).Ret();
                setMethod.Generator.LdArg_0().LdArg(1).StFld(backField).Ret();
            }

            propBuilder.WithGetter(getMethod).WithSetter(setMethod);
        }
    }
}