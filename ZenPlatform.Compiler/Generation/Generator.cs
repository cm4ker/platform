using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Expressions;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Cecil.Backend;
using TypeResolver = ZenPlatform.Compiler.Cecil.Backend.TypeResolver;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private readonly CompilationUnit _compilationUnit;
        private readonly AssemblyDefinition _asm;
        private ModuleDefinition _dllModule;

        private SymbolTable _typeSymbols;
        private SymbolTable _functions = new SymbolTable();

        private TypeResolver _typeResolver;

        private const string ASM_NAMESPACE = "CompileNamespace";


        public Generator(CompilationUnit compilationUnit, AssemblyDefinition asm)
        {
            _compilationUnit = compilationUnit;

            AstSymbolVisitor asv = new AstSymbolVisitor();
            asv.Visit(_compilationUnit);

            _asm = asm;

            _typeResolver = new TypeResolver(compilationUnit, _asm);

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
            _dllModule = _asm.MainModule;

            var td = new TypeDefinition(ASM_NAMESPACE, module.Name,
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Abstract |
                TypeAttributes.BeforeFieldInit | TypeAttributes.AnsiClass,
                _dllModule.TypeSystem.Object);


            _dllModule.Types.Add(td);

            _typeSymbols = new SymbolTable();

            module.TypeBody.SymbolTable = _typeSymbols;

            // Сделаем прибилд функции, чтобы она зерегистрировала себя в доступных символах модуля
            // Для того, чтобы можно было делать вызов функции из другой функции
            foreach (var item in PrebuildFunctions(module.TypeBody))
            {
                BuildFunction(item.Item1, item.Item2);
                td.Methods.Add(item.Item2);
            }
        }

        private void EmitClass(Class @class)
        {
            _dllModule = _asm.MainModule;

            TypeDefinition td = new TypeDefinition(ASM_NAMESPACE, @class.Name,
                TypeAttributes.Class | TypeAttributes.NotPublic |
                TypeAttributes.BeforeFieldInit | TypeAttributes.AnsiClass,
                _dllModule.TypeSystem.Object);


            _dllModule.Types.Add(td);

            _typeSymbols = new SymbolTable();

            @class.TypeBody.SymbolTable = _typeSymbols;

            // Сделаем прибилд функции, чтобы она зерегистрировала себя в доступных символах модуля
            // Для того, чтобы можно было делать вызов функции из другой функции
            foreach (var item in PrebuildFunctions(@class.TypeBody))
            {
                BuildFunction(item.Item1, item.Item2);
                td.Methods.Add(item.Item2);
            }
        }

        public void Emit()
        {
        }

        private void Error(string message)
        {
            throw new Exception(message);
        }


        private TypeReference ToCecilType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32: return _dllModule.TypeSystem.Int32;
                case TypeCode.Boolean: return _dllModule.TypeSystem.Boolean;
                case TypeCode.String: return _dllModule.TypeSystem.String;
                case TypeCode.Double: return _dllModule.TypeSystem.Double;
                case TypeCode.Char: return _dllModule.TypeSystem.Char;
                case TypeCode.Object when type == typeof(void): return _dllModule.TypeSystem.Void;
                case TypeCode.Object: return _dllModule.ImportReference(type);
                default:
                {
                    return null;
                }
            }
        }

        private void EmitExpression(Emitter e, Expression expression, SymbolTable symbolTable)
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
                switch (literal.Type)
                {
                    case ZInt t:
                        e.LdcI4(Int32.Parse(literal.Value));
                        break;
                    case ZString t:
                        e.LdStr(literal.Value);
                        break;
                    case ZDouble t:
                        e.LdcR8(double.Parse(literal.Value, CultureInfo.InvariantCulture));
                        break;
                    case ZCharacter t:
                        e.LdcI4(char.ConvertToUtf32(literal.Value, 0));
                        break;
                    case ZBool t:
                        if (literal.Value == "true")
                            e.LdcI4(1);
                        else if (literal.Value == "false")
                            e.LdcI4(0);
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

                if (variable.CodeObject is VariableDefinition vd)
                    e.LdLoc(vd);
                else if (variable.CodeObject is FieldDefinition fd)
                    e.LdsFld(fd);
                else if (variable.CodeObject is ParameterDefinition pd)
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

                //TODO: Необходим лукап типа в сборке через cecil, оттуда уже забирать проперти

                TypeDefinition td;

                if (fe.Expression.Type.IsArray)
                {
                    var asmCoreDefinition =
                        _dllModule.AssemblyResolver.Resolve((AssemblyNameReference) _dllModule.TypeSystem.CoreLibrary);
                    td = asmCoreDefinition.MainModule.GetType("System", "Array").Resolve();
                }
                else
                    td = _typeResolver.Resolve(fe.Expression.Type).Resolve();

                try
                {
                    var fr = td.Fields.FirstOrDefault(x => x.Name == fe.Name) ??
                             throw new Exception("Field not found: " + fe.Name);
                    e.LdFld(fr);
                }
                catch
                {
                    var pr = td.Properties.FirstOrDefault(x => x.Name == fe.Name) ??
                             throw new Exception("Field not found: " + fe.Name);
                    var md = _dllModule.ImportReference(pr.GetMethod);

                    e.Call(md);
                }
            }
        }


        /// <summary>
        /// Создание функций исходя из тела типа
        /// </summary>
        /// <param name="typeBody"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private List<(Function, MethodDefinition)> PrebuildFunctions(TypeBody typeBody)
        {
            if (typeBody == null)
                throw new ArgumentNullException();


            List<(Function, MethodDefinition)> result = new List<(Function, MethodDefinition)>();

            if (typeBody != null && typeBody.Functions != null)
            {
                foreach (Function function in typeBody.Functions)
                {
                    //Для каждого метода создаём свою таблицу символов
                    SymbolTable symbolTable = new SymbolTable(_typeSymbols);

                    // Make child visible to sibillings
                    function.InstructionsBody.SymbolTable = symbolTable;

                    // Create method.
                    MethodDefinition method = new MethodDefinition(function.Name,
                        MethodAttributes.Public | MethodAttributes.Static
                                                | MethodAttributes.HideBySig, _typeResolver.Resolve(function.Type));

                    result.Add((function, method));

                    // Make child visible to parent.
                    typeBody.SymbolTable.Add(function.Name, SymbolType.Function, function, method);
                    symbolTable.Add(function.Name, SymbolType.Function, function, method);
                }
            }

            return result;
        }

        private MethodDefinition BuildFunction(Function function, MethodDefinition method)
        {
            if (function == null)
                throw new ArgumentNullException();
            //
            // Build function stub.
            //

            // Find an unique name.
            string functionName = function.Name;
            while (_functions.Find(functionName, SymbolType.Function) != null)
                functionName += "#";

            // Find parameters.
            TypeReference[] parameters = null;
            if (function.Parameters != null)
            {
                parameters = new TypeReference[function.Parameters.Count];

                for (int x = 0; x < function.Parameters.Count; x++)
                {
                    parameters[x] = _typeResolver.Resolve(function.Parameters[x].Type);
                }
            }

            function.Builder = method.Body.GetILProcessor();

            if (function.Parameters != null)
            {
                for (int x = 0; x < function.Parameters.Count; x++)
                {
                    var pType = function.Parameters[x].Type;
                    var pName = function.Parameters[x].Name;


                    ParameterDefinition p = new ParameterDefinition(pName, ParameterAttributes.None,
                        parameters[x]);

                    function.InstructionsBody.SymbolTable.Add(pName, SymbolType.Variable,
                        function.Parameters[x], p);

                    method.Parameters.Add(p);
                }
            }

            EmitFunction(function);

            return method;
        }

        private void EmitFunction(Function function)
        {
            if (function == null)
                throw new ArgumentNullException();

            ILProcessor il = function.Builder;
            il.Body.InitLocals = true;

            var emitter = new Emitter(il);


            var returnVariable = new VariableDefinition(_typeResolver.Resolve(function.Type));
            var returnInstruction = new Label(il.Create(OpCodes.Ldloc, returnVariable));

            var isVoid = function.Type == null || function.Type is ZVoid;

            if (!isVoid)
            {
                il.Body.Variables.Add(returnVariable);
            }

            EmitBody(emitter, function.InstructionsBody, returnInstruction, returnVariable);


            if (!isVoid)
                emitter.Append(returnInstruction);
            il.Emit(OpCodes.Ret);

//            if (function.Type == null || function.Type.PrimitiveType == PrimitiveType.Void)
//                il.Append(Mono.Cecil.Cil.OpCodes.Ret);
        }


        private void EmitConvert(Emitter e, CastExpression expression, SymbolTable symbolTable)
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

            var valueType = expression.Value.Type;

            if (expression.Value is IndexerExpression && valueType.IsArray && valueType is ZArray a)
            {
                valueType = a.TypeOfElements;
            }

            var convertType = expression.Type;

            if (valueType is null || (valueType.IsSystem && convertType.IsSystem))
            {
                EmitConvCode(e, convertType);
            }
        }

        private void EmitConvCode(Emitter e, ZType type)
        {
            switch (type)
            {
                case ZInt t:
                    e.ConvI4();
                    return;
                case ZDouble t:
                    e.ConvR8();
                    return;
                case ZCharacter t:
                    e.ConvU2();
                    return;
            }

            throw new Exception("Converting to this value not supported");
        }

        private void EmitIncrement(Emitter e, ZType type)
        {
            EmitAddValue(e, type, 1);
        }

        private void EmitDecrement(Emitter e, ZType type)
        {
            EmitAddValue(e, type, -1);
        }

        private void EmitAddValue(Emitter e, ZType type, int value)
        {
            switch (type)
            {
                case ZInt t:
                    e.LdcI4(value);
                    break;
                case ZDouble d:
                    e.LdcR8(value);
                    break;
                default:
                    e.LdcI4(value);
                    break;
            }
        }
    }
}