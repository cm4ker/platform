using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Configuration.Common.TypeSystem;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Symbols;
using Module = ZenPlatform.Language.Ast.Definitions.Module;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private Dictionary<TypeEntity, ITypeBuilder> _stage0 = new Dictionary<TypeEntity, ITypeBuilder>();
        private Dictionary<Function, IMethodBuilder> _stage1Methods = new Dictionary<Function, IMethodBuilder>();
        private Dictionary<Property, IPropertyBuilder> _stage1Properties = new Dictionary<Property, IPropertyBuilder>();
        private Dictionary<Field, IField> _stage1Fields = new Dictionary<Field, IField>();

        private Dictionary<Constructor, IConstructorBuilder> _stage1constructors =
            new Dictionary<Constructor, IConstructorBuilder>();

        private GlobalVarManager _varManager;

        public void Build()
        {
            _epManager = new EntryPointAssemblyManager(_asm);

            if (_mode == CompilationMode.Server && _conf != null)
                _epManager.InitService();

            _map = new SyntaxTreeMemberAccessProvider(_root, _ts);

            BuildInfrastructure();
            BuildStructure();
            BuildGlobalVar();
            BuildCode();


            _epManager.EndBuild();
        }

        /// <summary>
        /// Построить глобальное дерево
        /// </summary>
        public void BuildGlobalVar()
        {
            if (_conf != null)
            {
                _varManager = new GlobalVarManager(_mode, _ts);

                foreach (var component in _conf.TypeManager.Components)
                {
                    component.ComponentImpl.Generator.StageGlobalVar(_varManager);
                }
            }
        }

        /// <summary>
        /// Построение структуры. Происходит в два этапа
        /// <br />
        /// 1) Построение типов
        /// <br />
        /// 2) Построение методов
        /// </summary>
        public void BuildStructure()
        {
            foreach (var cu in _cus)
            {
                BuildStage0(cu);
            }

            foreach (var cu in _cus)
            {
                BuildStage1(cu);
            }
        }

        /// <summary>
        /// Построение непосредственно кода
        /// </summary>
        public void BuildCode()
        {
            foreach (var cu in _cus)
            {
                BuildStage2(cu);
            }
        }

        /// <summary>
        /// Создание инфраструктуры всеми компонентами, для последующего использования внутри компонентов
        /// </summary>
        private void BuildInfrastructure()
        {
            if (_conf != null)
                foreach (var dataComponent in _conf.TypeManager.Components)
                {
                    dataComponent.ComponentImpl.Generator.StageInfrastructure(_asm, _parameters.TargetDatabaseType,
                        _mode);
                }
        }

        /// <summary>
        /// Prebuilding 1 level elements - classes and modules
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void BuildStage0(CompilationUnit cu)
        {
            foreach (var typeEntity in cu.GetTypes())
            {
                BuildTypeEntity(typeEntity);
            }
        }

        private void BuildTypeEntity(TypeEntity typeEntity)
        {
            if (_stage0.ContainsKey(typeEntity))
                return;

            void AfterPreBuild<T>(T sym, ITypeBuilder tb) where T : TypeEntity, IAstSymbol
            {
                var st = sym.FirstParent<IScoped>().SymbolTable;
                var symbol = st.Find<TypeSymbol>(sym);

                if (symbol == null)
                    symbol = st.AddType(sym);

                symbol.Connect(tb);
                _stage0.Add(sym, tb);
            }

            switch (typeEntity)
            {
                case Module m:
                    var tm = PreBuildModule(m);
                    AfterPreBuild(m, tm);
                    break;
                case Class c:
                    var tc = PreBuildClass(c);
                    AfterPreBuild(c, tc);
                    break;
                case ComponentAstTask co:
                {
                    var tco = PreBuildComponentAst(co);
                    if (tco is null) throw new Exception("Compilation error: component return null class builder");
                    AfterPreBuild(co, tco);
                    break;
                }

                default:
                    throw new Exception("The type entity not supported");
            }
        }

        /// <summary>
        /// Prebuilding 2 level elements - methods, constructors, properties and fields
        /// </summary>
        private void BuildStage1(CompilationUnit cu)
        {
            foreach (var typeEntity in cu.GetTypes())
            {
                switch (typeEntity)
                {
                    case Module m:

                        var tb = _stage0[m];

                        foreach (var function in m.TypeBody.Functions.FilterFunc(_mode))
                        {
                            var mf = PrebuildFunction(function, tb, false);
                            _stage1Methods.Add(function, mf);

                            var symbol = m.TypeBody.SymbolTable.Find<MethodSymbol>(function);
                            symbol.ConnectOverload(function, mf);

                            if (_conf != null && function.Flags == FunctionFlags.ServerClientCall &&
                                _mode == CompilationMode.Server)
                            {
                                EmitRegisterServerFunction(function);
                            }
                        }

                        break;
                    case Class c:
                        var tbc = _stage0[c];

                        foreach (var function in c.TypeBody.Functions.FilterFunc(_mode))
                        {
                            var mf = PrebuildFunction(function, tbc, true);
                            _stage1Methods.Add(function, mf);

                            var symbol = c.TypeBody.SymbolTable.Find<MethodSymbol>(function);
                            symbol.ConnectOverload(function, mf);

                            if (_conf != null && function.Flags == FunctionFlags.ServerClientCall &&
                                _mode == CompilationMode.Server)
                            {
                                EmitRegisterServerFunction(function);
                            }
                        }

                        foreach (var property in c.TypeBody.Properties)
                        {
                            var pp = PrebuildProperty(property, tbc);
                            var symbol = c.TypeBody.SymbolTable.Find<PropertySymbol>(property);
                            symbol.Connect(pp);
                            _stage1Properties.Add(property, pp);
                        }

                        foreach (var field in c.TypeBody.Fields)
                        {
                            var pf = PrebuildField(field, tbc);


                            var symbol = c.TypeBody.SymbolTable.Find<VariableSymbol>(field);
                            symbol.Connect(pf);


                            _stage1Fields.Add(field, pf);
                            ;
                        }

                        if (!c.TypeBody.Constructors.Any())
                        {
                            c.TypeBody.Constructors.Add(Constructor.Default);
                        }

                        foreach (var constructor in c.TypeBody.Constructors)
                        {
                            var pf = PrebuildConstructor(constructor, tbc);
                            _stage1constructors.Add(constructor, pf);
                            ;
                        }

                        break;
                    case ComponentAstTask cab:

                        if ((cab.CompilationMode & _mode) == 0) break;

                        var tcab = _stage0[cab];

                        var isClass = !cab.IsModule;
                        var isModule = !isClass;

                        foreach (var function in cab.TypeBody.Functions.FilterFunc(_mode))
                        {
                            var mf = PrebuildFunction(function, tcab, isClass);
                            _stage1Methods.Add(function, mf);


                            var symbol = cab.TypeBody.SymbolTable.Find<MethodSymbol>(function);
                            symbol.ConnectOverload(function, mf);

                            if (_conf != null && function.Flags == FunctionFlags.ServerClientCall &&
                                _mode == CompilationMode.Server)
                            {
                                EmitRegisterServerFunction(function);
                            }
                        }

                        cab.Component.ComponentImpl.Generator.Stage1(cab, tcab, _parameters.TargetDatabaseType, _mode,
                            _epManager);
                        break;

                    default:
                        throw new Exception("The type entity not supported");
                }
            }
        }

        /// <summary>
        /// Build, finaly, 3 level body of the methods && properties
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void BuildStage2(CompilationUnit cu)
        {
            foreach (var typeEntity in cu.GetTypes())
            {
                switch (typeEntity)
                {
                    case Module m:

                        foreach (var function in m.TypeBody.Functions.FilterFunc(_mode))
                        {
                            EmitFunction(function, _stage1Methods[function]);
                        }

                        break;
                    case Class c:
                        var tbc = _stage0[c];

                        foreach (var function in c.TypeBody.Functions.FilterFunc(_mode))
                        {
                            EmitFunction(function, _stage1Methods[function]);
                        }

                        foreach (var property in c.TypeBody.Properties)
                        {
                            BuildProperty(property, tbc, _stage1Properties[property]);
                        }

                        foreach (var constructor in c.TypeBody.Constructors)
                        {
                            EmitConstructor(constructor, tbc, _stage1constructors[constructor]);
                        }

                        break;
                    case ComponentAstTask cab:

                        if ((cab.CompilationMode & _mode) == 0) break;

                        var tbcab = _stage0[cab];

                        foreach (var function in cab.TypeBody.Functions.FilterFunc(_mode))
                        {
                            EmitFunction(function, _stage1Methods[function]);
                        }

                        foreach (var property in cab.TypeBody.Properties)
                        {
                            BuildProperty(property, tbcab, _stage1Properties[property]);
                        }

                        foreach (var constructor in cab.TypeBody.Constructors)
                        {
                            EmitConstructor(constructor, tbcab, _stage1constructors[constructor]);
                        }


                        cab.Component.ComponentImpl.Generator.Stage2(cab, tbcab, _parameters.TargetDatabaseType, _mode);
                        break;

                    default:
                        throw new Exception("The type entity not supported");
                }
            }
        }


        private IMethodBuilder PrebuildFunction(Function function, ITypeBuilder tb, bool isClass)
        {
            /* 
             * [Client]
             * fn A1()
             * 
             * [Server]
             * fn A2()
             * 
             * [ServerCall]
             * fn A3()
             */

//            //На сервере никогда не может существовать клиентских процедур
//            if (((int) function.Flags & (int) _mode) == 0 && !isClass)
//            {
//                return null;
//            }

            Console.WriteLine($"F: {function.Name} IsServer: {function.Flags}");

            var method = tb.DefineMethod(function.Name, function.IsPublic, !isClass, false)
                .WithReturnType(_map.GetClrType(function.Type));

            if (function.Parameters != null)
            {
                foreach (var p in function.Parameters)
                {
                    var codeObj = method.DefineParameter(p.Name, _map.GetClrType(p.Type), false, false);
                    function.Block.SymbolTable.FindOrDeclareVariable(p, codeObj);
                }
            }

            return method;
        }

        private IPropertyBuilder PrebuildProperty(Property property, ITypeBuilder tb)
        {
            var propBuilder = tb.DefineProperty(_map.GetClrType(property.Type), property.Name, false);

            IField backField = null;


            if (property.Setter == null && property.Getter == null)
            {
                backField = tb.DefineField(_map.GetClrType(property.Type), $"{property.Name}_backingField", false,
                    false);
            }

            if (property.HasGetter || property.Getter != null)
            {
                var getMethod = tb.DefineMethod($"get_{property.Name}", true, false, false);
                getMethod.WithReturnType(_map.GetClrType(property.Type));

                if (property.Getter == null)
                {
                    if (backField != null)
                        getMethod.Generator.LdArg_0().LdFld(backField).Ret();
                }

                propBuilder.WithGetter(getMethod);
            }

            if (property.HasSetter || property.Setter != null)
            {
                var setMethod = tb.DefineMethod($"set_{property.Name}", true, false, false);
                setMethod.WithReturnType(_bindings.Void);
                setMethod.DefineParameter("value", _map.GetClrType(property.Type), false, false);


                if (property.Setter == null)
                {
                    if (backField != null)
                        setMethod.Generator.LdArg_0().LdArg(1).StFld(backField).Ret();
                    else
                        setMethod.Generator.Ret();
                }


                propBuilder.WithSetter(setMethod);
            }

            return propBuilder;
        }

        private void BuildProperty(Property property, ITypeBuilder tb, IPropertyBuilder pb)
        {
            if (property.Getter != null)
            {
                var mb = tb.DefinedMethods.First(x => x.Name == pb.Getter.Name);

                IEmitter emitter = mb.Generator;
                emitter.InitLocals = true;

                ILocal resultVar = null;

                resultVar = emitter.DefineLocal(_map.GetClrType(property.Type));

                var returnLabel = emitter.DefineLabel();
                EmitBody(emitter, property.Getter, returnLabel, ref resultVar);

                emitter.MarkLabel(returnLabel);

                if (resultVar != null)
                    emitter.LdLoc(resultVar);

                emitter.Ret();
            }

            if (property.Setter != null)
            {
                var mb = tb.DefinedMethods.First(x => x.Name == pb.Setter.Name);

                IEmitter emitter = mb.Generator;
                emitter.InitLocals = true;

                ILocal resultVar = null;

                resultVar = emitter.DefineLocal(_map.GetClrType(property.Type));

                var valueSym =
                    property.Setter.SymbolTable.Find<VariableSymbol>("value", SymbolScopeBySecurity.Shared);
                valueSym.Connect(mb.Parameters[0]);

                var returnLabel = emitter.DefineLabel();
                EmitBody(emitter, property.Setter, returnLabel, ref resultVar);

                emitter.MarkLabel(returnLabel);
                emitter.Ret();
            }
        }

        private IField PrebuildField(Field field, ITypeBuilder tb)
        {
            return tb.DefineField(_map.GetClrType(field.Type), field.Name, false, false);
        }

        private IConstructorBuilder PrebuildConstructor(Constructor constructor, ITypeBuilder tb)
        {
            var c = tb.DefineConstructor(false);

            if (constructor.Parameters != null)
            {
                foreach (var p in constructor.Parameters)
                {
                    var codeObj = c.DefineParameter(_map.GetClrType(p.Type));
                    constructor.Block.SymbolTable.Find<VariableSymbol>(p).Connect(codeObj);
                }
            }

            return c;
        }

        private ITypeBuilder PreBuildClass(Class @class)
        {
            var tb = _asm.DefineType(
                (@class.GetNamespace()),
                @class.Name,
                TypeAttributes.Class | TypeAttributes.NotPublic |
                TypeAttributes.BeforeFieldInit | TypeAttributes.AnsiClass,
                _bindings.Object);

            if (@class.ImplementsReference)
                tb.AddInterfaceImplementation(_bindings.Reference);

            return tb;
        }

        private ITypeBuilder PreBuildModule(Module module)
        {
            return _asm.DefineType(
                module.GetNamespace(),
                module.Name,
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Abstract |
                TypeAttributes.BeforeFieldInit | TypeAttributes.AnsiClass, _bindings.Object);
        }

        private TypeEntity FindEntityByName(string name)
        {
            foreach (var entity in _cus.SelectMany(x => x.Entityes))
            {
                if (entity.Name == name)
                    return entity;
            }

            return null;
        }

        private ITypeBuilder PreBuildComponentAst(ComponentAstTask astTask)
        {
            return astTask.Component.ComponentImpl.Generator.Stage0(_asm, astTask);
        }
    }
}