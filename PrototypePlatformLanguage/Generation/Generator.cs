using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Serialization;
using PrototypePlatformLanguage.AST.Definitions;
using PrototypePlatformLanguage.AST.Definitions.Functions;
using PrototypePlatformLanguage.AST.Definitions.Statements;
using PrototypePlatformLanguage.AST.Definitions.Symbols;
using PrototypePlatformLanguage.AST.Infrastructure;
using Mono.Cecil;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using Module = PrototypePlatformLanguage.AST.Definitions.Module;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;


namespace PrototypePlatformLanguage.Generation
{
    public class Generator
    {
        private Module _module = null;
        private ModuleDefinition _dllModule = null;
        private SymbolTable _global = null;


        public Generator(Module module)
        {
            _module = module;
        }

        public void Compile(string path)
        {
            AssemblyDefinition ad = AssemblyDefinition.CreateAssembly(
                new AssemblyNameDefinition("BetaName", Version.Parse("1.0.0.0")), "ZModule", ModuleKind.Dll);

            _dllModule = ad.MainModule;

            TypeDefinition td = new TypeDefinition("TestNamespace", "SomeName",
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed);


            //
            // Create global variables.
            //

            _global = new SymbolTable();

            //
            // Create functions.
            //


            _module.TypeBody.SymbolTable = _global;
            BuildFunctions(_module.TypeBody, moduleBuilder);

            foreach (Function function in _module.TypeBody.Functions)
            {
                BuildFunction(function);
            }

            //
            // Create entry point.
            //

            MethodBuilder mainBuilder = moduleBuilder.DefineGlobalMethod("Main",
                MethodAttributes.Public | MethodAttributes.Static, typeof(void), null);
            ILGenerator mainIL = mainBuilder.GetILGenerator();

            EmitBody(mainIL, _module.Body, true);

            moduleBuilder.CreateGlobalFunctions();
            assemblyBuilder.SetEntryPoint(mainBuilder.GetBaseDefinition());
            assemblyBuilder.Save(_module.Name + ".exe");

            System.IO.File.Move(_module.Name + ".exe", path);
        }

        private void FindFunction(Body body, ArrayList functions)
        {
            if (body != null && body.Functions != null)
            {
                foreach (Function function in body.Functions)
                    FindFunction(function.Body, functions);
            }
        }

        private void Error(string message)
        {
            throw new Exception(message);
        }

