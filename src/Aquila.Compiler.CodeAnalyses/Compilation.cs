using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Aquila.Compiler.Contracts;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Binding;
using Aquila.Language.Ast.Extension;
using Aquila.Language.Ast.Symbols;
using Aquila.Language.Ast.Symbols.PE;
using Aquila.Language.Ast.Symbols.Source;
using Binder = Aquila.Language.Ast.Binding.Binder;

namespace Aquila.Compiler
{
    public class Compilation
    {
        private BoundGlobalScope? _globalScope;
        private SourceAssemblySymbol _assembly;

        private Compilation(bool isScript, Compilation? previous, IAssemblyPlatform platform,
            params SyntaxTree[] syntaxTrees)
        {
            IsScript = isScript;
            Previous = previous;
            SyntaxTrees = syntaxTrees.ToImmutableArray();

            _assembly = new SourceAssemblySymbol();
            _assembly.Builder = platform.CreateAssembly("Debug");
            _assembly.Builder.DefineType("", "Program",
                TypeAttributes.Public | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
                platform.TypeSystem.GetSystemBindings().Object);

            var corLib = new PEAssemblySymbol(platform.TypeSystem.GetSystemBindings().CorAssembly);

            corLib.SetCorLibrary(corLib);
            _assembly.SetCorLibrary(corLib);
        }

        public static Compilation Create(IAssemblyPlatform platform, params SyntaxTree[] syntaxTrees)
        {
            return new Compilation(isScript: false, previous: null, platform, syntaxTrees);
        }

        public static Compilation CreateScript(Compilation? previous, IAssemblyPlatform platform,
            params SyntaxTree[] syntaxTrees)
        {
            return new Compilation(isScript: true, previous, platform, syntaxTrees);
        }

        public bool IsScript { get; }
        public Compilation? Previous { get; }
        public ImmutableArray<SyntaxTree> SyntaxTrees { get; }
        public MethodSymbol? MainFunction => GlobalScope.MainFunction;
        public ImmutableArray<MethodSymbol> Functions => GlobalScope.Functions;
        public ImmutableArray<LocalSymbol> Variables => GlobalScope.Variables;

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(this, IsScript, Previous?.GlobalScope, SyntaxTrees);
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }

                return _globalScope;
            }
        }

        public IEnumerable<Symbol> GetSymbols()
        {
            var submission = this;
            var seenSymbolNames = new HashSet<string>();

            var builtinFunctions = BuiltinFunctions.GetAll().ToList();

            while (submission != null)
            {
                foreach (var function in submission.Functions)
                    if (seenSymbolNames.Add(function.Name))
                        yield return function;

                foreach (var variable in submission.Variables)
                    if (seenSymbolNames.Add(variable.Name))
                        yield return variable;

                foreach (var builtin in builtinFunctions)
                    if (seenSymbolNames.Add(builtin.Name))
                        yield return builtin;

                submission = submission.Previous;
            }
        }

        internal NamedTypeSymbol GetSpecialType(SpecialType specialType)
        {
            if (specialType <= SpecialType.None || specialType > SpecialType.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(specialType),
                    $"Unexpected SpecialType: '{(int) specialType}'.");
            }

            NamedTypeSymbol result = Assembly.GetSpecialType(specialType);

            Debug.Assert(result.SpecialType == specialType);
            return result;
        }

        internal AssemblySymbol Assembly
        {
            get { return _assembly; }
        }


        internal BoundProgram GetProgram()
        {
            var previous = Previous == null ? null : Previous.GetProgram();
            return Binder.BindProgram(this, IsScript, previous, GlobalScope);
        }

        public void EmitTree(TextWriter writer)
        {
            if (GlobalScope.MainFunction != null)
                EmitTree(GlobalScope.MainFunction, writer);
            else if (GlobalScope.ScriptFunction != null)
                EmitTree(GlobalScope.ScriptFunction, writer);
        }

        public void EmitTree(MethodSymbol symbol, TextWriter writer)
        {
            var program = GetProgram();
            symbol.WriteTo(writer);
            writer.WriteLine();
            if (!program.Functions.TryGetValue(symbol, out var body))
                return;
            //  body.WriteTo(writer);
        }

        // TODO: References should be part of the compilation, not arguments for Emit
        public ImmutableArray<Diagnostic> Emit(string moduleName, string[] references, string outputPath)
        {
            var parseDiagnostics = SyntaxTrees.SelectMany(st => st.Diagnostics);

            var diagnostics = parseDiagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();
            if (diagnostics.HasErrors())
                return diagnostics;

            var program = GetProgram();


            throw new NotImplementedException();
            //return Emitter.Emit(program, moduleName, references, outputPath);
        }
    }

    public static class DiagnosticExtensions
    {
        public static bool HasErrors(this ImmutableArray<Diagnostic> diagnostics)
        {
            return diagnostics.Any(d => d.IsError);
        }

        public static bool HasErrors(this IEnumerable<Diagnostic> diagnostics)
        {
            return diagnostics.Any(d => d.IsError);
        }
    }
}