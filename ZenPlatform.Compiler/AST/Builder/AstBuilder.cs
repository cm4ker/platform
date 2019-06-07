using System;
using System.Collections.Generic;
using System.Dynamic;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Builder
{
    public class AstBuilder
    {
        private List<CompilationUnitBuilder> _units;

        public AstBuilder()
        {
            _units = new List<CompilationUnitBuilder>();
        }


        public CompilationUnitBuilder WithUnit()
        {
            var ub = new CompilationUnitBuilder(new CompilationUnit(null));
            _units.Add(ub);

            return ub;
        }
    }

    public class CompilationUnitBuilder
    {
        private readonly CompilationUnit _cu;

        public CompilationUnitBuilder(CompilationUnit cu)
        {
            _cu = cu;
        }

        ModuleBuilder WithModule(string moduleName)
        {
            return new ModuleBuilder(new Module(null, new TypeBody(new MemberCollection()), moduleName));
        }
    }

    public class ModuleBuilder
    {
        private readonly Module _module;

        public ModuleBuilder(Module module)
        {
            _module = module;
        }

        public FunctionBuilder WithFunction(string name, TypeBuilder tb)
        {
            var f = new Function(null, new InstructionsBodyNode(new StatementCollection()),
                new ParameterCollection(), name, tb.TypeNode, new AttributeCollection());
            _module.TypeBody.Functions.Add(f);
            return new FunctionBuilder(f);
        }
    }

    public class TypeBuilder
    {
        private TypeNode _typeNode;

        public TypeBuilder(string name)
        {
            _typeNode = new TypeNode(null, name);
        }

        internal TypeNode TypeNode => _typeNode;
    }

    public class FunctionBuilder
    {
        private readonly Function _function;
        private StatementBuilder _sb;

        public FunctionBuilder(Function function)
        {
            _function = function;
            _sb = new StatementBuilder(_function.InstructionsBody);
        }

        public ParameterBuilder WithParameter(string name, TypeBuilder tb, PassMethod pm)
        {
            var arg = new Parameter(null, name, tb.TypeNode, pm);
            return new ParameterBuilder(arg);
        }

        public StatementBuilder StatementBuilder => _sb;
    }

    public class StatementBuilder
    {
        private readonly InstructionsBodyNode _body;

        public StatementBuilder(InstructionsBodyNode body)
        {
            _body = body;
        }

        public VariableBuilder WithVariable(string name, TypeBuilder type)
        {
            var variable = new Variable(null, null, name, type.TypeNode);
            _body.Statements.Add(variable);
            return new VariableBuilder(variable);
        }

        public object WithCall(string name)
        {
            return null;
        }
    }

    public class VariableBuilder
    {
        private readonly Variable _variable;

        public VariableBuilder(Variable variable)
        {
            _variable = variable;
        }
    }

    public class ParameterBuilder
    {
        private readonly Parameter _parameter;

        public ParameterBuilder(Parameter parameter)
        {
            _parameter = parameter;
        }
    }

    public class ExpressionBuilder
    {
    }

    public class ArgumentBuilder
    {
        private readonly Argument _arg;

        public ArgumentBuilder(Argument arg)
        {
            _arg = arg;
        }
    }

    public class ClassBuilder
    {
    }
}