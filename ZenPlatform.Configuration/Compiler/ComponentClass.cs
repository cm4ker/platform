using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Configuration.Compiler
{
    public class ComponentAstBase : TypeEntity, IAstSymbol
    {
        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitTypeEntity(this);
        }

        public XCComponent Component { get; }

        public XCObjectTypeBase Type { get; }

        public ComponentAstBase(CompilationMode compilationMode, XCComponent component, XCObjectTypeBase type,
            ILineInfo lineInfo, string name) : base(
            lineInfo, name)
        {
            CompilationMode = compilationMode;
            Component = component;
            Type = type;
        }

        public ComponentAstBase(CompilationMode compilationMode, XCComponent component, XCObjectTypeBase type,
            ILineInfo lineInfo, string name,
            TypeBody tb) : base(lineInfo,
            name, tb)
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


        public SymbolScope SymbolScope { get; set; }
    }

    /// <summary>
    /// Перенаправляет генерацию кода в компонент
    /// </summary>
    public class ComponentClass : ComponentAstBase
    {
        public ComponentClass(CompilationMode compilationMode, XCComponent component, XCObjectTypeBase type,
            ILineInfo lineInfo, string name) : base(
            compilationMode, component, type, lineInfo, name)
        {
        }

        public ComponentClass(CompilationMode compilationMode, XCComponent component, XCObjectTypeBase type,
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
        public ComponentModule(CompilationMode compilationMode, XCComponent component, XCObjectTypeBase type,
            ILineInfo lineInfo, string name) : base(
            compilationMode, component, type, lineInfo, name)
        {
        }

        public ComponentModule(CompilationMode compilationMode, XCComponent component, XCObjectTypeBase type,
            ILineInfo lineInfo, string name,
            TypeBody tb) : base(compilationMode, component, type, lineInfo, name, tb)
        {
        }
    }
}