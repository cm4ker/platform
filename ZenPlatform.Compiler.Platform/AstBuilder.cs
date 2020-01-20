using System.Collections.Generic;
using ZenPlatform.Configuration.Common;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Compiler.Platform
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
            var ub = new CompilationUnitBuilder(new CompilationUnit(null, new List<NamespaceBase>(),
                new List<TypeEntity>()));
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
            _cu.Entityes.Add(mod);
            return new ModuleBuilder(mod);
        }

        public ClassBuilder WithClass(string className)
        {
            var cl = new Class(null, new TypeBody(new MemberCollection()), className);

            _cu.Entityes.Add(cl);

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
            var f = new Function(null, new Block(new StatementCollection()),
                new ParameterCollection(), new AttributeCollection(), name, new SingleTypeSyntax(null, type, 0));
            _module.TypeBody.Functions.Add(f);
            return new FunctionBuilder(f);
        }
    }

    public class TypeBuilder
    {
        private UnionTypeSyntax _mtn;

        public TypeBuilder()
        {
            _mtn = new UnionTypeSyntax(null, new TypeCollection());
        }
    }

    public class ClassBuilder
    {
        private readonly Class _cl;


        public ClassBuilder(Class cl)
        {
            _cl = cl;
        }

        public ClassBuilder WithProperty(string name, XCTypeBase type)
        {
            return this;
        }

        public ClassBuilder WithProperty(string name, IEnumerable<XCTypeBase> types)
        {
            var propTypes = new TypeCollection();
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
            var arg = new Parameter(null, name, new SingleTypeSyntax(null, type, 0), pm);
            return new ParameterBuilder(arg);
        }

        public StatementBuilder StatementBuilder => _sb;
    }

    public class StatementBuilder
    {
        private readonly Block _body;

        public StatementBuilder(Block body)
        {
            _body = body;
        }

        public VariableBuilder WithVariable(string name, string type)
        {
            return null;
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