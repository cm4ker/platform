using System;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Contracts.TypeSystem;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast
{
    /// <summary>
    /// Перенаправляет генерацию кода в компонент
    /// </summary>
    public class ComponentAstTask : TypeEntity, IAstSymbol
    {
        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitTypeEntity(this);
        }

        public IComponent Component { get; }

        public Func<ITypeSystem, IType> BaseTypeSelector { get; set; }

        public ComponentAstTask(CompilationMode compilationMode, IComponent component, bool isModule,
            string name, TypeBody tb) : base(null, tb, name)
        {
            CompilationMode = compilationMode;
            Component = component;
            IsModule = isModule;
        }

        public bool IsModule { get; }

        public CompilationMode CompilationMode { get; set; }

        public void AddFunction(Function function)
        {
            TypeBody.Functions.Add(function);
        }

        public SymbolScopeBySecurity SymbolScope { get; set; }
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
        public BindingClass(string forwardedName, RoslynType bindingType) : base(null, TypeBody.Empty, forwardedName,
            null)
        {
            BindingType = bindingType;
        }

        public RoslynType BindingType { get; set; }

        public SymbolScopeBySecurity SymbolScope { get; set; }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitTypeEntity(this);
        }
    }
}