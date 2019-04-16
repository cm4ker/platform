using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatfrom.Language.AST.Definitions;
using ZenPlatfrom.Language.AST.Definitions.Expression;
using ZenPlatfrom.Language.AST.Definitions.Functions;
using ZenPlatfrom.Language.AST.Definitions.Statements;
using ZenPlatfrom.Language.AST.Definitions.Symbols;
using ZenPlatfrom.Language.AST.Infrastructure;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using Module = ZenPlatfrom.Language.AST.Definitions.Module;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;
using Type = System.Type;
using TypeAttributes = Mono.Cecil.TypeAttributes;


namespace ZenPlatfrom.Language.Generation
{
    public class Label
    {
        public Instruction Instruction = Instruction.Create(Mono.Cecil.Cil.OpCodes.Nop);
    }

    public class Generator
    {
        private Module _module = null;
        private ModuleDefinition _dllModule = null;
        private SymbolTable _typeSymbols = null;

        public Generator(Module module)
        {
            _module = module;
        }

        public void Compile(string path)
        {
            //
            // Create functions.
            //

            AssemblyDefinition ad = AssemblyDefinition.CreateAssembly(
                new AssemblyNameDefinition("BetaName", Version.Parse("1.0.0.0")), "ZModule", ModuleKind.Dll);

            _dllModule = ad.MainModule;


            TypeDefinition td = new TypeDefinition("CompileNamespace", _module.Name,
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed);

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


            var moduleName = $"{_module.Name}.dll";

            ad.Write(moduleName);

            System.IO.File.Move(moduleName, path);
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

                default:
                {
                    if (type == typeof(void))
                        return _dllModule.TypeSystem.Void;
                    return ToCecilType(type);
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
            else if (expression is UnaryExpression)
            {
                UnaryExpression unaryExpression = expression as UnaryExpression;

                switch (unaryExpression.UnaryOperatorType)
                {
                    case UnaryOperatorType.Indexer:
                        EmitExpression(il, unaryExpression.Value, symbolTable);
                        EmitExpression(il, unaryExpression.Indexer, symbolTable);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldelem_I4);
                        break;
                    case UnaryOperatorType.Negative:
                        EmitExpression(il, unaryExpression.Value, symbolTable);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Neg);
                        break;
                    case UnaryOperatorType.Not:
                        EmitExpression(il, unaryExpression.Value, symbolTable);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Not);
                        break;
                }
            }
            else if (expression is Literal)
            {
                Literal literal = expression as Literal;

                switch (literal.LiteralType)
                {
                    case LiteralType.Integer:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, Int32.Parse(literal.Value));
                        break;
                    case LiteralType.String:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldstr, literal.Value);
                        break;
                    case LiteralType.Real:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_R8,
                            double.Parse(literal.Value, CultureInfo.InvariantCulture));
                        break;
                    case LiteralType.Character:
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, char.ConvertToUtf32(literal.Value, 0));
                        break;
                    case LiteralType.Boolean:
                        if (literal.Value == "true")
                            il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, 1);
                        else if (literal.Value == "false")
                            il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, 0);
                        break;
                }
            }
            else if (expression is Name)
            {
                Name name = expression as Name;

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
            else if (expression is Call)
            {
                EmitCall(il, expression as Call, symbolTable);
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
                        MethodAttributes.Public | MethodAttributes.Static, ToCecilType(function.Type.ToSystemType()));

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

            EmitBody(il, function.InstructionsBody);

            if (function.Type == null || function.Type.PrimitiveType == PrimitiveType.Void)
                il.Emit(Mono.Cecil.Cil.OpCodes.Ret);
        }

        private void EmitBody(ILProcessor il, InstructionsBody body)
        {
            foreach (Statement statement in body.Statements)
            {
                //
                // Declare local variables.
                //

                if (statement is Variable)
                {
                    Variable variable = statement as Variable;

                    VariableDefinition local =
                        new VariableDefinition(ToCecilType(variable.Type.ToSystemType()));
                    il.Body.Variables.Add(local);

                    body.SymbolTable.Add(variable.Name, SymbolType.Variable, variable, local);

                    //
                    // Initialize  variable.
                    //

                    if (variable.Type.VariableType == VariableType.Primitive)
                    {
                        if (variable.Value != null && variable.Value is Expression)
                        {
                            EmitExpression(il, (Expression) variable.Value, body.SymbolTable);

                            il.Emit(Mono.Cecil.Cil.OpCodes.Stloc, local);
                        }
                    }
                    else if (variable.Type.VariableType == VariableType.PrimitiveArray)
                    {
                        // Empty array initialization.
                        if (variable.Value != null && variable.Value is Expression)
                        {
                            EmitExpression(il, (Expression) variable.Value, body.SymbolTable);
                            il.Emit(Mono.Cecil.Cil.OpCodes.Newarr,
                                ToCecilType(variable.Type.ToSystemType()));

                            il.Emit(Mono.Cecil.Cil.OpCodes.Stloc, local);
                        }
                        else if (variable.Value != null && variable.Value is ElementCollection)
                        {
                            ElementCollection elements = variable.Value as ElementCollection;

                            il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, elements.Count);
                            il.Emit(Mono.Cecil.Cil.OpCodes.Newarr,
                                ToCecilType(variable.Type.ToSystemType()));

                            il.Emit(Mono.Cecil.Cil.OpCodes.Stloc, local);

                            for (int x = 0; x < elements.Count; x++)
                            {
                                // Load array
                                il.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, local);
                                // Load index
                                il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, x);
                                // Load value
                                EmitExpression(il, elements[x].Expression, body.SymbolTable);
                                // Store
                                il.Emit(Mono.Cecil.Cil.OpCodes.Stelem_I4);
                            }
                        }
                    }
                }
                else if (statement is Assignment)
                {
                    EmitAssignment(il, statement as Assignment, body.SymbolTable);
                }
                else if (statement is Return)
                {
                    if (((Return) statement).Value != null)
                        EmitExpression(il, ((Return) statement).Value, body.SymbolTable);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Ret);
                }
                else if (statement is CallStatement)
                {
                    CallStatement call = statement as CallStatement;
                    Symbol symbol = body.SymbolTable.Find(call.Name, SymbolType.Function);
                    EmitCallStatement(il, statement as CallStatement, body.SymbolTable);

                    if (symbol != null)
                    {
                        if (((MethodDefinition) symbol.CodeObject).ReturnType != _dllModule.TypeSystem.Void)
                            il.Emit(Mono.Cecil.Cil.OpCodes.Pop);
                    }
                    else
                    {
                        if (call.Name == "Read")
                            il.Emit(Mono.Cecil.Cil.OpCodes.Pop);
                    }
                }
                else if (statement is If)
                {
                    //
                    // Genereate if statement.
                    //

                    If ifStatement = statement as If;

                    // Eval condition
                    EmitExpression(il, ifStatement.Condition, body.SymbolTable);

                    if (ifStatement.IfInstructionsBody != null && ifStatement.ElseInstructionsBody == null)
                    {
                        ifStatement.IfInstructionsBody.SymbolTable = new SymbolTable(body.SymbolTable);


                        var exit = new Label();
                        il.Emit(Mono.Cecil.Cil.OpCodes.Brfalse, exit.Instruction);
                        EmitBody(il, ifStatement.IfInstructionsBody);
                        il.Append(exit.Instruction);
                    }
                    else if (ifStatement.IfInstructionsBody != null && ifStatement.ElseInstructionsBody != null)
                    {
                        ifStatement.IfInstructionsBody.SymbolTable = new SymbolTable(body.SymbolTable);
                        ifStatement.ElseInstructionsBody.SymbolTable = new SymbolTable(body.SymbolTable);
                        Label exit = new Label();
                        Label elseLabel = new Label();
                        il.Emit(Mono.Cecil.Cil.OpCodes.Brfalse, elseLabel.Instruction);
                        EmitBody(il, ifStatement.IfInstructionsBody);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Br, exit.Instruction);
                        il.Append(elseLabel.Instruction);
                        EmitBody(il, ifStatement.ElseInstructionsBody);
                        il.Append(exit.Instruction);
                    }
                }

                else if (statement is While)
                {
                    //
                    // Generate while statement.
                    //

                    While whileStatement = statement as While;
                    whileStatement.InstructionsBody.SymbolTable = new SymbolTable(body.SymbolTable);
                    Label begin = new Label();
                    Label exit = new Label();
                    il.Append(begin.Instruction);
                    // Eval condition
                    EmitExpression(il, whileStatement.Condition, body.SymbolTable);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Brfalse, exit.Instruction);
                    EmitBody(il, whileStatement.InstructionsBody);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Br, begin.Instruction);
                    il.Append(exit.Instruction);
                }
                else if (statement is Do)
                {
                    //
                    // Generate do statement.
                    //

                    Do doStatement = statement as Do;
                    doStatement.InstructionsBody.SymbolTable = new SymbolTable(body.SymbolTable);

                    Label loop = new Label();
                    il.Append(loop.Instruction);
                    EmitBody(il, doStatement.InstructionsBody);
                    EmitExpression(il, doStatement.Condition, body.SymbolTable);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, loop.Instruction);
                }
                else if (statement is For)
                {
                    //
                    // Generate for statement.
                    //

                    For forStatement = statement as For;
                    forStatement.InstructionsBody.SymbolTable = new SymbolTable(body.SymbolTable);

                    Label loop = new Label();
                    Label exit = new Label();

                    // Emit initializer
                    EmitAssignment(il, forStatement.Initializer, body.SymbolTable);
                    il.Append(loop.Instruction);
                    // Emit condition
                    EmitExpression(il, forStatement.Condition, body.SymbolTable);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Brfalse, exit.Instruction);
                    // Emit body
                    EmitBody(il, forStatement.InstructionsBody);
                    // Emit counter
                    EmitAssignment(il, forStatement.Counter, body.SymbolTable);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Br, loop.Instruction);
                    il.Append(exit.Instruction);
                }
            }
        }

        private void EmitAssignment(ILProcessor il, Assignment assignment, SymbolTable symbolTable)
        {
            Symbol variable = symbolTable.Find(assignment.Name, SymbolType.Variable);
            if (variable == null)
                Error("Assignment variable " + assignment.Name + " unknown.");

            // Non-indexed assignment
            if (assignment.Index == null)
            {
                if (variable.CodeObject is ParameterDefinition)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_S,
                            ((ParameterDefinition) variable.CodeObject).Sequence - 1);
                }

                // Load value
                EmitExpression(il, assignment.Value, symbolTable);

                // Store
                if (variable.CodeObject is VariableDefinition)
                    il.Emit(Mono.Cecil.Cil.OpCodes.Stloc, (VariableDefinition) variable.CodeObject);
                else if (variable.CodeObject is FieldDefinition)
                    il.Emit(Mono.Cecil.Cil.OpCodes.Stsfld, (FieldDefinition) variable.CodeObject);
                else if (variable.CodeObject is ParameterDefinition)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        il.Emit(Mono.Cecil.Cil.OpCodes.Stind_I4);
                    else
                        il.Emit(Mono.Cecil.Cil.OpCodes.Starg, ((ParameterDefinition) variable.CodeObject).Sequence - 1);
                }
            }
            else
            {
                // Load array.
                if (variable.CodeObject is VariableDefinition)
                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, (VariableDefinition) variable.CodeObject);
                else if (variable.CodeObject is FieldDefinition)
                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldsfld, (FieldDefinition) variable.CodeObject);
                // Load index.
                EmitExpression(il, assignment.Index, symbolTable);
                // Load value.
                EmitExpression(il, assignment.Value, symbolTable);
                // Set
                il.Emit(Mono.Cecil.Cil.OpCodes.Stelem_I4);
            }
        }

        private void EmitCall(ILProcessor il, Call call, SymbolTable symbolTable)
        {
            Symbol symbol = symbolTable.Find(call.Name, SymbolType.Function);

            if (symbol != null)
            {
                Function function = symbol.SyntaxObject as Function;

                //
                // Check arguments
                //

                if (call.Arguments == null && function.Parameters == null)
                {
                    // Ugly hack.
                    goto Hack;
                }
                else if (call.Arguments.Count != function.Parameters.Count)
                {
                    Error("Argument mismatch [" + call.Name + "]");
                }
                else if (call.Arguments.Count != function.Parameters.Count)
                {
                    Error("Argument mismatch [" + call.Name + "]");
                }
                else
                {
                    for (int x = 0; x < call.Arguments.Count; x++)
                    {
                        if (call.Arguments[x].PassMethod != function.Parameters[x].PassMethod)
                        {
                            Error("Argument error [" + call.Name + "], argument [" + x + "] is wrong.");
                        }
                    }
                }

                if (call.Arguments != null)
                {
                    foreach (Argument argument in call.Arguments)
                    {
                        if (argument.PassMethod == PassMethod.ByReference)
                        {
                            // Regular value
                            if (argument.Value is Name)
                            {
                                Symbol variable = symbolTable.Find(((Name) argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is VariableDefinition)
                                {
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldloca, variable.CodeObject as VariableDefinition);
                                }
                                else if (variable.CodeObject is FieldDefinition)
                                {
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldsflda, variable.CodeObject as FieldDefinition);
                                }
                                else if (variable.CodeObject is ParameterBuilder)
                                {
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldarga_S,
                                        ((ParameterBuilder) variable.CodeObject).Position - 1);
                                }
                            }
                            else if (argument.Value is UnaryExpression &&
                                     ((UnaryExpression) argument.Value).UnaryOperatorType == UnaryOperatorType.Indexer)
                            {
                                Symbol variable = symbolTable.Find(((Name) argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is LocalBuilder)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.VariableType ==
                                        VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldloca, variable.CodeObject as VariableDefinition);
                                }
                                else if (variable.CodeObject is FieldBuilder)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.VariableType ==
                                        VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldsflda, variable.CodeObject as FieldDefinition);
                                }
                                else if (variable.CodeObject is ParameterDefinition)
                                {
                                    if (((Parameter) variable.SyntaxObject).Type.VariableType ==
                                        VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldarga,
                                        ((ParameterDefinition) variable.CodeObject).Sequence - 1);
                                }

                                EmitExpression(il, ((UnaryExpression) argument.Value).Indexer, symbolTable);
                                il.Emit(Mono.Cecil.Cil.OpCodes.Ldelema);
                            }
                            else
                            {
                                Error("ref may only be applied to variables");
                            }
                        }
                        else
                        {
                            EmitExpression(il, argument.Value, symbolTable);
                        }
                    }
                }

                Hack:
                il.Emit(Mono.Cecil.Cil.OpCodes.Call, ((MethodDefinition) symbol.CodeObject));
            }
            else
            {
                if (call.Name == "Read")
                {
//                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldstr, "Input > ");
//                    MethodInfo write = System.Type.GetType("System.Console")
//                        .GetMethod("Write", new System.Type[] {typeof(string)});
//                    il.Emit(Mono.Cecil.Cil.OpCodes.Call, write, null);
//
//                    MethodInfo read = System.Type.GetType("System.Console").GetMethod("ReadLine");
//                    MethodInfo parse = System.Type.GetType("System.Int32")
//                        .GetMethod("Parse", new System.Type[] {typeof(string)});
//                    il.Emit(Mono.Cecil.Cil.OpCodes.Call, read, null);
//                    il.Emit(Mono.Cecil.Cil.OpCodes.Call, parse, null);
                }
                else if (call.Name == "Write")
                {
//                    EmitExpression(il, call.Arguments[0].Value, symbolTable);
//                    MethodInfo write = System.Type.GetType("System.Console")
//                        .GetMethod("WriteLine", new System.Type[] {typeof(int)});
//                    il.EmitCall(OpCodes.Call, write, null);
                }
                else
                {
                    Error("Unknown function name. [" + call.Name + "]");
                }
            }
        }


        private void EmitCallStatement(ILProcessor il, CallStatement call, SymbolTable symbolTable)
        {
            Symbol symbol = symbolTable.Find(call.Name, SymbolType.Function);

            if (symbol != null)
            {
                Function function = symbol.SyntaxObject as Function;

                //
                // Check arguments
                //
                if (call.Arguments == null && function.Parameters == null)
                {
                    // Ugly hack.
                    goto Hack;
                }
                else if (call.Arguments.Count != function.Parameters.Count)
                {
                    Error("Argument mismatch [" + call.Name + "]");
                }
                else if (call.Arguments.Count != function.Parameters.Count)
                {
                    Error("Argument mismatch [" + call.Name + "]");
                }
                else
                {
                    for (int x = 0; x < call.Arguments.Count; x++)
                    {
                        if (call.Arguments[x].PassMethod != function.Parameters[x].PassMethod)
                        {
                            Error("Argument error [" + call.Name + "], argument [" + x + "] is wrong.");
                        }
                    }
                }

                if (call.Arguments != null)
                {
                    foreach (Argument argument in call.Arguments)
                    {
                        if (argument.PassMethod == PassMethod.ByReference)
                        {
                            // Regular value
                            if (argument.Value is Name)
                            {
                                Symbol variable = symbolTable.Find(((Name) argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is LocalBuilder)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.VariableType ==
                                        VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldloca, variable.CodeObject as VariableDefinition);
                                }
                                else if (variable.CodeObject is FieldBuilder)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.VariableType ==
                                        VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldsflda, variable.CodeObject as FieldDefinition);
                                }
                                else if (variable.CodeObject is ParameterBuilder)
                                {
                                    if (((Parameter) variable.SyntaxObject).Type.VariableType ==
                                        VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldarga,
                                        ((ParameterBuilder) variable.CodeObject).Position - 1);
                                }
                            }
                            else if (argument.Value is UnaryExpression &&
                                     ((UnaryExpression) argument.Value).UnaryOperatorType == UnaryOperatorType.Indexer)
                            {
                                Symbol variable = symbolTable.Find(((Name) argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is VariableDefinition)
                                {
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, variable.CodeObject as VariableDefinition);
                                }
                                else if (variable.CodeObject is FieldBuilder)
                                {
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldsfld, variable.CodeObject as FieldDefinition);
                                }
                                else if (variable.CodeObject is ParameterBuilder)
                                {
                                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldarga,
                                        ((ParameterBuilder) variable.CodeObject).Position - 1);
                                }

                                EmitExpression(il, ((UnaryExpression) argument.Value).Indexer, symbolTable);
                                il.Emit(Mono.Cecil.Cil.OpCodes.Ldelema);
                            }
                            else
                            {
                                Error("ref may only be applied to variables");
                            }
                        }
                        else
                        {
                            EmitExpression(il, argument.Value, symbolTable);
                        }
                    }
                }

                Hack:
                il.Emit(Mono.Cecil.Cil.OpCodes.Call, ((MethodDefinition) symbol.CodeObject));
            }
            else
            {
//                if (call.Name == "Read")
//                {
//                    il.Emit(OpCodes.Ldstr, "Input > ");
//                    MethodInfo write = System.Type.GetType("System.Console")
//                        .GetMethod("Write", new System.Type[] {typeof(string)});
//                    il.EmitCall(OpCodes.Call, write, null);
//
//                    MethodInfo read = System.Type.GetType("System.Console").GetMethod("ReadLine");
//                    MethodInfo parse = System.Type.GetType("System.Int32")
//                        .GetMethod("Parse", new System.Type[] {typeof(string)});
//                    il.EmitCall(OpCodes.Call, read, null);
//                    il.EmitCall(OpCodes.Call, parse, null);
//                }
//                else if (call.Name == "Write")
//                {
//                    EmitExpression(il, call.Arguments[0].Value, symbolTable);
//                    MethodInfo write = System.Type.GetType("System.Console")
//                        .GetMethod("WriteLine", new System.Type[] {typeof(int)});
//                    il.EmitCall(OpCodes.Call, write, null);
//                }
//                else
//                {
//                    Error("Unknown function name. [" + call.Name + "]");
//                }
            }
        }
    }
}