        private void EmitExpression(ILGenerator il, Expression expression, SymbolTable symbolTable)
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
            else if (expression is UnaryExpression)
            {
                UnaryExpression unaryExpression = expression as UnaryExpression;

                switch (unaryExpression.UnaryOperatorType)
                {
                    case UnaryOperatorType.Indexer:
                        EmitExpression(il, unaryExpression.Value, symbolTable);
                        EmitExpression(il, unaryExpression.Indexer, symbolTable);
                        il.Emit(OpCodes.Ldelem_I4);
                        break;
                    case UnaryOperatorType.Negative:
                        EmitExpression(il, unaryExpression.Value, symbolTable);
                        il.Emit(OpCodes.Neg);
                        break;
                    case UnaryOperatorType.Not:
                        EmitExpression(il, unaryExpression.Value, symbolTable);
                        il.Emit(OpCodes.Not);
                        break;
                }
            }
            else if (expression is Literal)
            {
                Literal literal = expression as Literal;

                switch (literal.LiteralType)
                {
                    case LiteralType.Integer:
                        il.Emit(OpCodes.Ldc_I4, Int32.Parse(literal.Value));
                        break;
                    case LiteralType.Real:
                        il.Emit(OpCodes.Ldc_R4, float.Parse(literal.Value));
                        break;
                    case LiteralType.Character:
                        il.Emit(OpCodes.Ldc_I4, char.GetNumericValue(literal.Value, 0));
                        break;
                    case LiteralType.Boolean:
                        if (literal.Value == "true")
                            il.Emit(OpCodes.Ldc_I4, 1);
                        else if (literal.Value == "false")
                            il.Emit(OpCodes.Ldc_I4, 0);
                        break;
                }
            }
            else if (expression is Name)
            {
                Name name = expression as Name;

                Symbol variable = symbolTable.Find(name.Value, SymbolType.Variable);
                if (variable == null)
                    Error("Assignment variable " + name.Value + " unknown.");

                if (variable.CodeObject is LocalBuilder)
                    il.Emit(OpCodes.Ldloc, (LocalBuilder) variable.CodeObject);
                else if (variable.CodeObject is FieldBuilder)
                    il.Emit(OpCodes.Ldsfld, (FieldBuilder) variable.CodeObject);
                else if (variable.CodeObject is ParameterBuilder)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    il.Emit(OpCodes.Ldarg_S, ((ParameterBuilder) variable.CodeObject).Position - 1);
                    if (p.PassMethod == PassMethod.ByReference)
                        il.Emit(OpCodes.Ldind_I4);
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
        /// <param name="body"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private List<MethodDefinition> BuildFunctions(TypeBody body, TypeDefinition typeDefinition)
        {
            if (body == null || typeDefinition == null)
                throw new ArgumentNullException();

            SymbolTable sibillings = new SymbolTable(_global);

            List<MethodDefinition> result = new List<MethodDefinition>();

            if (body != null && body.Functions != null)
            {
                foreach (Function function in body.Functions)
                {
                    // Make child visible to sibillings
                    function.InstructionsBody.SymbolTable = sibillings;

                    MethodBuilder method = BuildFunction(function);

                    // Make child visible to parent.
                    body.SymbolTable.Add(function.Name, SymbolType.Function, function, method);
                    sibillings.Add(function.Name, SymbolType.Function, function, method);
                }
            }
        }

        private MethodDefinition BuildFunction(Function function)
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

            // Create method.
            MethodDefinition method = new MethodDefinition(functionName,
                MethodAttributes.Public | MethodAttributes.Static, _dllModule.ImportReference(returnType));

            if (function.Parameters != null)
            {
                for (int x = 0; x < function.Parameters.Count; x++)
                {
                    var pType = function.Parameters[x].Type.ToSystemType();
                    var pName = function.Parameters[x].Name;


                    ParameterDefinition p = new ParameterDefinition(pName, ParameterAttributes.None,
                        _dllModule.ImportReference(pType));

                    method.Parameters.Add(p);
                }
            }

            return method;
        }

        private void EmitFunction(Function function)
        {
            if (function == null)
                throw new ArgumentNullException();

            //
            // Build function body.
            //

            ILGenerator il = function.Builder.GetILGenerator();

            EmitBody(il, function.InstructionsBody, false);

            return;
        }

