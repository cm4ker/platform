using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Compiler.Visitor;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using SreTA = System.Reflection.TypeAttributes;
using Class = ZenPlatform.Language.Ast.Definitions.Class;


namespace ZenPlatform.Compiler.Generation.NewGenerator
{
    /// <summary>
    /// Создание структуры типов
    /// </summary>
    public class TypeBodyGenerator : AstVisitorBase<object>
    {
        private IAstNodeContext _context;
        private SystemTypeBindings _bindings;

        private const string DEFAULT_ASM_NAMESPACE = "CompileNamespace";

        public TypeBodyGenerator(GeneratorParameters parameters)
        {
            _context = new AstNodeContext();
            _context.Assembly = parameters.Builder;
            _context.Mode = parameters.Mode;
            _bindings = _context.Assembly.TypeSystem.GetSystemBindings();
        }

        public override object VisitCompilationUnit(CompilationUnit cu)
        {
            _context.SyntaxNode = cu;
            return null;
        }

        public override object VisitFunction(Function function)
        {
            //На сервере никогда не может существовать клиентских процедур
            if (((int) function.Flags & (int) _context.Mode) == 0 && !_context.IsClass)
            {
            }

            var method = _context.Type.DefineMethod(function.Name, function.IsPublic, !_context.IsClass, false)
                .WithReturnType(function.Type.ToClrType(_context.Assembly));

            var symTable = function.FirstParent<IScoped>().SymbolTable;
            symTable.ConnectCodeObject(function, method);

            function.Builder = method.Generator;


            return null;
        }

        public override object VisitParameter(Parameter obj)
        {
            var codeObj = _context.Method.WithParameter(obj.Name, obj.Type.ToClrType(_context.Assembly), false, false);
            _context.SymbolTable.ConnectCodeObject(obj, codeObj);


            return null;
        }

        public override object VisitField(Field obj)
        {
            //if (!_context.IsClass) Stop();

            var fld = _context.Type.DefineField(obj.Type.ToClrType(_context.Assembly), obj.Name, false, false);
            obj.FirstParent<IScoped>().SymbolTable.ConnectCodeObject(obj, fld);


            return null;
        }

        public override object VisitProperty(Property property)
        {
            var tb = _context.Type;

            //if (!_context.IsClass) Stop();

            tb.DefineProperty(property.Type.ToClrType(_context.Assembly), property.Name);

            return null;
        }
    }

    /// <summary>
    /// Создаём типы
    /// </summary>
    public class TypeGenerator : AstVisitorBase<object>
    {
        private IAstNodeContext _context;
        private SystemTypeBindings _bindings;

        private const string DEFAULT_ASM_NAMESPACE = "CompileNamespace";

        public TypeGenerator(GeneratorParameters parameters)
        {
            _context = new AstNodeContext();
            _context.Assembly = parameters.Builder;
            _context.Mode = parameters.Mode;
            _bindings = _context.Assembly.TypeSystem.GetSystemBindings();
        }

        public override object VisitClass(Class obj)
        {
            _context.Type = _context.Assembly.DefineType(DEFAULT_ASM_NAMESPACE, obj.Name,
                SreTA.Class | SreTA.Public | SreTA.Abstract |
                SreTA.BeforeFieldInit | SreTA.AnsiClass, _bindings.Object);

            obj.FirstParent<IScoped>().SymbolTable.ConnectCodeObject(obj, _context.Type);
            return default;
        }

        public override object VisitModule(Module module)
        {
            _context.Type = _context.Assembly.DefineType(DEFAULT_ASM_NAMESPACE, module.Name,
                SreTA.Class | SreTA.NotPublic |
                SreTA.BeforeFieldInit | SreTA.AnsiClass, _bindings.Object);

            module.FirstParent<IScoped>().SymbolTable.ConnectCodeObject(module, _context.Type);
            return default;
        }
    }
}