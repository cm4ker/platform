using System.Collections.Generic;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Builder
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

        public ModuleBuilder WithModule(string moduleName)
        {
            var mod = new Module(null, new TypeBody(new MemberCollection()), moduleName);
            _cu.TypeEntities.Add(mod);
            return new ModuleBuilder(mod);
        }

        public ClassBuilder WithClass(string className)
        {
            var cl = new Class(null, new TypeBody(new MemberCollection()), className);

            _cu.TypeEntities.Add(cl);

            return new ClassBuilder(cl);
        }
    }

    public class ModuleBuilder
    {
        private readonly Module _module;

        public ModuleBuilder(Module module)
        {
            _module = module;
        }

        public FunctionBuilder WithFunction(string name, string type)
        {
            var f = new Function(null, new BlockNode(new StatementCollection()),
                new ParameterCollection(), name, new SingleTypeNode(null, type), new AttributeCollection());
            _module.TypeBody.Functions.Add(f);
            return new FunctionBuilder(f);
        }
    }

    public class TypeBuilder
    {
        private MultiTypeNode _mtn;

        public TypeBuilder()
        {
            _mtn = new MultiTypeNode(null, new TypeCollection());
        }
    }

    public class ClassBuilder
    {
        private readonly Class _cl;


        public ClassBuilder(Class cl)
        {
            _cl = cl;
        }


        public ClassBuilder WithProperty(string name, string type)
        {
            return this;
        }
    }

    public class FunctionBuilder
    {
        private readonly Function _function;
        private StatementBuilder _sb;

        public FunctionBuilder(Function function)
        {
            _function = function;
            _sb = new StatementBuilder(_function.Block);
        }

        public ParameterBuilder WithParameter(string name, string type, PassMethod pm)
        {
            var arg = new Parameter(null, name, new SingleTypeNode(null, type), pm);
            return new ParameterBuilder(arg);
        }

        public StatementBuilder StatementBuilder => _sb;
    }

    public class StatementBuilder
    {
        private readonly BlockNode _body;

        public StatementBuilder(BlockNode body)
        {
            _body = body;
        }

        public VariableBuilder WithVariable(string name, string type)
        {
            var variable = new Variable(null, null, name, new SingleTypeNode(null, type));
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
}