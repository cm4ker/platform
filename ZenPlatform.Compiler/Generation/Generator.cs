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
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Expressions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Infrastructure;
using IMethodBuilder = ZenPlatform.Compiler.Contracts.IMethodBuilder;
using SreTA = System.Reflection.TypeAttributes;
using Class = ZenPlatform.Language.Ast.AST.Definitions.Class;

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
            _bindings = new SystemTypeBindings(_ts);

            foreach (var typeEntity in _cu.TypeEntities)
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
            var tb = _asm.DefineType(DEFAULT_ASM_NAMESPACE, @class.Name,
                SreTA.Class | SreTA.NotPublic |
                SreTA.BeforeFieldInit | SreTA.AnsiClass,
                _bindings.Object);

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

        private void EmitExpression(IEmitter e, Expression expression, SymbolTable symbolTable)
        {
            if (expression is BinaryExpression)
            {
                EmitExpression(e, ((BinaryExpression) expression).Left, symbolTable);
                EmitExpression(e, ((BinaryExpression) expression).Right, symbolTable);

                switch (((BinaryExpression) expression).BinaryOperatorType)
                {
                    case BinaryOperatorType.Add:
                        e.Add();
                        break;
                    case BinaryOperatorType.Subtract:
                        e.Sub();
                        break;
                    case BinaryOperatorType.Multiply:
                        e.Mul();
                        break;
                    case BinaryOperatorType.Divide:
                        e.Div();
                        break;
                    case BinaryOperatorType.Modulo:
                        e.Rem();
                        break;
                    case BinaryOperatorType.Equal:
                        e.Ceq();
                        break;
                    case BinaryOperatorType.NotEqual:
                        e.NotEqual();
                        break;
                    case BinaryOperatorType.GreaterThen:
                        e.Cgt();
                        break;
                    case BinaryOperatorType.LessThen:
                        e.Clt();
                        break;
                    case BinaryOperatorType.GraterOrEqualTo:
                        e.GreaterOrEqual();
                        break;
                    case BinaryOperatorType.LessOrEqualTo:
                        e.LessOrEqual();
                        break;
                    case BinaryOperatorType.And:
                        e.Add();
                        break;
                    case BinaryOperatorType.Or:
                        e.Or();
                        break;
                }
            }
            else if (expression is UnaryExpression ue)
            {
                if (ue is IndexerExpression ie)
                {
                    EmitExpression(e, ie.Value, symbolTable);
                    EmitExpression(e, ie.Indexer, symbolTable);
                    e.LdElemI4();
                }

                if (ue is LogicalOrArithmeticExpression lae)

                    switch (lae.Type)
                    {
                        case UnaryOperatorType.Indexer:

                            break;
                        case UnaryOperatorType.Negative:
                            EmitExpression(e, lae.Value, symbolTable);
                            e.Neg();
                            break;
                        case UnaryOperatorType.Not:
                            EmitExpression(e, lae.Value, symbolTable);
                            e.Not();
                            break;
                    }

                if (ue is CastExpression ce)
                {
                    EmitExpression(e, ce.Value, symbolTable);
                    EmitConvert(e, ce, symbolTable);
                }
            }
            else if (expression is Literal literal)
            {
                switch (literal.Type.Kind)
                {
                    case TypeNodeKind.Int:
                        e.LdcI4(Int32.Parse(literal.Value));
                        break;
                    case TypeNodeKind.String:
                        e.LdStr(literal.Value);
                        break;
                    case TypeNodeKind.Double:
                        e.LdcR8(double.Parse(literal.Value, CultureInfo.InvariantCulture));
                        break;
                    case TypeNodeKind.Char:
                        e.LdcI4(char.ConvertToUtf32(literal.Value, 0));
                        break;
                    case TypeNodeKind.Boolean:
                        e.LdcI4(bool.Parse(literal.Value) ? 1 : 0);
                        break;
                }
            }
            else if (expression is Name name)
            {
                var variable = symbolTable.Find(name.Value, SymbolType.Variable);

                if (variable == null)
                    Error("Assignment variable " + name.Value + " unknown.");

                if (name.Type is null)
                    if (variable.SyntaxObject is ITypedNode tn)
                        name.Type = tn.Type;

                if (variable.CodeObject is ILocal vd)
                {
                    if (name.Type is UnionTypeNode)
                    {
                        e.LdLocA(vd);
                        e.EmitCall(_bindings.UnionTypeStorage.FindProperty("Value").Getter);
                    }
                    else
                        e.LdLoc(vd);
                }
                else if (variable.CodeObject is IField fd)
                    e.LdsFld(fd);
                else if (variable.CodeObject is IParameter pd)
                {
                    Parameter p = variable.SyntaxObject as Parameter;

                    if (name.Type is UnionTypeNode)
                    {
                        e.LdArgA(pd);
                        e.EmitCall(_bindings.UnionTypeStorage.FindProperty("Value").Getter);
                    }
                    else
                        e.LdArg(pd.ArgIndex);

                    if (p.PassMethod == PassMethod.ByReference)
                        e.LdIndI4();
                }
            }
            else if (expression is Call call)
            {
                EmitCall(e, call, symbolTable);
            }
            else if (expression is FieldExpression fe)
            {
                EmitExpression(e, fe.Expression, symbolTable);
                var expType = fe.Expression.Type;

                IType extTypeScan = null;

                var expProp = extTypeScan.Properties.First(x => x.Name == fe.Name);
                fe.Type = new SingleTypeNode(null, expProp.PropertyType.Name, TypeNodeKind.Unknown);

                e.PropGetValue(expProp);
            }
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
                        .WithReturnType(null);

                    result.Add((function, method));

                    // Make child visible to parent.
                    typeBody.SymbolTable.ConnectCodeObject(function, method);
                    //symbolTable.Add(function.Name, SymbolType.Function, function, method);
                }
            }