        private void EmitBody(ILGenerator il, InstructionsBody body, bool root)
        {
            //
            // Declare local variables.
            //

            foreach (Statement statement in body.Statements)
            {
                if (statement is Variable)
                {
                    Variable variable = statement as Variable;

                    LocalBuilder local = null;
                    FieldBuilder global = null;

                    if (root)
                    {
                        global = (FieldBuilder) body.SymbolTable.Find(variable.Name, SymbolType.Variable).CodeObject;
                    }
                    else
                    {
                        local = il.DeclareLocal(variable.Type.ToSystemType());
                        body.SymbolTable.Add(variable.Name, SymbolType.Variable, variable, local);
                    }

                    //
                    // Initialize  variable.
                    //

                    if (variable.Type.VariableType == VariableType.Primitive)
                    {
                        if (variable.Value != null && variable.Value is Expression)
                        {
                            EmitExpression(il, (Expression) variable.Value, body.SymbolTable);

                            if (root)
                                il.Emit(OpCodes.Stsfld, global);
                            else
                                il.Emit(OpCodes.Stloc, local);
                        }
                    }
                    else if (variable.Type.VariableType == VariableType.PrimitiveArray)
                    {
                        // Empty array initialization.
                        if (variable.Value != null && variable.Value is Expression)
                        {
                            EmitExpression(il, (Expression) variable.Value, body.SymbolTable);
                            il.Emit(OpCodes.Newarr, variable.Type.ToSystemType());
                            if (root)
                                il.Emit(OpCodes.Stsfld, global);
                            else
                                il.Emit(OpCodes.Stloc, local);
                        }
                        else if (variable.Value != null && variable.Value is ElementCollection)
                        {
                            ElementCollection elements = variable.Value as ElementCollection;

                            il.Emit(OpCodes.Ldc_I4, elements.Count);
                            il.Emit(OpCodes.Newarr, variable.Type.ToSystemType());
                            if (root)
                                il.Emit(OpCodes.Stsfld, global);
                            else
                                il.Emit(OpCodes.Stloc, local);

                            for (int x = 0; x < elements.Count; x++)
                            {
                                // Load array
                                if (root)
                                    il.Emit(OpCodes.Ldsfld, global);
                                else
                                    il.Emit(OpCodes.Ldloc, local);
                                // Load index
                                il.Emit(OpCodes.Ldc_I4, x);
                                // Load value
                                EmitExpression(il, elements[x].Expression, body.SymbolTable);
                                // Store
                                il.Emit(OpCodes.Stelem_I4);
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
                    il.Emit(OpCodes.Ret);
                }
                else if (statement is CallStatement)
                {
                    CallStatement call = statement as CallStatement;
                    Symbol symbol = body.SymbolTable.Find(call.Name, SymbolType.Function);
                    EmitCallStatement(il, statement as CallStatement, body.SymbolTable);

                    if (symbol != null)
                    {
                        if (((MethodBuilder) symbol.CodeObject).ReturnType != typeof(void))
                            il.Emit(OpCodes.Pop);
                    }
                    else
                    {
                        if (call.Name == "Read")
                            il.Emit(OpCodes.Pop);
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

                    if (ifStatement.IfBody != null && ifStatement.ElseBody == null)
                    {
                        ifStatement.IfBody.SymbolTable = new SymbolTable(body.SymbolTable);
                        Label exit = il.DefineLabel();
                        il.Emit(OpCodes.Brfalse, exit);
                        EmitBody(il, ifStatement.IfBody, false);
                        il.MarkLabel(exit);
                    }
                    else if (ifStatement.IfBody != null && ifStatement.ElseBody != null)
                    {
                        ifStatement.IfBody.SymbolTable = new SymbolTable(body.SymbolTable);
                        ifStatement.ElseBody.SymbolTable = new SymbolTable(body.SymbolTable);
                        Label exit = il.DefineLabel();
                        Label elseLabel = il.DefineLabel();
                        il.Emit(OpCodes.Brfalse, elseLabel);
                        EmitBody(il, ifStatement.IfBody, false);
                        il.Emit(OpCodes.Br, exit);
                        il.MarkLabel(elseLabel);
                        EmitBody(il, ifStatement.ElseBody, false);
                        il.MarkLabel(exit);
                    }
                }

                else if (statement is While)
                {
                    //
                    // Generate while statement.
                    //

                    While whileStatement = statement as While;
                    whileStatement.Body.SymbolTable = new SymbolTable(body.SymbolTable);
                    Label begin = il.DefineLabel();
                    Label exit = il.DefineLabel();
                    il.MarkLabel(begin);
                    // Eval condition
                    EmitExpression(il, whileStatement.Condition, body.SymbolTable);
                    il.Emit(OpCodes.Brfalse, exit);
                    EmitBody(il, whileStatement.Body, false);
                    il.Emit(OpCodes.Br, begin);
                    il.MarkLabel(exit);
                }
                else if (statement is Do)
                {
                    //
                    // Generate do statement.
                    //

                    Do doStatement = statement as Do;
                    doStatement.Body.SymbolTable = new SymbolTable(body.SymbolTable);

                    Label loop = il.DefineLabel();
                    il.MarkLabel(loop);
                    EmitBody(il, doStatement.Body, false);
                    EmitExpression(il, doStatement.Condition, body.SymbolTable);
                    il.Emit(OpCodes.Brtrue, loop);
                }
                else if (statement is For)
                {
                    //
                    // Generate for statement.
                    //

                    For forStatement = statement as For;
                    forStatement.Body.SymbolTable = new SymbolTable(body.SymbolTable);

                    Label loop = il.DefineLabel();
                    Label exit = il.DefineLabel();

                    // Emit initializer
                    EmitAssignment(il, forStatement.Initializer, body.SymbolTable);
                    il.MarkLabel(loop);
                    // Emit condition
                    EmitExpression(il, forStatement.Condition, body.SymbolTable);
                    il.Emit(OpCodes.Brfalse, exit);
                    // Emit body
                    EmitBody(il, forStatement.Body, false);
                    // Emit counter
                    EmitAssignment(il, forStatement.Counter, body.SymbolTable);
                    il.Emit(OpCodes.Br, loop);
                    il.MarkLabel(exit);
                }
            }
        }

        private void EmitAssignment(ILGenerator il, Assignment assignment, SymbolTable symbolTable)
        {
            Symbol variable = symbolTable.Find(assignment.Name, SymbolType.Variable);
            if (variable == null)
                Error("Assignment variable " + assignment.Name + " unknown.");

            // Non-indexed assignment
            if (assignment.Index == null)
            {
                if (variable.CodeObject is ParameterBuilder)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        il.Emit(OpCodes.Ldarg_S, ((ParameterBuilder) variable.CodeObject).Position - 1);
                }

                // Load value
                EmitExpression(il, assignment.Value, symbolTable);

                // Store
                if (variable.CodeObject is LocalBuilder)
                    il.Emit(OpCodes.Stloc, (LocalBuilder) variable.CodeObject);
                else if (variable.CodeObject is FieldBuilder)
                    il.Emit(OpCodes.Stsfld, (FieldBuilder) variable.CodeObject);
                else if (variable.CodeObject is ParameterBuilder)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        il.Emit(OpCodes.Stind_I4);
                    else
                        il.Emit(OpCodes.Starg, ((ParameterBuilder) variable.CodeObject).Position - 1);
                }
            }
            else
            {
                // Load array.
                if (variable.CodeObject is LocalBuilder)
                    il.Emit(OpCodes.Ldloc, (LocalBuilder) variable.CodeObject);
                else if (variable.CodeObject is FieldBuilder)
                    il.Emit(OpCodes.Ldsfld, (FieldBuilder) variable.CodeObject);
                // Load index.
                EmitExpression(il, assignment.Index, symbolTable);
                // Load value.
                EmitExpression(il, assignment.Value, symbolTable);
                // Set
                il.Emit(OpCodes.Stelem_I4);
            }
        }

        private void EmitCall(ILGenerator il, Call call, SymbolTable symbolTable)
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
                                    il.Emit(OpCodes.Ldloca, variable.CodeObject as LocalBuilder);
                                }
                                else if (variable.CodeObject is FieldBuilder)
                                {
                                    il.Emit(OpCodes.Ldsflda, variable.CodeObject as FieldBuilder);
                                }
                                else if (variable.CodeObject is ParameterBuilder)
                                {
                                    il.Emit(OpCodes.Ldarga_S, ((ParameterBuilder) variable.CodeObject).Position - 1);
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
                                    il.Emit(OpCodes.Ldloca, variable.CodeObject as LocalBuilder);
                                }
                                else if (variable.CodeObject is FieldBuilder)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.VariableType ==
                                        VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(OpCodes.Ldsflda, variable.CodeObject as FieldBuilder);
                                }
                                else if (variable.CodeObject is ParameterBuilder)
                                {
                                    if (((Parameter) variable.SyntaxObject).Type.VariableType ==
                                        VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(OpCodes.Ldarga, ((ParameterBuilder) variable.CodeObject).Position - 1);
                                }

                                EmitExpression(il, ((UnaryExpression) argument.Value).Indexer, symbolTable);
                                il.Emit(OpCodes.Ldelema);
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
                il.Emit(OpCodes.Call, ((MethodBuilder) symbol.CodeObject));
            }
            else
            {
                if (call.Name == "Read")
                {
                    il.Emit(OpCodes.Ldstr, "Input > ");
                    MethodInfo write = System.Type.GetType("System.Console")
                        .GetMethod("Write", new System.Type[] {typeof(string)});
                    il.EmitCall(OpCodes.Call, write, null);

                    MethodInfo read = System.Type.GetType("System.Console").GetMethod("ReadLine");
                    MethodInfo parse = System.Type.GetType("System.Int32")
                        .GetMethod("Parse", new System.Type[] {typeof(string)});
                    il.EmitCall(OpCodes.Call, read, null);
                    il.EmitCall(OpCodes.Call, parse, null);
                }
                else if (call.Name == "Write")
                {
                    EmitExpression(il, call.Arguments[0].Value, symbolTable);
                    MethodInfo write = System.Type.GetType("System.Console")
                        .GetMethod("WriteLine", new System.Type[] {typeof(int)});
                    il.EmitCall(OpCodes.Call, write, null);
                }
                else
                {
                    Error("Unknown function name. [" + call.Name + "]");
                }
            }
        }


