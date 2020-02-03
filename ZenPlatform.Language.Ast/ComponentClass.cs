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

        
        /*
         Type(Id) + Metadata(Stuff)
         */
        public IPType Type { get; }

        public object Bag { get; set; }

        public ComponentAstBase(CompilationMode compilationMode, IComponent component, IPType type,
            ILineInfo lineInfo, string name,
            TypeBody tb) : base(lineInfo
            , tb, name)
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
            TypeBody tb) : base(compilationMode, component, type, lineInfo, name, tb)
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
}