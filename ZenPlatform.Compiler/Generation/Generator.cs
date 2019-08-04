using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Net.Http;
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

        public void Build()
        {
            foreach (var typeEntity in _cu.Entityes)
            {
                switch (typeEntity)
                {
                    case Module m:
                        BuildModule(m);
                        break;
                    case Class c:
                        EmitClass(c);
                        break;
                    default:
                        throw new Exception("The type entity not supported");
                }
            }
        }

        private void BuildModule(Module module)
        {
            var typeBuilder = _asm.DefineType(DEFAULT_ASM_NAMESPACE, module.Name,
                SreTA.Class | SreTA.Public | SreTA.Abstract |
                SreTA.BeforeFieldInit | SreTA.AnsiClass, _bindings.Object);

            // Сделаем прибилд функции, чтобы она зерегистрировала себя в доступных символах модуля
            // Для того, чтобы можно было делать вызов функции из другой функции
            foreach (var item in PrebuildFunctions(module.TypeBody, typeBuilder, false))
            {
                BuildFunction(item.Item1, item.Item2);
            }

            typeBuilder.EndBuild();
        }

        private void EmitClass(Class @class)
        {
            var tb = _asm.DefineType(
                (string.IsNullOrEmpty(@class.Namespace) ? DEFAULT_ASM_NAMESPACE : @class.Namespace), @class.Name,
                SreTA.Class | SreTA.NotPublic |
                SreTA.BeforeFieldInit | SreTA.AnsiClass,
                _bindings.Object);

            PrebuildProperties(@class.TypeBody, tb);

            //Поддержка интерфейса ICanMapSelfFromDataReader
            EmitMappingSupport(@class, tb);

            // Сделаем прибилд функции, чтобы она зерегистрировала себя в доступных символах модуля
            // Для того, чтобы можно было делать вызов функции из другой функции
            foreach (var item in PrebuildFunctions(@class.TypeBody, tb, true))
            {
                BuildFunction(item.Item1, item.Item2);
            }

            tb.EndBuild();
        }

        private void Error(string message)
        {
            throw new Exception(message);
        }


        /// <summary>
        /// Создание функций исходя из тела типа
        /// </summary>
        /// <param name="typeBody"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private List<(Function, IMethodBuilder)> PrebuildFunctions(TypeBody typeBody, ITypeBuilder tb, bool isClass)
        {
            if (typeBody == null)
                throw new ArgumentNullException();


            List<(Function, IMethodBuilder)> result = new List<(Function, IMethodBuilder)>();

            if (typeBody != null && typeBody.Functions != null)
            {
                foreach (Function function in typeBody.Functions)
                {
                    //На сервере никогда не может существовать клиентских процедур
                    if (((int) function.Flags & (int) _mode) == 0 && !isClass)
                    {
                        continue;
                    }

                    Console.WriteLine($"F: {function.Name} IsServer: {function.Flags}");

                    var method = tb.DefineMethod(function.Name, function.IsPublic, !isClass, false)
                        .WithReturnType(function.Type.ToClrType(_asm));

                    result.Add((function, method));

                    // Make child visible to parent.
                    typeBody.SymbolTable.ConnectCodeObject(function, method);
                    //symbolTable.Add(function.Name, SymbolType.Function, function, method);
                }
            }

            return result;
        }

        private IMethodBuilder BuildFunction(Function function, IMethodBuilder method)
        {
            if (function == null)
                throw new ArgumentNullException();
            if (function.Parameters != null)
            {
                foreach (var p in function.Parameters)
                {
                    var codeObj = method.DefineParameter(p.Name, null, false, false);
                    function.Block.SymbolTable.ConnectCodeObject(p, codeObj);
                }
            }

            function.Builder = method.Generator;

            EmitFunction(function);
            return method;
        }
    }
}