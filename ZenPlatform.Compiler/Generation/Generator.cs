using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Expressions;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using SreTA = System.Reflection.TypeAttributes;


namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private readonly CompilationUnit _compilationUnit;
        private readonly IAssemblyBuilder _asm;
        private readonly ITypeSystem _ts;


//        private SymbolTable _typeSymbols;
//        private SymbolTable _functions = new SymbolTable();

        private SystemTypeBindings _bindings;

        private const string ASM_NAMESPACE = "CompileNamespace";


        public Generator(CompilationUnit compilationUnit, IAssemblyBuilder asm)
        {
            _compilationUnit = compilationUnit;
            _asm = asm;
            _ts = asm.TypeSystem;

            _bindings = new SystemTypeBindings(_ts);

            foreach (var typeEntity in compilationUnit.TypeEntities)
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
                        throw new Exception("The type entity not supproted");
                }
            }
        }


        private void BuildModule(Module module)
        {
            var typeBuilder = _asm.DefineType(ASM_NAMESPACE, module.Name,
                SreTA.Class | SreTA.Public | SreTA.Abstract |
                SreTA.BeforeFieldInit | SreTA.AnsiClass, _bindings.Object);

            //_typeSymbols = new SymbolTable();

            //module.TypeBody.SymbolTable = _typeSymbols;

            // Сделаем прибилд функции, чтобы она зерегистрировала себя в доступных символах модуля
            // Для того, чтобы можно было делать вызов функции из другой функции
            foreach (var item in PrebuildFunctions(module.TypeBody, typeBuilder))
            {
                BuildFunction(item.Item1, item.Item2);
            }

            typeBuilder.EndBuild();
        }

        private void EmitClass(Class @class)
        {
            var tb = _asm.DefineType(ASM_NAMESPACE, @class.Name,
                SreTA.Class | SreTA.NotPublic |
                SreTA.BeforeFieldInit | SreTA.AnsiClass,
                _bindings.Object);

            //_typeSymbols = new SymbolTable();

            //@class.TypeBody.SymbolTable = _typeSymbols;

            // Сделаем прибилд функции, чтобы она зерегистрировала себя в доступных символах модуля
            // Для того, чтобы можно было делать вызов функции из другой функции
            foreach (var item in PrebuildFunctions(@class.TypeBody, tb))
            {
                BuildFunction(item.Item1, item.Item2);
            }
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
                if (literal.Type.Type.IsSystem)
                    switch (literal.Type.Type.Name)
                    {
                        case "Int32":
                            e.LdcI4(Int32.Parse(literal.Value));
                            break;
                        case "String":
                            e.LdStr(literal.Value);
                            break;
                        case "Double":
                            e.LdcR8(double.Parse(literal.Value, CultureInfo.InvariantCulture));
                            break;
                        case "Char":
                            e.LdcI4(char.ConvertToUtf32(literal.Value, 0));
                            break;
                        case "Boolean":
                            e.LdcI4(bool.Parse(literal.Value) ? 1 : 0);
                            break;
                    }
            }
            else if (expression is Name name)
            {
                Symbol variable = symbolTable.Find(name.Value, SymbolType.Variable);

                if (variable == null)
                    Error("Assignment variable " + name.Value + " unknown.");

                if (name.Type is null)
                    if (variable.SyntaxObject is ITypedNode tn)
                        name.Type = tn.Type;

                if (variable.CodeObject is ILocal vd)
                    e.LdLoc(vd);
                else if (variable.CodeObject is IField fd)
                    e.LdsFld(fd);
                else if (variable.CodeObject is IParameter pd)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    e.LdArg(pd.Sequence);
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
                var expType = fe.Expression.Type.Type;
                var expProp = expType.Properties.First(x => x.Name == fe.Name);
                fe.Type = new TypeNode(null, expProp.PropertyType);
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
        private List<(Function, IMethodBuilder)> PrebuildFunctions(TypeBody typeBody, ITypeBuilder tb)
        {
            if (typeBody == null)
                throw new ArgumentNullException();


            List<(Function, IMethodBuilder)> result = new List<(Function, IMethodBuilder)>();

            if (typeBody != null && typeBody.Functions != null)
            {
                foreach (Function function in typeBody.Functions)
                {
                    //Для каждого метода создаём свою таблицу символов
                    //SymbolTable symbolTable = new SymbolTable(_typeSymbols);

                    // Make child visible to sibillings
                    //function.InstructionsBody.SymbolTable = symbolTable;

                    var method = tb.DefineMethod(function.Name, true, true, false)
                        .WithReturnType(function.Type.Type);

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
            //
            // Build function stub.
            //

//            // Find an unique name.
//            string functionName = function.Name;
//            while (_functions.Find(functionName, SymbolType.Function) != null)
//                functionName += "#";

            if (function.Parameters != null)
            {
                foreach (var p in function.Parameters)
                {
                    method.WithParameter(p.Name, p.Type.Type, false, false);
                }
            }

            function.Builder = method.Generator;


            EmitFunction(function);

            return method;
        }

        private void EmitFunction(Function function)
        {
            if (function == null)
                throw new ArgumentNullException();

            IEmitter emitter = function.Builder;
            emitter.InitLocals = true;

            var resultVar = emitter.DefineLocal(function.Type.Type);
            var returnLabel = emitter.DefineLabel();
            EmitBody(emitter, function.InstructionsBody, returnLabel, resultVar);
            emitter.MarkLabel(returnLabel);
            emitter.Ret();
        }

        private void EmitConvert(IEmitter e, CastExpression expression, SymbolTable symbolTable)
        {
            if (expression.Value is Name name)
            {
                Symbol variable = symbolTable.Find(name.Value, SymbolType.Variable);
                if (variable == null)
                    Error("Assignment variable " + name.Value + " unknown.");

                if (variable.SyntaxObject is Variable v)
                    expression.Value.Type = v.Type;
                else if (variable.SyntaxObject is Parameter p)
                    expression.Value.Type = p.Type;
            }

            var valueType = expression.Value.Type.Type;

            if (expression.Value is IndexerExpression && valueType.IsArray)
            {
                valueType = valueType.ArrayElementType;
            }

            var convertType = expression.Type.Type;

            if (valueType is null || (valueType.IsValueType && convertType.IsValueType))
            {
                EmitConvCode(e, convertType);
            }
        }

        private void EmitConvCode(IEmitter e, IType type)
        {
            if (type.Equals(_bindings.Int))
                e.ConvI4();
            else if (type.Equals(_bindings.Double))
                e.ConvR8();
            else if (type.Equals(_bindings.Char))
                e.ConvU2();
            else
                throw new Exception("Converting to this value not supported");
        }

        private void EmitIncrement(IEmitter e, IType type)
        {
            EmitAddValue(e, type, 1);
        }

        private void EmitDecrement(IEmitter e, IType type)
        {
            EmitAddValue(e, type, -1);
        }

        private void EmitAddValue(IEmitter e, IType type, int value)
        {
            if (type == _bindings.Int)
                e.LdcI4(value);
            else if (type == _bindings.Double)
                e.LdcR8(value);
            else if (type == _bindings.Char)
                e.LdcI4(value);
        }
    }
}