//            if (isClass)
//            {
//                foreach (var field in typeBody.Fields)
//                {
//                    var fieldCodeObj = tb.DefineField(field.Type.Type, field.Name, false, false);
//                    typeBody.SymbolTable.ConnectCodeObject(field, fieldCodeObj);
//                }
//
//                foreach (var property in typeBody.Properties)
//                {
//                    var propBuilder = tb.DefineProperty(property.Type.Type, property.Name);
//
//                    IField backField = null;
//
//                    if (property.Setter == null && property.Getter == null)
//                    {
//                        backField = tb.DefineField(property.Type.Type, $"{property.Name}_backingField", false,
//                            false);
//                    }
//
//                    var getMethod = tb.DefineMethod($"get_{property.Name}", true, false, false);
//                    var setMethod = tb.DefineMethod($"set_{property.Name}", true, false, false);
//
//                    setMethod.WithReturnType(_bindings.Void);
//                    var valueArg = setMethod.WithParameter("value", property.Type.Type, false, false);
//
//                    getMethod.WithReturnType(property.Type.Type);
//
//                    if (property.Getter != null)
//                    {
//                        IEmitter emitter = getMethod.Generator;
//                        emitter.InitLocals = true;
//
//                        ILocal resultVar = null;
//
//                        resultVar = emitter.DefineLocal(property.Type.Type);
//
//                        var returnLabel = emitter.DefineLabel();
//                        EmitBody(emitter, property.Getter, returnLabel, ref resultVar);
//
//                        emitter.MarkLabel(returnLabel);
//
//                        if (resultVar != null)
//                            emitter.LdLoc(resultVar);
//
//                        emitter.Ret();
//                    }
//                    else
//                    {
//                        getMethod.Generator.LdArg_0().LdFld(backField).Ret();
//                    }
//
//                    if (property.Setter != null)
//                    {
//                        IEmitter emitter = setMethod.Generator;
//                        emitter.InitLocals = true;
//
//                        ILocal resultVar = null;
//
//                        resultVar = emitter.DefineLocal(property.Type.Type);
//
//                        var valueSym = property.Setter.SymbolTable.Find("value", SymbolType.Variable);
//                        valueSym.CodeObject = valueArg;
//
//                        var returnLabel = emitter.DefineLabel();
//                        EmitBody(emitter, property.Setter, returnLabel, ref resultVar);
//
//                        emitter.MarkLabel(returnLabel);
//                        emitter.Ret();
//                    }
//                    else
//                    {
//                        if (backField != null)
//                            setMethod.Generator.LdArg_0().LdArg(1).StFld(backField).Ret();
//                        else
//                            setMethod.Generator.Ret();
//                    }
//
//
//                    propBuilder.WithGetter(getMethod).WithSetter(setMethod);
//                }
//            }

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
                    var codeObj = method.WithParameter(p.Name, null, false, false);
                    function.Block.SymbolTable.ConnectCodeObject(p, codeObj);
                }
            }

            function.Builder = method.Generator;

            EmitFunction(function);
            return method;
        }
    }
}