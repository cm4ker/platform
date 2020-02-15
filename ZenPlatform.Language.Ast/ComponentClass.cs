using System;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Language.Ast
{
    public class ComponentAstBase : TypeEntity, IAstSymbol
    {
        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitTypeEntity(this);
        }

        public IComponent Component { get; }

        public IPType Type { get; }

        public object Bag { get; set; }

        public Func<ITypeSystem, IType> BaseTypeSelector { get; set; }

        public ComponentAstBase(CompilationMode compilationMode, IComponent component, IPType type,
            ILineInfo lineInfo, string name, TypeBody tb, TypeSyntax @base = null) : base(lineInfo, tb, name, @base)
        {
            CompilationMode = compilationMode;
            Component = component;
            Type = type;
        }

        public CompilationMode CompilationMode { get; set; }

        public void AddFunction(Function function)
        {
            TypeBody.AddFunction(function);
        }


        public SymbolScopeBySecurity SymbolScope { get; set; }
    }

    /// <summary>
    /// Перенаправляет генерацию кода в компонент
    /// </summary>
    public class ComponentClass : ComponentAstBase
    {
        public ComponentClass(CompilationMode compilationMode, IComponent component, IPType type,
            ILineInfo lineInfo, string name,
            TypeBody tb, TypeSyntax @base = null) : base(compilationMode, component, type, lineInfo, name, tb, @base)
        {
        }
    }


    /// <summary>
    /// Перенаправляет генерацию кода в компонент
    /// </summary>
    public class ComponentModule : ComponentAstBase
    {
        public ComponentModule(CompilationMode compilationMode, IComponent component, IPType type,
            ILineInfo lineInfo, string name,
            TypeBody tb) : base(compilationMode, component, type, lineInfo, name, tb)
        {
        }
    }

    /*
     
    -------- "+" ---------   | ---------- "-" ----------
     1) у нас появляется     |   1) Мы отказываемся от
     возможность лекго доб-  | понятия примитивный класс(только платформенный)
     лять новые классы в     | и используем ТОЛЬКО внутренние объявленные классы
     платформу. Просто новый | и бидинги
     биндинг                 |
     
     
     
     
     System.Int32        int
     System.Int64        long

     System.Guid         System.Guid
     
     using System;
     import method Int32 SomeMethod();     
     
     Int32 a = 0;
     int b = 0;
     
     
     if(a == b)
     
     */
    public class BindingClass : TypeEntity, IAstSymbol
    {
        public BindingClass(string forwardedName, IType bindingType) : base(null, TypeBody.Empty, forwardedName, null)
        {
            BindingType = bindingType;
        }

        public IType BindingType { get; set; }

        public SymbolScopeBySecurity SymbolScope { get; set; }
    }
}