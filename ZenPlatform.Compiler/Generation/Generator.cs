using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Microsoft.CodeAnalysis.Operations;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Generation.NewGenerator;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Infrastructure;
using IMethodBuilder = ZenPlatform.Compiler.Contracts.IMethodBuilder;
using SreTA = System.Reflection.TypeAttributes;
using Class = ZenPlatform.Language.Ast.Definitions.Class;
using Expression = ZenPlatform.Language.Ast.Definitions.Expression;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private readonly CompilationUnit _cu;
        private readonly IAssemblyBuilder _asm;
        private readonly ITypeSystem _ts;
        private readonly CompilationMode _mode;

        private SystemTypeBindings _bindings;

        private const string DEFAULT_ASM_NAMESPACE = "CompileNamespace";

        public Generator(GeneratorParameters parameters)
        {
            _cu = parameters.Unit;
            _asm = parameters.Builder;
            _ts = _asm.TypeSystem;

            _mode = parameters.Mode;
            _bindings = _ts.GetSystemBindings();
        }


        public void BuildStructure()
        {
            BuildStage0();
            BuildStage1();
        }

        public void BuildCode()
        {
            BuildStage2();
        }

        private Dictionary<TypeEntity, ITypeBuilder> _stage0 = new Dictionary<TypeEntity, ITypeBuilder>();
        private Dictionary<Function, IMethodBuilder> _stage1Methods = new Dictionary<Function, IMethodBuilder>();
        private Dictionary<Property, IPropertyBuilder> _stage1Properties = new Dictionary<Property, IPropertyBuilder>();
        private Dictionary<Field, IField> _stage1Fields = new Dictionary<Field, IField>();

        private Dictionary<Constructor, IConstructorBuilder> _stage1constructors =
            new Dictionary<Constructor, IConstructorBuilder>();


        /// <summary>
        /// Prebuilding 1 level elements - classes and modules
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void BuildStage0()
        {
            foreach (var typeEntity in _cu.Entityes)
            {
                switch (typeEntity)
                {
                    case Module m:
                        var tm = PreBuildModule(m);
                        m.FirstParent<IScoped>().SymbolTable.ConnectCodeObject(m, tm);
                        _stage0.Add(m, tm);
                        break;
                    case Class c:

                        var tc = PreBuildClass(c);
                        c.FirstParent<IScoped>().SymbolTable.ConnectCodeObject(c, tc);
                        _stage0.Add(c, tc);
                        break;
                    default:
                        throw new Exception("The type entity not supported");
                }
            }
        }

        /// <summary>
        /// Prebuilding 2 level elements - methods, constructors, properties and fields
        /// </summary>
        private void BuildStage1()
        {
            foreach (var typeEntity in _cu.Entityes)
            {
                switch (typeEntity)
                {
                    case Module m:

                        var tb = _stage0[m];

                        foreach (var function in m.TypeBody.Functions)
                        {
                            _stage1Methods.Add(function, PrebuildFunction(function, tb, false));
                        }

                        break;
                    case Class c:
                        var tbc = _stage0[c];

                        foreach (var function in c.TypeBody.Functions)
                        {
                            var mf = PrebuildFunction(function, tbc, false);
                            _stage1Methods.Add(function, mf);
                            c.TypeBody.SymbolTable.ConnectCodeObject(function, mf);
                        }

                        foreach (var property in c.TypeBody.Properties)
                        {
                            var pp = PrebuildProperty(property, tbc);
                            c.TypeBody.SymbolTable.ConnectCodeObject(property, pp);
                            _stage1Properties.Add(property, pp);
                        }

                        foreach (var field in c.TypeBody.Fields)
                        {
                            var pf = PrebuildField(field, tbc);
                            c.TypeBody.SymbolTable.ConnectCodeObject(field, pf);
                            _stage1Fields.Add(field, pf);
                            ;
                        }

                        foreach (var constructor in c.TypeBody.Constructors)
                        {
                            var pf = PrebuildConstructor(constructor, tbc);
                            _stage1constructors.Add(constructor, pf);
                            ;
                        }

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
        private void BuildStage2()
        {
            foreach (var typeEntity in _cu.Entityes)
            {
                switch (typeEntity)
                {
                    case Module m:

                        foreach (var function in m.TypeBody.Functions)
                        {
                            BuildFunction(function, _stage1Methods[function]);
                        }

                        break;
                    case Class c:
                        var tbc = _stage0[c];

                        EmitMappingSupport(c, tbc);

                        foreach (var function in c.TypeBody.Functions)
                        {
                            BuildFunction(function, _stage1Methods[function]);
                        }

                        foreach (var property in c.TypeBody.Properties)
                        {
                            BuildProperty(property, tbc, _stage1Properties[property]);
                        }

                        foreach (var constructor in c.TypeBody.Constructors)
                        {
                            BuildConstructor(constructor, tbc, _stage1constructors[constructor]);
                        }

                        break;
                    default:
                        throw new Exception("The type entity not supported");
                }
            }
        }


        private void BuildConstructor(Constructor constructor, ITypeBuilder tbc, IConstructorBuilder stage1Constructor)
        {
            constructor.Builder = stage1Constructor.Generator;

            EmitConstructor(constructor);
        }

        private void EmitConstructor(Constructor constructor)
        {
            if (constructor == null)
                throw new ArgumentNullException();

            IEmitter emitter = constructor.Builder;
            emitter.InitLocals = true;

            ILocal resultVar = null;

            var returnLabel = emitter.DefineLabel();

            //Используем конструктор по умолчанию
            emitter.LdArg_0().EmitCall(_bindings.Object.Constructors[0]);

            EmitBody(emitter, constructor.Block, returnLabel, ref resultVar);
            emitter.MarkLabel(returnLabel);

            emitter.Ret();
        }

        private IMethodBuilder PrebuildFunction(Function function, ITypeBuilder tb, bool isClass)
        {
            //На сервере никогда не может существовать клиентских процедур
            if (((int) function.Flags & (int) _mode) == 0 && !isClass)
            {
                return null;
            }

            Console.WriteLine($"F: {function.Name} IsServer: {function.Flags}");

            var method = tb.DefineMethod(function.Name, function.IsPublic, !isClass, false)
                .WithReturnType(function.Type.ToClrType(_asm));

            if (function.Parameters != null)
            {
                foreach (var p in function.Parameters)
                {
                    var codeObj = method.DefineParameter(p.Name, p.Type.ToClrType(_asm), false, false);
                    function.Block.SymbolTable.ConnectCodeObject(p, codeObj);
                }
            }

            return method;
        }

        private IPropertyBuilder PrebuildProperty(Property property, ITypeBuilder tb)
        {
            var propBuilder = tb.DefineProperty(property.Type.ToClrType(_asm), property.Name);

            IField backField = null;


            if (property.Setter == null && property.Getter == null)
            {
                backField = tb.DefineField(property.Type.ToClrType(_asm), $"{property.Name}_backingField", false,
                    false);
            }

            if (property.HasGetter || property.Getter != null)
            {
                var getMethod = tb.DefineMethod($"get_{property.Name}", true, false, false);
                getMethod.WithReturnType(property.Type.ToClrType(_asm));

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
                setMethod.DefineParameter("value", property.Type.ToClrType(_asm), false, false);


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

                resultVar = emitter.DefineLocal(property.Type.ToClrType(_asm));

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

                resultVar = emitter.DefineLocal(property.Type.ToClrType(_asm));

                var valueSym = property.Setter.SymbolTable.Find("value", SymbolType.Variable);
                valueSym.CodeObject = mb.Parameters[0];

                var returnLabel = emitter.DefineLabel();
                EmitBody(emitter, property.Setter, returnLabel, ref resultVar);

                emitter.MarkLabel(returnLabel);
                emitter.Ret();
            }
        }

        private IField PrebuildField(Field field, ITypeBuilder tb)
        {
            return tb.DefineField(field.Type.ToClrType(_asm), field.Name, false, false);
        }

        private IConstructorBuilder PrebuildConstructor(Constructor constructor, ITypeBuilder tb)
        {
            var c = tb.DefineConstructor(false);

            if (constructor.Parameters != null)
            {
                foreach (var p in constructor.Parameters)
                {
                    var codeObj = c.DefineParameter(p.Type.ToClrType(_asm));
                    constructor.Block.SymbolTable.ConnectCodeObject(p, codeObj);
                }
            }

            return c;
        }


        private ITypeBuilder PreBuildClass(Class @class)
        {
            var tb = _asm.DefineType(
                (string.IsNullOrEmpty(@class.Namespace) ? DEFAULT_ASM_NAMESPACE : @class.Namespace), @class.Name,
                SreTA.Class | SreTA.NotPublic |
                SreTA.BeforeFieldInit | SreTA.AnsiClass,
                _bindings.Object);


            if (@class.ImplementsReference)
                tb.AddInterfaceImplementation(_bindings.Reference);

            return tb;
        }

        private ITypeBuilder PreBuildModule(Module module)
        {
            return _asm.DefineType(DEFAULT_ASM_NAMESPACE, module.Name,
                SreTA.Class | SreTA.Public | SreTA.Abstract |
                SreTA.BeforeFieldInit | SreTA.AnsiClass, _bindings.Object);
        }

        private void Error(string message)
        {
            throw new Exception(message);
        }

        private IMethodBuilder BuildFunction(Function function, IMethodBuilder method)
        {
            if (function == null)
                throw new ArgumentNullException();


            function.Builder = method.Generator;

            EmitFunction(function);
            return method;
        }
    }
}