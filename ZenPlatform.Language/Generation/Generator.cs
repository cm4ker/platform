using System;
using System.Collections.Generic;
using System.Globalization;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatform.Language.AST.Definitions;
using ZenPlatform.Language.AST.Definitions.Expression;
using ZenPlatform.Language.AST.Definitions.Functions;
using ZenPlatform.Language.AST.Definitions.Symbols;
using ZenPlatform.Language.AST.Infrastructure;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using OpCode = Mono.Cecil.Cil.OpCode;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;
using Type = System.Type;
using TypeAttributes = Mono.Cecil.TypeAttributes;


namespace ZenPlatform.Language.Generation
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
                case TypeCode.Object: return _dllModule.ImportReference(type);
                default:
                {
                    if (type == typeof(void))
                        return _dllModule.TypeSystem.Void;
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
                        il.Emit(Mono.Cecil.Cil.OpCodes.Add);
                        break;
                    case BinaryOperatorType.Subtract:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Sub);
                        break;
                    case BinaryOperatorType.Multiply:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Mul);
                        break;
                    case BinaryOperatorType.Divide:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Div);
                        break;
                    case BinaryOperatorType.Modulo:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Rem);
                        break;
                    case BinaryOperatorType.Equal:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ceq);
                        break;
                    case BinaryOperatorType.NotEqual:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ceq);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_0);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ceq);
                        break;
                    case BinaryOperatorType.GreaterThen:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Cgt);
                        break;
                    case BinaryOperatorType.LessThen:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Clt);
                        break;
                    case BinaryOperatorType.GraterOrEqualTo:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Clt);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_0);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ceq);
                        break;
                    case BinaryOperatorType.LessOrEqualTo:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Cgt);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_0);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ceq);
                        break;
                    case BinaryOperatorType.And:
                        il.Emit(Mono.Cecil.Cil.OpCodes.And);
                        break;
                    case BinaryOperatorType.Or:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Or);
                        break;
                }
            }
            else if (expression is UnaryExpression ue)
            {
                if (ue is IndexerExpression ie)
                {
                    EmitExpression(il, ie.Value, symbolTable);
                    EmitExpression(il, ie.Indexer, symbolTable);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldelem_I4);
                }

                if (ue is LogicalOrArithmeticExpression lae)

                    switch (lae.Type)
                    {
                        case UnaryOperatorType.Indexer:

                            break;
                        case UnaryOperatorType.Negative:
                            EmitExpression(il, lae.Value, symbolTable);
                            il.Emit(Mono.Cecil.Cil.OpCodes.Neg);
                            break;
                        case UnaryOperatorType.Not:
                            EmitExpression(il, lae.Value, symbolTable);
                            il.Emit(Mono.Cecil.Cil.OpCodes.Not);
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
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, Int32.Parse(literal.Value));
                        break;
                    case PrimitiveType.String:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldstr, literal.Value);
                        break;
                    case PrimitiveType.Double:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_R8,
                            double.Parse(literal.Value, CultureInfo.InvariantCulture));
                        break;
                    case PrimitiveType.Character:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, char.ConvertToUtf32(literal.Value, 0));
                        break;
                    case PrimitiveType.Boolean:
                        if (literal.Value == "true")
                            il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_S, (byte) 1);
                        else if (literal.Value == "false")
                            il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_S, (byte) 0);
                        break;
                }
            }
            else if (expression is Name name)
            {
                Symbol variable = symbolTable.Find(name.Value, SymbolType.Variable);
                if (variable == null)
                    Error("Assignment variable " + name.Value + " unknown.");

                if (variable.CodeObject is VariableDefinition)
                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, (VariableDefinition) variable.CodeObject);
                else if (variable.CodeObject is FieldDefinition)
                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldsfld, (FieldDefinition) variable.CodeObject);
                else if (variable.CodeObject is ParameterDefinition)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_S,
                        (byte) ((ParameterDefinition) variable.CodeObject).Sequence);
                    if (p.PassMethod == PassMethod.ByReference)
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldind_I4);
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
            System.Type returnType = function.Type.ToSystemType();

            // Find parameters.
            System.Type[] parameters = null;
            if (function.Parameters != null)
            {
                parameters = new System.Type[function.Parameters.Count];

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

            //
            // Build function body.
            //

            ILProcessor il = function.Builder;

            il.Body.InitLocals = true;

            EmitBody(il, function.InstructionsBody);


            if (function.Type == null || function.Type.PrimitiveType == PrimitiveType.Void)
                il.Emit(Mono.Cecil.Cil.OpCodes.Ret);
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
                var opCode = GetOpCodeFromType(convertType);
                il.Emit(opCode);
            }
        }

        private OpCode GetOpCodeFromType(AST.Definitions.Type type)
        {
            if (type.PrimitiveType == PrimitiveType.Integer) return Mono.Cecil.Cil.OpCodes.Conv_I4;
            if (type.PrimitiveType == PrimitiveType.Double) return Mono.Cecil.Cil.OpCodes.Conv_R8;
            if (type.PrimitiveType == PrimitiveType.Character) return Mono.Cecil.Cil.OpCodes.Conv_U2;
            if (type.PrimitiveType == PrimitiveType.Real) return Mono.Cecil.Cil.OpCodes.Conv_R4;

            throw new Exception("Converting to this value not supported");
        }
    }
}