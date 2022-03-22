using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Symbols.Synthesized;

namespace Aquila.CodeAnalysis.Emit
{
    partial class PEModuleBuilder
    {
        /// <summary>
        /// Create real CIL entry point, where it calls given method.
        /// </summary>
        internal void CreateEntryPoint(MethodSymbol sourceEP, DiagnosticBag diagnostic)
        {
            // "static int Main(string[] args)"
            var epMethodSymbol = new SynthesizedMethodSymbol(this.EntryPointType, "Main", true, false,
                _compilation.CoreTypes.Int32, Accessibility.Private);

            epMethodSymbol.SetParameters(new SynthesizedParameterSymbol(epMethodSymbol,
                ArrayTypeSymbol.CreateSZArray(this.Compilation.SourceAssembly, this.Compilation.CoreTypes.String), 0,
                RefKind.None, "args"));

            //
            var body = MethodGenerator.GenerateMethodBody(this, epMethodSymbol,
                (il) =>
                {
                    if (_compilation.Options.OutputKind == OutputKind.ConsoleApplication)
                    {
                       var types = this.Compilation.CoreTypes;
                        var args_place = new ParamPlace(epMethodSymbol.Parameters[0]);

                        // CreateConsole(string mainscript, params string[] args)
                        var create_method = types.AqContext.Symbol.LookupMember<MethodSymbol>("CreateConsole",
                            m => m.ParameterCount == 1 &&
                                 m.Parameters[0].Type == args_place.Type); // params string[] args

                        Debug.Assert(create_method != null);

                        args_place.EmitLoad(il); // args

                        //il.EmitCall(this, diagnostic, ILOpCode.Call, create_method);
                        il.EmitOpCode(ILOpCode.Call, +1);
                        il.EmitToken(create_method, null, diagnostic);

                        il.EmitCall(this, diagnostic, ILOpCode.Call, sourceEP);

                        if (sourceEP.ReturnType.IsVoid())
                        {
                            il.EmitIntConstant(0);
                            il.EmitRet(false);
                        }
                    }
                    else
                    {
                        il.EmitIntConstant(0);
                        il.EmitRet(false);
                    }
                },
                null, diagnostic, false);

            SetMethodBody(epMethodSymbol, body);

            //
            this.EntryPointType.EntryPointSymbol = epMethodSymbol;
        }

        /// <summary>
        /// Emits body of scripts main wrapper.
        /// </summary>
        /// <param name="wrapper">&lt;Main&gt;`0 method, that calls real Main.</param>
        /// <param name="main">Real scripts main method.</param>
        /// <param name="diagnostic">DiagnosticBag.</param>
        internal void CreateMainMethodWrapper(MethodSymbol wrapper, MethodSymbol main, DiagnosticBag diagnostic)
        {
            if (wrapper == null)
                return;

            // generate body of <wrapper> calling <main>
            Debug.Assert(wrapper.IsStatic);
            Debug.Assert(main.IsStatic);

            Debug.Assert(wrapper.ParameterCount == main.ParameterCount);

            //
            var body = MethodGenerator.GenerateMethodBody(this, wrapper,
                (il) => { },
                null, diagnostic, false);

            SetMethodBody(wrapper, body);
        }
    }
}