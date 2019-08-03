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

        private void EmitExpression(IEmitter e, Expression expression, SymbolTable symbolTable)
        {
            if (expression is BinaryExpression be)
            {
                EmitExpression(e, ((BinaryExpression) expression).Left, symbolTable);
                EmitExpression(e, ((BinaryExpression) expression).Right, symbolTable);

                if (be.Type.IsNumeric())
                    switch (be.BinaryOperatorType)
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
                    }

                if (be.Type.IsString())
                    switch (be.BinaryOperatorType)
                    {
                        case BinaryOperatorType.Add:
                            e.EmitCall(_bindings.Methods.Concat);
                            //e.EmitCall(_bindings.String.FindMethod(x => x.Name == "Concat" && x.Parameters.Count == 2));
                            break;
                        default: throw new NotSupportedException();
                    }

                if (be.Type.IsBoolean())
                {
                    switch (be.BinaryOperatorType)
                    {
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
                            e.And();
                            break;
                        case BinaryOperatorType.Or:
                            e.Or();
                            break;
                    }
                }
            }
            else if (expression is UnaryExpression ue)
            {
                if (ue is IndexerExpression ie)
                {
                    EmitExpression(e, ie.Expression, symbolTable);
                    EmitExpression(e, ie.Indexer, symbolTable);
                    e.LdElemI4();
                }

                if (ue is LogicalOrArithmeticExpression lae)

                    switch (lae.OperaotrType)
                    {
                        case UnaryOperatorType.Indexer:

                            break;
                        case UnaryOperatorType.Negative:
                            EmitExpression(e, lae.Expression, symbolTable);
                            e.Neg();
                            break;
                        case UnaryOperatorType.Not:
                            EmitExpression(e, lae.Expression, symbolTable);
                            e.Not();
                            break;
                    }

                if (ue is CastExpression ce)
                {
                    EmitExpression(e, ce.Expression, symbolTable);
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
                    if (name.Type is UnionTypeSyntax)
                    {
                        e.LdLocA(vd);
                        e.EmitCall(_bindings.UnionTypeStorage.FindProperty("Value").Getter);
                    }
                    else
                        e.LdLoc(vd);
                }
                else if (variable.CodeObject is IField fd)
                {
                    e.LdArg_0();
                    e.LdFld(fd);
                }
                else if (variable.CodeObject is IParameter pd)
                {
                    Parameter p = variable.SyntaxObject as Parameter;

                    if (name.Type is UnionTypeSyntax)
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

                IType extTypeScan = expType.ToClrType(_asm);

                var expProp = extTypeScan.Properties.First(x => x.Name == fe.FieldName);

                if (expProp.PropertyType.IsArray)
                    fe.Type = new ArrayTypeSyntax(null,
                        new SingleTypeSyntax(null, expProp.PropertyType.ArrayElementType.Name, TypeNodeKind.Unknown));
                else
                    fe.Type = new SingleTypeSyntax(null, expProp.PropertyType.Name, TypeNodeKind.Unknown);

                e.PropGetValue(expProp);
            }
            else if (expression is PostIncrementExpression pis)
            {
                var symbol = symbolTable.Find(pis.Name.Value, SymbolType.Variable) ??
                             throw new Exception($"Variable {pis.Name} not found");

                IType opType = null;
//                if (symbol.SyntaxObject is Parameter p)
//                    opType = p.Type.Type;
//                else if (symbol.SyntaxObject is Variable v)
//                    opType = v.Type.Type;


                EmitExpression(e, pis.Name, symbolTable);

                EmitIncrement(e, opType);
                e.Add();

                if (symbol.CodeObject is IParameter pd)
                    e.StArg(pd);

                if (symbol.CodeObject is ILocal vd)
                    e.StLoc(vd);
            }
            else if (expression is PostDecrementExpression pds)
            {
                var symbol = symbolTable.Find(pds.Name.Value, SymbolType.Variable) ??
                             throw new Exception($"Variable {pds.Name} not found");

                IType opType = null;
//                if (symbol.SyntaxObject is Parameter p)
//                    opType = p.Type.Type;
//                else if (symbol.SyntaxObject is Variable v)
//                    opType = v.Type.Type;


                EmitExpression(e, pds.Name, symbolTable);

                EmitDecrement(e, opType);
                e.Add();

                if (symbol.CodeObject is IParameter pd)
                    e.StArg(pd);

                if (symbol.CodeObject is ILocal vd)
                    e.StLoc(vd);
            }
            else if (expression is Variable variable)
            {
                EmitVariable(e, symbolTable, variable);
            }
            else if (expression is Throw th)
            {
                EmitExpression(e, th.Exception, symbolTable);
                var constructor = _ts.FindType<Exception>().FindConstructor(_bindings.String);
                e.NewObj(constructor);
                e.Throw();
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