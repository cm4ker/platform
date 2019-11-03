using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        public ComponentAstBase(XCComponent component, XCObjectTypeBase type, ILineInfo lineInfo, string name) : base(
            lineInfo, name)
        {
            Component = component;
            Type = type;
        }

        public ComponentAstBase(XCComponent component, XCObjectTypeBase type, ILineInfo lineInfo, string name,
            TypeBody tb) : base(lineInfo,
            name, tb)
        {
            Component = component;
            Type = type;
        }

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
        public ComponentClass(XCComponent component, XCObjectTypeBase type, ILineInfo lineInfo, string name) : base(
            component, type, lineInfo, name)
        {
        }

        public ComponentClass(XCComponent component, XCObjectTypeBase type, ILineInfo lineInfo, string name,
            TypeBody tb) : base(component, type, lineInfo, name, tb)
        {
        }
    }


    /// <summary>
    /// Перенаправляет генерацию кода в компонент
    /// </summary>
    public class ComponentModule : ComponentAstBase
    {
        public ComponentModule(XCComponent component, XCObjectTypeBase type, ILineInfo lineInfo, string name) : base(
            component, type, lineInfo, name)
        {
        }

        public ComponentModule(XCComponent component, XCObjectTypeBase type, ILineInfo lineInfo, string name,
            TypeBody tb) : base(component, type, lineInfo, name, tb)
        {
        }
    }
}