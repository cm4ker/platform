using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Aquila.Compiler;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Expressions;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Definitions.Statements;
using Aquila.Language.Ast.Extension;
using Aquila.Language.Ast.Lowering;
using Aquila.Language.Ast.Symbols;
using Aquila.Language.Ast.Text;

namespace Aquila.Language.Ast.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private readonly Compilation _compialtion;
        private readonly bool _isScript;
        private readonly MethodSymbol? _function;

        private Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)> _loopStack =
            new Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)>();

        private int _labelCounter;
        private BoundScope _scope;

        private Binder(Compilation compialtion, bool isScript, BoundScope? parent, MethodSymbol? function)
        {
            _scope = new BoundScope(parent);
            _compialtion = compialtion;
            _isScript = isScript;
            _function = function;

            if (function != null)
            {
                // foreach (var p in function.Parameters)
                //     _scope.TryDeclareVariable(p);
            }
        }

        public static BoundGlobalScope BindGlobalScope(Compilation compilation, bool isScript,
            BoundGlobalScope? previous,
            ImmutableArray<SyntaxTree> syntaxTrees)
        {
            var parentScope = CreateParentScope(previous);
            var binder = new Binder(compilation, isScript, parentScope, function: null);

            binder.Diagnostics.AddRange(syntaxTrees.SelectMany(st => st.Diagnostics));
            if (binder.Diagnostics.Any())
                return new BoundGlobalScope(previous, binder.Diagnostics.ToImmutableArray(), null, null,
                    ImmutableArray<MethodSymbol>.Empty, ImmutableArray<LocalSymbol>.Empty);

            var functionDeclarations = syntaxTrees.SelectMany(st => st.Root.Methods);

            foreach (var function in functionDeclarations)
                binder.BindMethodDeclaration(function);
            // Check for main/script with global statements

            var functions = binder._scope.GetDeclaredFunctions();

            MethodSymbol? mainFunction;
            MethodSymbol? scriptFunction = null;

            if (isScript)
            {
                mainFunction = null;
                scriptFunction = null;
            }
            else
            {
                mainFunction = functions.FirstOrDefault(f => f.Name == "main");
                scriptFunction = null;

                if (mainFunction != null)
                {
                    if (mainFunction.NamedType.SpecialType != SpecialType.System_Void || mainFunction.Parameters.Any())
                        binder.Diagnostics.ReportMainMustHaveCorrectSignature(mainFunction.Declaration!
                            .Location);
                }
            }

            var diagnostics = binder.Diagnostics.ToImmutableArray();
            var variables = binder._scope.GetDeclaredVariables();

            if (previous != null)
                diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);

            return new BoundGlobalScope(previous, diagnostics, mainFunction, scriptFunction, functions, variables);
        }

        public static BoundProgram BindProgram(Compilation compilation, bool isScript, BoundProgram? previous,
            BoundGlobalScope globalScope)
        {
            var parentScope = CreateParentScope(globalScope);

            if (globalScope.Diagnostics.Any())
                return new BoundProgram(previous, globalScope.Diagnostics, null, null,
                    ImmutableDictionary<MethodSymbol, BoundBlockStatement>.Empty);

            var functionBodies = ImmutableDictionary.CreateBuilder<MethodSymbol, BoundBlockStatement>();
            var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

            foreach (var function in globalScope.Functions)
            {
                var binder = new Binder(compilation, isScript, parentScope, function);
                var body = binder.BindStatement(function.Declaration!.Block);
                var loweredBody = Lowerer.Lower(function, body);

                if (function.NamedType.SpecialType != SpecialType.System_Void &&
                    !ControlFlowGraph.AllPathsReturn(loweredBody))
                    binder._diagnostics.ReportAllPathsMustReturn(function.Declaration.Identifier.Location);

                functionBodies.Add(function, loweredBody);

                diagnostics.AddRange(binder.Diagnostics);
            }

            if (globalScope.MainFunction != null && globalScope.Statements.Any())
            {
            }
            else if (globalScope.ScriptFunction != null)
            {
            }

            return new BoundProgram(previous,
                diagnostics.ToImmutable(),
                globalScope.MainFunction,
                globalScope.ScriptFunction,
                functionBodies.ToImmutable());
        }

        private void BindMethodDeclaration(MethodDeclarationSyntax syntax)
        {
            var parameters = ImmutableArray.CreateBuilder<ParameterSymbol>();

            var seenParameterNames = new HashSet<string>();

            foreach (var parameterSyntax in syntax.Parameters)
            {
                var parameterName = ""; //parameterSyntax.Identifier.Value;
                var parameterType = BindTypeClause(parameterSyntax.Type);
                if (!seenParameterNames.Add(parameterName))
                {
                    _diagnostics.ReportParameterAlreadyDeclared(parameterSyntax.Location, parameterName);
                }
                else
                {
                    var parameter = new SourceSimpleParameterSymbol(parameterName, parameterType, parameters.Count);
                    parameters.Add(parameter);
                }
            }

            var type = BindTypeClause(syntax.Type) ?? GetSpecialType(SpecialType.System_Void, syntax);

            // var function =
            //     new SourceOrdinaryMethodSymbol(syntax.Identifier.Value, parameters.ToImmutable(), type, syntax);
            // if (syntax.Identifier.Value != null &&
            //     !_scope.TryDeclareFunction(function))
            // {
            //     _diagnostics.ReportSymbolAlreadyDeclared(syntax.Identifier.Location, function.Name);
            // }
        }

        private static BoundScope CreateParentScope(BoundGlobalScope? previous)
        {
            var stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            var parent = CreateRootScope();

            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);

                foreach (var f in previous.Functions)
                    scope.TryDeclareFunction(f);

                foreach (var v in previous.Variables)
                    scope.TryDeclareVariable(v);

                parent = scope;
            }

            return parent;
        }

        private static BoundScope CreateRootScope()
        {
            var result = new BoundScope(null);

            foreach (var f in BuiltinFunctions.GetAll())
                result.TryDeclareFunction(f);

            return result;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private BoundStatement BindErrorStatement(SyntaxNode syntax)
        {
            return new BoundExpressionStatement(syntax, new BoundErrorExpression(syntax));
        }

        private BoundStatement BindGlobalStatement(StatementSyntax syntax)
        {
            return BindStatement(syntax, isGlobal: true);
        }

        private BoundStatement BindStatement(StatementSyntax syntax, bool isGlobal = false)
        {
            var result = BindStatementInternal(syntax);

            if (!_isScript || !isGlobal)
            {
                if (result is BoundExpressionStatement es)
                {
                    var isAllowedExpression = es.Expression.Kind == BoundNodeKind.ErrorExpression ||
                                              es.Expression.Kind == BoundNodeKind.AssignmentExpression ||
                                              es.Expression.Kind == BoundNodeKind.CallExpression ||
                                              es.Expression.Kind == BoundNodeKind.CompoundAssignmentExpression;
                    if (!isAllowedExpression)
                        _diagnostics.ReportInvalidExpressionStatement(syntax.Location);
                }
            }

            return result;
        }

        private BoundStatement BindStatementInternal(StatementSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.BlockStatement:
                    return BindBlockStatement((BlockStatementSyntax) syntax);
                case SyntaxKind.VariableDeclaration:
                    return BindVariableDeclaration((VariableDeclarationSyntax) syntax);
                case SyntaxKind.IfStatement:
                    return BindIfStatement((If) syntax);
                case SyntaxKind.WhileStatement:
                    return BindWhileStatement((While) syntax);
                case SyntaxKind.DoWhileStatement:
                    return BindDoWhileStatement((DoWhile) syntax);
                case SyntaxKind.ForStatement:
                    return BindForStatement((For) syntax);
                case SyntaxKind.BreakStatement:
                    return BindBreakStatement((BreakSyntax) syntax);
                case SyntaxKind.ContinueStatement:
                    return BindContinueStatement((ContinueSyntax) syntax);
                case SyntaxKind.ReturnStatement:
                    return BindReturnStatement((ReturnSyntax) syntax);
                case SyntaxKind.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatement) syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();
            _scope = new BoundScope(_scope);

            foreach (var statementSyntax in syntax.Statements)
            {
                var statement = BindStatement(statementSyntax);
                statements.Add(statement);
            }

            _scope = _scope.Parent!;

            return new BoundBlockStatement(syntax, statements.ToImmutable());
        }

        private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
        {
            throw new NotImplementedException();
            // var type = BindTypeClause(syntax.VariableType);
            // //TODO: add initializer support
            // var initializer = BindExpression(syntax.Initializer);
            // var variableType = type ?? initializer.NamedType;
            // var variable =
            //     BindVariableDeclaration(syntax.Identifier, false, variableType, initializer.ConstantValue);
            // var convertedInitializer = BindConversion(syntax.Initializer.Location, initializer, variableType);
            //
            // return new BoundVariableDeclaration(syntax, variable, convertedInitializer);
        }

        [return: NotNullIfNotNull("syntax")]
        private NamedTypeSymbol? BindTypeClause(TypeSyntax? syntax)
        {
            if (syntax == null)
                return null;

            var type = LookupType(syntax.Value);
            if (type == null)
                _diagnostics.ReportUndefinedType(syntax.Location, syntax.Value);

            return type;
        }

        private BoundStatement BindIfStatement(If syntax)
        {
            var condition = BindExpression(syntax.Condition,
                GetSpecialType(SpecialType.System_Boolean, _diagnostics, syntax.Condition));

            if (condition.ConstantValue != null)
            {
                if ((bool) condition.ConstantValue.Value == false)
                    _diagnostics.ReportUnreachableCode(syntax.ThenBlock);
                else if (syntax.ElseBlock != null)
                    _diagnostics.ReportUnreachableCode(syntax.ElseBlock);
            }

            var thenStatement = BindStatement(syntax.ThenBlock);
            var elseStatement = syntax.ElseBlock == null ? null : BindStatement(syntax.ElseBlock);
            return new BoundIfStatement(syntax, condition, thenStatement, elseStatement);
        }

        private BoundStatement BindWhileStatement(While syntax)
        {
            var condition = BindExpression(syntax.Condition,
                GetSpecialType(SpecialType.System_Boolean, _diagnostics, syntax.Condition));

            if (condition.ConstantValue != null)
            {
                if (!(bool) condition.ConstantValue.Value)
                {
                    _diagnostics.ReportUnreachableCode(syntax.Block);
                }
            }

            var body = BindLoopBody(syntax.Block, out var breakLabel, out var continueLabel);
            return new BoundWhileStatement(syntax, condition, body, breakLabel, continueLabel);
        }

        private BoundStatement BindDoWhileStatement(DoWhile syntax)
        {
            var body = BindLoopBody(syntax.Block, out var breakLabel, out var continueLabel);
            var condition = BindExpression(syntax.Condition,
                GetSpecialType(SpecialType.System_Boolean, syntax.Condition));
            return new BoundDoWhileStatement(syntax, body, condition, breakLabel, continueLabel);
        }

        private BoundStatement BindForStatement(For syntax)
        {
            var lowerBound = BindExpression(syntax.Initializer,
                GetSpecialType(SpecialType.System_Int32, syntax.Initializer));
            var upperBound = BindExpression(syntax.Condition,
                GetSpecialType(SpecialType.System_Int32, syntax.Condition));

            _scope = new BoundScope(_scope);

            // var variable = BindVariableDeclaration(syntax.Identifier, isReadOnly: true,
            //     GetSpecialType(SpecialType.System_Int32, syntax.Identifier));
            var body = BindLoopBody(syntax.Block, out var breakLabel, out var continueLabel);

            _scope = _scope.Parent!;

            return new BoundForStatement(syntax, null, lowerBound, upperBound, body, breakLabel, continueLabel);
        }

        private BoundStatement BindLoopBody(StatementSyntax body, out BoundLabel breakLabel,
            out BoundLabel continueLabel)
        {
            _labelCounter++;
            breakLabel = new BoundLabel($"break{_labelCounter}");
            continueLabel = new BoundLabel($"continue{_labelCounter}");

            _loopStack.Push((breakLabel, continueLabel));
            var boundBody = BindStatement(body);
            _loopStack.Pop();

            return boundBody;
        }

        private BoundStatement BindBreakStatement(BreakSyntax syntax)
        {
            if (_loopStack.Count == 0)
            {
                _diagnostics.ReportInvalidBreakOrContinue(syntax.Location, "break");
                return BindErrorStatement(syntax);
            }

            var breakLabel = _loopStack.Peek().BreakLabel;
            return new BoundGotoStatement(syntax, breakLabel);
        }

        private BoundStatement BindContinueStatement(ContinueSyntax syntax)
        {
            if (_loopStack.Count == 0)
            {
                _diagnostics.ReportInvalidBreakOrContinue(syntax.Location, "continue");
                return BindErrorStatement(syntax);
            }

            var continueLabel = _loopStack.Peek().ContinueLabel;
            return new BoundGotoStatement(syntax, continueLabel);
        }

        private BoundStatement BindReturnStatement(ReturnSyntax syntax)
        {
            var expression = syntax.Expression == null ? null : BindExpression(syntax.Expression);

            if (_function == null)
            {
                if (_isScript)
                {
                    // Ignore because we allow both return with and without values.
                    if (expression == null)
                        expression = new BoundLiteralExpression(syntax, "");
                }
                else if (expression != null)
                {
                    // Main does not support return values.
                    _diagnostics.ReportInvalidReturnWithValueInGlobalStatements(syntax.Expression!.Location);
                }
            }
            else
            {
                if (_function.NamedType.SpecialType == SpecialType.System_Void)
                {
                    if (expression != null)
                        _diagnostics.ReportInvalidReturnExpression(syntax.Expression!.Location, _function.Name);
                }
                else
                {
                    if (expression == null)
                        _diagnostics.ReportMissingReturnExpression(syntax.Location, _function.NamedType);
                    else
                        expression = BindConversion(syntax.Expression!.Location, expression, _function.NamedType);
                }
            }

            return new BoundReturnStatement(syntax, expression);
        }

        private BoundStatement BindExpressionStatement(ExpressionStatement syntax)
        {
            var expression = BindExpression(syntax.Expression, canBeVoid: true);
            return new BoundExpressionStatement(syntax, expression);
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax, NamedTypeSymbol targetNamedType)
        {
            return BindConversion(syntax, targetNamedType);
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax, bool canBeVoid = false)
        {
            var result = BindExpressionInternal(syntax);
            if (!canBeVoid && result.NamedType.SpecialType == SpecialType.System_Void)
            {
                _diagnostics.ReportExpressionMustHaveValue(syntax.Location);
                return new BoundErrorExpression(syntax);
            }

            return result;
        }

        private BoundExpression BindExpressionInternal(ExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                // case SyntaxKind.ParenthesizedExpression:
                //     return BindParenthesizedExpression((ParenthesizedExpressionSyntax) syntax);
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((Literal) syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression(new object());
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentSyntax) syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax) syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax) syntax);
                case SyntaxKind.CallExpression:
                    return BindCallExpression((CallSyntax) syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        // private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        // {
        //     return BindExpression(syntax.Expression);
        // }

        private BoundExpression BindLiteralExpression(Literal syntax)
        {
            var value = syntax.Value;
            return new BoundLiteralExpression(syntax, value);
        }

        private BoundExpression BindNameExpression(object syntax)
        {
            throw new NotImplementedException();

            //var name = syntax.Identifier;
            // if (syntax.IdentifierToken.IsMissing)
            // {
            //     // This means the token was inserted by the parser. We already
            //     // reported error so we can just return an error expression.
            //     return new BoundErrorExpression(syntax);
            // }

            // var variable = BindVariableReference(syntax.Identifier);
            // if (variable == null)
            //     return new BoundErrorExpression(syntax);
            //
            // return new BoundVariableExpression(syntax, variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentSyntax syntax)
        {
            var boundExpression = BindExpression(syntax.Value);
            var reciver = BindExpression(syntax.Assignable);
            //var variable = BindVariableReference(syntax.IdentifierToken);
            // if (variable == null)
            //     return boundExpression;
            //
            // if (variable.IsReadOnly)
            //     _diagnostics.ReportCannotAssign(syntax.AssignmentToken.Location, name);
            //
            // if (syntax.AssignmentToken.Kind != SyntaxKind.EqualsToken)
            // {
            //     var equivalentOperatorTokenKind =
            //         SyntaxFacts.GetBinaryOperatorOfAssignmentOperator(syntax.AssignmentToken.Kind);
            //     var boundOperator = BoundBinaryOperator.Bind(equivalentOperatorTokenKind, variable.NamedType,
            //         boundExpression.NamedType);
            //
            //     if (boundOperator == null)
            //     {
            //         _diagnostics.ReportUndefinedBinaryOperator(syntax.AssignmentToken.Location,
            //             syntax.AssignmentToken.Text, variable.NamedType, boundExpression.NamedType);
            //         return new BoundErrorExpression(syntax);
            //     }
            //
            //     var convertedExpression =
            //         BindConversion(syntax.Expression.Location, boundExpression, variable.NamedType);
            //     return new BoundCompoundAssignmentExpression(syntax, variable, boundOperator, convertedExpression);
            // }
            // else
            // {
            //     var convertedExpression =
            //         BindConversion(syntax.Expression.Location, boundExpression, variable.NamedType);
            //     return new BoundAssignmentExpression(syntax, variable, convertedExpression);
            // }

            return null;
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            throw new NotImplementedException();
            // var boundOperand = BindExpression(syntax.Expression);
            //
            // // if (boundOperand.NamedType == NamedTypeSymbol.Error)
            // //     return new BoundErrorExpression(syntax);
            //
            // var boundOperator = BoundUnaryOperator.Bind(syntax.OperaotrType, boundOperand.NamedType);
            //
            // if (boundOperator == null)
            // {
            //     _diagnostics.ReportUndefinedUnaryOperator(syntax.Location, "TODO: unknown symbol!",
            //         boundOperand.NamedType);
            //     return new BoundErrorExpression(syntax);
            // }
            //
            // return new BoundUnaryExpression(syntax, boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);

            // if (boundLeft.NamedType == NamedTypeSymbol.Error || boundRight.NamedType == NamedTypeSymbol.Error)
            //     return new BoundErrorExpression(syntax);

            var boundOperator =
                BoundBinaryOperator.Bind(syntax.BinaryOperatorType, boundLeft.NamedType, boundRight.NamedType);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.Location, "TODO: unknown symbol!",
                    boundLeft.NamedType, boundRight.NamedType);
                return new BoundErrorExpression(syntax);
            }

            return new BoundBinaryExpression(syntax, boundLeft, boundOperator, boundRight);
        }

        private BoundExpression BindCallExpression(CallSyntax syntax)
        {
            var boundArguments = ImmutableArray.CreateBuilder<BoundExpression>();

            foreach (var argument in syntax.Arguments)
            {
                var boundArgument = BindExpression(argument.Expression);
                boundArguments.Add(boundArgument);
            }

            var boundedExpression = BindExpression(syntax.Expression, true);

            //TODO: can an expression be invoked

            // var symbol = _scope.TryLookupSymbol(syntax.Identifier.Text);
            // if (symbol == null)
            // {
            //     _diagnostics.ReportUndefinedFunction(syntax.Identifier.Location, syntax.Identifier.Text);
            //     return new BoundErrorExpression(syntax);
            // }
            //
            // var function = symbol as MethodSymbol;
            // if (function == null)
            // {
            //     _diagnostics.ReportNotAFunction(syntax.Identifier.Location, syntax.Identifier.Text);
            //     return new BoundErrorExpression(syntax);
            // }

            // if (syntax.Arguments.Count != function.Parameters.Length)
            // {
            //     TextSpan span = TextSpan.FromBounds(0, 0);
            //     // if (syntax.Arguments.Count > function.Parameters.Length)
            //     // {
            //     //     SyntaxNode firstExceedingNode;
            //     //     if (function.Parameters.Length > 0)
            //     //         firstExceedingNode = syntax.Arguments.GetSeparator(function.Parameters.Length - 1);
            //     //     else
            //     //         firstExceedingNode = syntax.Arguments[0];
            //     //     var lastExceedingArgument = syntax.Arguments[syntax.Arguments.Count - 1];
            //     //     span = TextSpan.FromBounds(firstExceedingNode.Span.Start, lastExceedingArgument.Span.End);
            //     // }
            //     // else
            //     // {
            //     //     span = syntax.CloseParenthesisToken.Span;
            //     // }
            //
            //     var location = new TextLocation(SourceText.From("TODO: "), span);
            //     _diagnostics.ReportWrongArgumentCount(location, function.Name, function.Parameters.Length,
            //         syntax.Arguments.Count);
            //     return new BoundErrorExpression(syntax);
            // }

            for (var i = 0; i < syntax.Arguments.Count; i++)
            {
                var argumentLocation = syntax.Arguments[i].Location;
                var argument = boundArguments[i];
                ParameterSymbol parameter = null; //function.Parameters[i];
                boundArguments[i] = BindConversion(argumentLocation, argument, parameter.Type as NamedTypeSymbol);
            }

            return new BoundCallExpression(syntax, boundedExpression, null, boundArguments.ToImmutable());
        }

        private BoundExpression BindConversion(ExpressionSyntax syntax, NamedTypeSymbol namedType,
            bool allowExplicit = false)
        {
            var expression = BindExpression(syntax);
            return BindConversion(syntax.Location, expression, namedType, allowExplicit);
        }

        private BoundExpression BindConversion(TextLocation diagnosticLocation, BoundExpression expression,
            NamedTypeSymbol namedType, bool allowExplicit = false)
        {
            var conversion = Conversion.Classify(expression.NamedType, namedType);

            if (!conversion.Exists)
            {
                // if (expression.NamedType != NamedTypeSymbol.Error && namedType != NamedTypeSymbol.Error)
                //     _diagnostics.ReportCannotConvert(diagnosticLocation, expression.NamedType, namedType);

                return new BoundErrorExpression(expression.Syntax);
            }

            if (!allowExplicit && conversion.IsExplicit)
            {
                _diagnostics.ReportCannotConvertImplicitly(diagnosticLocation, expression.NamedType, namedType);
            }

            if (conversion.IsIdentity)
                return expression;

            return new BoundConversionExpression(expression.Syntax, namedType, expression);
        }

        private LocalSymbol BindVariableDeclaration(object identifier, bool isReadOnly,
            NamedTypeSymbol namedType,
            BoundConstant? constant = null)
        {
            var name = "?" ?? "?";
            //var declare = !identifier.IsMissing;
            var variable = _function == null
                ? (LocalSymbol) new GlobalLocalSymbol(name, isReadOnly, namedType, constant)
                : new LocalLocalSymbol(name, isReadOnly, namedType, constant);

            // if (!_scope.TryDeclareVariable(variable))
            //     _diagnostics.ReportSymbolAlreadyDeclared(identifier.Location, name);

            return variable;
        }

        private LocalSymbol? BindVariableReference(object identifierToken)
        {
            throw  new NotImplementedException();
            
            // var name = identifierToken.Value;
            // switch (_scope.TryLookupSymbol(name))
            // {
            //     case LocalSymbol variable:
            //         return variable;
            //
            //     case null:
            //         _diagnostics.ReportUndefinedVariable(identifierToken.Location, name);
            //         return null;
            //
            //     default:
            //         _diagnostics.ReportNotAVariable(identifierToken.Location, name);
            //         return null;
            // }
        }


        internal NamedTypeSymbol GetSpecialType(SpecialType typeId, SyntaxNode node)
        {
            return GetSpecialType(this._compialtion, typeId, node, _diagnostics);
        }

        internal NamedTypeSymbol GetSpecialType(SpecialType typeId, DiagnosticBag diagnostics, SyntaxNode node)
        {
            return GetSpecialType(this._compialtion, typeId, node, diagnostics);
        }

        internal static NamedTypeSymbol GetSpecialType(Compilation compilation, SpecialType typeId, SyntaxNode node,
            DiagnosticBag diagnostics)
        {
            NamedTypeSymbol typeSymbol = compilation.GetSpecialType(typeId);
            Debug.Assert((object) typeSymbol != null, "Expect an error type if special type isn't found");
            //    ReportUseSiteDiagnostics(typeSymbol, diagnostics, node);
            return typeSymbol;
        }

        private NamedTypeSymbol? LookupType(string name)
        {
            switch (name)
            {
                // case "any":
                //     return NamedTypeSymbol.Any;
                // case "bool":
                //     return NamedTypeSymbol.Bool;
                // case "int":
                //     return NamedTypeSymbol.Int;
                // case "string":
                //     return NamedTypeSymbol.String;
                // default:
                //     return null;
            }

            return null;
        }
    }
}