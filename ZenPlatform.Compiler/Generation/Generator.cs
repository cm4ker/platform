using System;
using System.Collections.Generic;
using System.Globalization;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Expression;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using OpCode = Mono.Cecil.Cil.OpCode;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;
using Type = System.Type;
using TypeAttributes = Mono.Cecil.TypeAttributes;


namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private Module _module = null;
        private readonly AssemblyDefinition _asm;
        private ModuleDefinition _dllModule = null;
        private SymbolTable _typeSymbols = null;

        public Generator(Module module, AssemblyDefinition asm)
        {
            _module = module;
            _asm = asm;
        }

        public void Emit()
        {
            _dllModule = _asm.MainModule;

            TypeDefinition td = new TypeDefinition("CompileNamespace", _module.Name,
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Abstract |
                TypeAttributes.BeforeFieldInit | TypeAttributes.AnsiClass,
                _dllModule.TypeSystem.Object);


            _dllModule.Types.Add(td);
            //
            // Create global variables.
            //

            _typeSymbols = new SymbolTable();

            _module.TypeBody.SymbolTable = _typeSymbols;

            // Сделаем прибилд функции, чтобы она зерегистрировала себя в доступных символах модуля
            // Для того, чтобы можно было делать вызов функции из другой функции
            foreach (var item in PrebuildFunctions(_module.TypeBody))
            {
                BuildFunction(item.Item1, item.Item2);
                td.Methods.Add(item.Item2);
            }
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

        private void EmitExpression(ILProcessor il, Expression expression, SymbolTable symbolTable)
        {
            if (expression is BinaryExpression)
            {
                EmitExpression(il, ((BinaryExpression) expression).Left, symbolTable);
                EmitExpression(il, ((BinaryExpression) expression).Right, symbolTable);

                switch (((BinaryExpression) expression).BinaryOperatorType)
                {
                    case BinaryOperatorType.Add:
                        il.Emit(OpCodes.Add);
                        break;
                    case BinaryOperatorType.Subtract:
                        il.Emit(OpCodes.Sub);
                        break;
                    case BinaryOperatorType.Multiply:
                        il.Emit(OpCodes.Mul);
                        break;
                    case BinaryOperatorType.Divide:
                        il.Emit(OpCodes.Div);
                        break;
                    case BinaryOperatorType.Modulo:
                        il.Emit(OpCodes.Rem);
                        break;
                    case BinaryOperatorType.Equal:
                        il.Emit(OpCodes.Ceq);
                        break;
                    case BinaryOperatorType.NotEqual:
                        il.Emit(OpCodes.Ceq);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Ceq);
                        break;
                    case BinaryOperatorType.GreaterThen:
                        il.Emit(OpCodes.Cgt);
                        break;
                    case BinaryOperatorType.LessThen:
                        il.Emit(OpCodes.Clt);
                        break;
                    case BinaryOperatorType.GraterOrEqualTo:
                        il.Emit(OpCodes.Clt);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Ceq);
                        break;
                    case BinaryOperatorType.LessOrEqualTo:
                        il.Emit(OpCodes.Cgt);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Ceq);
                        break;
                    case BinaryOperatorType.And:
                        il.Emit(OpCodes.And);
                        break;
                    case BinaryOperatorType.Or:
                        il.Emit(OpCodes.Or);
                        break;
                }
            }
            else if (expression is UnaryExpression ue)
            {
                if (ue is IndexerExpression ie)
                {
                    EmitExpression(il, ie.Value, symbolTable);
                    EmitExpression(il, ie.Indexer, symbolTable);
                    il.Emit(OpCodes.Ldelem_I4);
                }

                if (ue is LogicalOrArithmeticExpression lae)

                    switch (lae.Type)
                    {
                        case UnaryOperatorType.Indexer:

                            break;
                        case UnaryOperatorType.Negative:
                            EmitExpression(il, lae.Value, symbolTable);
                            il.Emit(OpCodes.Neg);
                            break;
                        case UnaryOperatorType.Not:
                            EmitExpression(il, lae.Value, symbolTable);
                            il.Emit(OpCodes.Not);
                            break;
                    }

                if (ue is CastExpression ce)
                {
                    EmitExpression(il, ce.Value, symbolTable);
                    EmitConvert(il, ce, symbolTable);
                }
            }
            else if (expression is Literal literal)
            {
                switch (literal.Type.PrimitiveType)
                {
                    case PrimitiveType.Integer:
                        il.Emit(OpCodes.Ldc_I4, Int32.Parse(literal.Value));
                        break;
                    case PrimitiveType.String:
                        il.Emit(OpCodes.Ldstr, literal.Value);
                        break;
                    case PrimitiveType.Double:
                        il.Emit(OpCodes.Ldc_R8,
                            double.Parse(literal.Value, CultureInfo.InvariantCulture));
                        break;
                    case PrimitiveType.Character:
                        il.Emit(OpCodes.Ldc_I4, char.ConvertToUtf32(literal.Value, 0));
                        break;
                    case PrimitiveType.Boolean:
                        if (literal.Value == "true")
                            il.Emit(OpCodes.Ldc_I4_S, (byte) 1);
                        else if (literal.Value == "false")
                            il.Emit(OpCodes.Ldc_I4_S, (byte) 0);
                        break;
                }
            }
            else if (expression is Name name)
            {
                Symbol variable = symbolTable.Find(name.Value, SymbolType.Variable);
                if (variable == null)
                    Error("Assignment variable " + name.Value + " unknown.");

                if (variable.CodeObject is VariableDefinition)
                    il.Emit(OpCodes.Ldloc, (VariableDefinition) variable.CodeObject);
                else if (variable.CodeObject is FieldDefinition)
                    il.Emit(OpCodes.Ldsfld, (FieldDefinition) variable.CodeObject);
                else if (variable.CodeObject is ParameterDefinition)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    il.Emit(OpCodes.Ldarg_S,
                        (byte) ((ParameterDefinition) variable.CodeObject).Sequence);
                    if (p.PassMethod == PassMethod.ByReference)
                        il.Emit(OpCodes.Ldind_I4);
                }
            }
            else if (expression is Call call)
            {
                EmitCall(il, call, symbolTable);
            }
        }

        // Used to keep all function names unique.
        private SymbolTable m_Functions = new SymbolTable();

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
                                                | MethodAttributes.HideBySig,
                        ToCecilType(function.Type.ToSystemType()));

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
            while (m_Functions.Find(functionName, SymbolType.Function) != null)
                functionName += "#";

            // Find return type.
            Type returnType = function.Type.ToSystemType();

            // Find parameters.
            Type[] parameters = null;
            if (function.Parameters != null)
            {
                parameters = new Type[function.Parameters.Count];

                for (int x = 0; x < function.Parameters.Count; x++)
                {
                    parameters[x] = function.Parameters[x].Type.ToSystemType();
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
                        ToCecilType(pType.ToSystemType()));

                    function.InstructionsBody.SymbolTable.Add(pName, SymbolType.Variable, function.Parameters[x], p);

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


            var returnVariable = new VariableDefinition(ToCecilType(function.Type.ToSystemType()));
            var returnInstruction = il.Create(OpCodes.Ldloc, returnVariable);

            var isVoid = function.Type == null || function.Type.PrimitiveType == PrimitiveType.Void;

            if (!isVoid)
            {
                il.Body.Variables.Add(returnVariable);
            }

            EmitBody(il, function.InstructionsBody, returnInstruction, returnVariable);


            if (!isVoid)
                il.Append(returnInstruction);
            il.Emit(OpCodes.Ret);

//            if (function.Type == null || function.Type.PrimitiveType == PrimitiveType.Void)
//                il.Append(Mono.Cecil.Cil.OpCodes.Ret);
        }


        private void EmitConvert(ILProcessor il, CastExpression expression, SymbolTable symbolTable)
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
            var convertType = expression.Type;

            if (valueType.VariableType == VariableType.Primitive && convertType.VariableType == VariableType.Primitive)
            {
                var opCode = GetConvCodeFromType(convertType);
                il.Emit(opCode);
            }
        }

        private OpCode GetConvCodeFromType(AST.Definitions.Type type)
        {
            if (type.PrimitiveType == PrimitiveType.Integer) return OpCodes.Conv_I4;
            if (type.PrimitiveType == PrimitiveType.Double) return OpCodes.Conv_R8;
            if (type.PrimitiveType == PrimitiveType.Character) return OpCodes.Conv_U2;
            if (type.PrimitiveType == PrimitiveType.Real) return OpCodes.Conv_R4;

            throw new Exception("Converting to this value not supported");
        }

        private OpCode GetLdcCodeFromType(AST.Definitions.Type type)
        {
            switch (type.PrimitiveType)
            {
                case PrimitiveType.Integer: return OpCodes.Ldc_I4;
                case PrimitiveType.Double: return OpCodes.Ldc_R8;
                case PrimitiveType.Real: return OpCodes.Ldc_R4;
                default: return OpCodes.Ldc_I4;
            }
        }
    }
}