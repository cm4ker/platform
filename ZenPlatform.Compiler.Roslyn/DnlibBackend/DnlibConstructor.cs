using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class SreConstructor : SreInvokableBase
    {
        public SreConstructor(SreTypeSystem ts, IMethodDefOrRef method, MethodDef methodDef, ITypeDefOrRef declType)
            : base(ts, method, methodDef,
                declType)
        {
        }

        public bool Equals(SreConstructor other)
        {
            throw new NotImplementedException();
        }

        public override void DumpRef(TextWriter tw)
        {
            DeclaringType.DumpRef(tw);
        }
    }

    public class SreConstructorBuilder : SreConstructor
    {
        private readonly MethodDefUser _methodDef;
        private List<SreParameter> _parameters = new List<SreParameter>();


        public SreConstructorBuilder(SreTypeSystem ts, MethodDefUser methodDef, ITypeDefOrRef declType) : base(ts,
            methodDef, methodDef, declType)
        {
            _methodDef = methodDef;
            _methodDef.Body = new CilBody();


            Body = new RBlockBuilder(ts, this);
        }

        public override IReadOnlyList<SreParameter> Parameters => _parameters;

        public SreParameter DefineParameter(SreType type)
        {
            var dtype = (SreType) type;

            var typeSig = dtype.TypeRef.ToTypeSig();
            MethodDef.MethodSig.Params.Add(typeSig);

            MethodDef.Parameters.UpdateParameterTypes();

            var p = MethodDef.Parameters.Last();
            p.CreateParamDef();

            var dp = new SreParameter(System, MethodDef, DeclaringTypeReference.Module, p);

            _parameters.Add(dp);

            return dp;
        }

        public RBlockBuilder Body { get; }

        public void Dump(TextWriter tw)
        {
            tw.Write("public ");

            tw.W(DeclaringType.Name);

            using (tw.Parenthesis())
            {
                var wasFirst = false;
                foreach (var parameter in Parameters)
                {
                    if (wasFirst)
                        tw.W(", ");

                    parameter.Dump(tw);

                    wasFirst = true;
                }
            }

            tw.W(": base");

            using (tw.Parenthesis())
            {
                foreach (var exp in Body.BaseCall)
                {
                    exp.Dump(tw);
                }
            }

            Body.Dump(tw);
        }
    }
}