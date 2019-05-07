using System.Collections.Generic;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Compiler.AST.Infrastructure
{
    public class ElementCollection : List<ElementNode>
    {
    }

    public class FunctionCollection : List<Function>
    {
    }

    public class VariableCollection : List<Variable>
    {
    }

    public class StructureCollection : List<Structure>
    {
    }

    public class ParameterCollection : List<Parameter>
    {
    }

    public class MemberCollection : List<Member>
    {
    }

    public class StatementCollection : List<Statement>
    {
    }

    public class ArgumentCollection : List<Argument>
    {
    }
}