        private void EmitCallStatement(ILGenerator il, CallStatement call, SymbolTable symbolTable)
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
                                    il.Emit(OpCodes.Ldloca, variable.CodeObject as LocalBuilder);
                                }
                                else if (variable.CodeObject is FieldBuilder)
                                {
                                    if (((Variable) variable.SyntaxObject).Type.VariableType ==
                                        VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(OpCodes.Ldsflda, variable.CodeObject as FieldBuilder);
                                }
                                else if (variable.CodeObject is ParameterBuilder)
                                {
                                    if (((Parameter) variable.SyntaxObject).Type.VariableType ==
                                        VariableType.PrimitiveArray)
                                        Error("ref cannot be applied to arrays");
                                    il.Emit(OpCodes.Ldarga, ((ParameterBuilder) variable.CodeObject).Position - 1);
                                }
                            }
                            else if (argument.Value is UnaryExpression &&
                                     ((UnaryExpression) argument.Value).UnaryOperatorType == UnaryOperatorType.Indexer)
                            {
                                Symbol variable = symbolTable.Find(((Name) argument.Value).Value, SymbolType.Variable);
                                if (variable.CodeObject is LocalBuilder)
                                {
                                    il.Emit(OpCodes.Ldloc, variable.CodeObject as LocalBuilder);
                                }
                                else if (variable.CodeObject is FieldBuilder)
                                {
                                    il.Emit(OpCodes.Ldsfld, variable.CodeObject as FieldBuilder);
                                }
                                else if (variable.CodeObject is ParameterBuilder)
                                {
                                    il.Emit(OpCodes.Ldarga, ((ParameterBuilder) variable.CodeObject).Position - 1);
                                }

                                EmitExpression(il, ((UnaryExpression) argument.Value).Indexer, symbolTable);
                                il.Emit(OpCodes.Ldelema);
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
                il.Emit(OpCodes.Call, ((MethodBuilder) symbol.CodeObject));
            }
            else
            {
                if (call.Name == "Read")
                {
                    il.Emit(OpCodes.Ldstr, "Input > ");
                    MethodInfo write = System.Type.GetType("System.Console")
                        .GetMethod("Write", new System.Type[] {typeof(string)});
                    il.EmitCall(OpCodes.Call, write, null);

                    MethodInfo read = System.Type.GetType("System.Console").GetMethod("ReadLine");
                    MethodInfo parse = System.Type.GetType("System.Int32")
                        .GetMethod("Parse", new System.Type[] {typeof(string)});
                    il.EmitCall(OpCodes.Call, read, null);
                    il.EmitCall(OpCodes.Call, parse, null);
                }
                else if (call.Name == "Write")
                {
                    EmitExpression(il, call.Arguments[0].Value, symbolTable);
                    MethodInfo write = System.Type.GetType("System.Console")
                        .GetMethod("WriteLine", new System.Type[] {typeof(int)});
                    il.EmitCall(OpCodes.Call, write, null);
                }
                else
                {
                    Error("Unknown function name. [" + call.Name + "]");
                }
            }
        }
    }
}