using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aquila.Compiler.Contracts;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynConstructorBuilder : RoslynConstructor, IConstructorBuilder
    {
        private readonly MethodDefUser _methodDef;
        private List<RoslynParameter> _parameters = new List<RoslynParameter>();


        public RoslynConstructorBuilder(RoslynTypeSystem ts, MethodDefUser methodDef, ITypeDefOrRef declType) : base(ts,
            methodDef, methodDef, declType)
        {
            _methodDef = methodDef;
            _methodDef.Body = new CilBody();


            Body = new RoslynEmitter(ts, this);
        }

        public override IReadOnlyList<IParameter> Parameters => _parameters;

        public IParameter DefineParameter(IType type)
        {
            var dtype = (RoslynType) type;

            var typeSig = dtype.TypeRef.ToTypeSig();
            MethodDef.MethodSig.Params.Add(typeSig);

            MethodDef.Parameters.UpdateParameterTypes();

            var p = MethodDef.Parameters.Last();
            p.CreateParamDef();

            var dp = new RoslynParameter(System, MethodDef, DeclaringTypeReference.Module, p);

            _parameters.Add(dp);

            return dp;
        }

        public RoslynEmitter Body { get; }

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
                var wasFirst = false;
                foreach (var exp in Body.BaseCall)
                {
                    if (wasFirst)
                        tw.W(",");

                    exp.Dump(tw);

                    wasFirst = true;
                }
            }

            Body.Dump(tw);
        }
    }
}