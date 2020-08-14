// using System;
// using System.Collections.Generic;
// using System.Collections.Immutable;
// using System.Linq;
// using System.Security.Cryptography.Xml;
// using Aquila.CodeAnalysis.Symbols.Synthesized;
// using Aquila.Syntax.Ast;
// using Aquila.Syntax.Ast.Functions;
// using Aquila.Syntax.Ast.Statements;
// using Aquila.Syntax.Syntax;
// using Microsoft.CodeAnalysis;
// using Pchp.CodeAnalysis.FlowAnalysis;
//
// namespace Aquila.CodeAnalysis.Symbols.Source
// {
//     internal partial class SourceLambdaSymbol : SourceRoutineSymbol
//     {
//         readonly LambdaFunctionExpr _syntax;
//
//         /// <summary>
//         /// The type containing declaration of this lambda method.
//         /// </summary>
//         protected NamedTypeSymbol Container { get; }
//
//         /// <summary>
//         /// Whether <c>$this</c> is pased to the routine (non static lambda).
//         /// </summary>
//         internal bool UseThis { get; }
//
//         FieldSymbol _lazyRoutineInfoField;    // internal static RoutineInfo !name;
//
//         public SourceLambdaSymbol(LambdaFunctionExpr syntax, NamedTypeSymbol container)
//         {
//             Container = container ?? throw new ArgumentNullException(nameof(container));
//             _syntax = syntax;
//             UseThis = !syntax.IsStatic;
//         }
//
//         /// <summary>
//         /// A field representing the function info at runtime.
//         /// Lazily associated with index by runtime.
//         /// </summary>
//         internal FieldSymbol EnsureRoutineInfoField(Pchp.CodeAnalysis.Emit.PEModuleBuilder module)
//         {
//             if (_lazyRoutineInfoField == null)
//             {
//                 _lazyRoutineInfoField = module.SynthesizedManager
//                     .GetOrCreateSynthesizedField(Container, this.DeclaringCompilation.CoreTypes.RoutineInfo, $"[routine]{this.MetadataName}", Accessibility.Internal, true, true, true);
//             }
//
//             return _lazyRoutineInfoField;
//         }
//
//         /// <summary>Parameter containing reference to <c>Closure</c> object.</summary>
//         internal ParameterSymbol ClosureParameter => ImplicitParameters[0];
//
//         internal override bool RequiresLateStaticBoundParam => false;   // <static> is passed from <closure>.scope
//
//         protected override IEnumerable<ParameterSymbol> BuildImplicitParams()
//         {
//             return new[]
//             {
//                 // Closure <closure> // treated as synthesized not implicit
//                 new SynthesizedParameterSymbol(this, DeclaringCompilation.CoreTypes.Closure, 0, RefKind.None, name: "<closure>"),
//             };
//         }
//
//         protected override IEnumerable<SourceParameterSymbol> BuildSrcParams(Signature signature, PHPDocBlock phpdocOpt = null)
//         {
//             // [use params], [formal params]
//             return base.BuildSrcParams(UseParams.Concat(signature.FormalParams), phpdocOpt);
//         }
//
//         internal override IList<Statement> Statements => _syntax.Body.Statements;
//
//         internal override Signature SyntaxSignature => _syntax.Signature;
//
//         internal virtual IList<FormalParam> UseParams => _syntax.UseParams;
//
//         internal override TypeRef SyntaxReturnType => _syntax.ReturnType;
//
//         internal override AstNode Syntax => _syntax;
//
//         internal override PHPDocBlock PHPDocBlock => _syntax.PHPDoc;
//
//         internal override SourceFileSymbol ContainingFile => Container.GetContainingFileSymbol();
//
//         public override string Name => WellKnownPchpNames.LambdaMethodName;
//
//         public override Symbol ContainingSymbol => Container;
//
//         public override ImmutableArray<Location> Locations
//         {
//             get
//             {
//                 return ImmutableArray.Create(Location.Create(ContainingFile.SyntaxTree, _syntax.Span.ToTextSpan()));
//             }
//         }
//
//         public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
//         {
//             get
//             {
//                 throw new NotImplementedException();
//             }
//         }
//
//         public override Accessibility DeclaredAccessibility => Accessibility.Private;
//
//         public override bool IsStatic => true;
//
//         public override bool IsAbstract => false;
//
//         public override bool IsSealed => false;
//
//         protected override TypeRefContext CreateTypeRefContext() => new TypeRefContext(DeclaringCompilation, Container as SourceTypeSymbol, thisType: null);
//     }
//
//     internal partial class SourceArrowFnSymbol : SourceLambdaSymbol
//     {
//         readonly ImmutableArray<Statement> _body;
//         readonly Parameter[] _useparams;
//
//         public SourceArrowFnSymbol(ArrowFunctionExpr syntax, NamedTypeSymbol container)
//             : base(syntax, container)
//         {
//             _body = ImmutableArray.Create<Statement>(new JumpStmt(syntax.Expression.Span, JumpStmt.Types.Return, syntax.Expression));
//             _useparams = EnumerateCapturedVariables(syntax)
//                 .Select(v => new FormalParam(Span.Invalid, v.Value, Span.Invalid, null, FormalParam.Flags.Default, null))
//                 .ToArray();
//         }
//
//         static IReadOnlyCollection<VariableName> EnumerateCapturedVariables(ArrowFunctionExpr fn)
//         {
//             var capturedvars = new HashSet<VariableName>();
//
//             // collect all variables in fn
//             foreach (var v in fn.Expression.SelectLocalVariables())
//             {
//                 capturedvars.Add(v.VarName);
//             }
//
//             // remove vars specified in parameters
//             foreach (var p in fn.Signature.FormalParams)
//             {
//                 capturedvars.Remove(p.Name.Name);
//             }
//
//             //// intersect with variables from parent function scope
//             //var containingvars = new HashSet<VariableName>();
//
//             //foreach (var v in fn.GetContainingRoutine().SelectLocalVariables())
//             //{
//             //    containingvars.Add(v.VarName);
//             //}
//
//             //capturedvars.IntersectWith(containingvars);
//
//             //
//             return capturedvars;
//         }
//
//         internal override IList<Statement> Statements => _body;
//
//         internal override IList<Parameter> UseParams => _useparams;
//     }
// }
