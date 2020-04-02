using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class SreMethodBuilder : SreMethod
    {
        private List<SreParameter> _parameters = new List<SreParameter>();


        public SreMethodBuilder(SreTypeSystem typeSystem, MethodDef method, ITypeDefOrRef declaringType) :
            base(typeSystem, method, method, declaringType)
        {
            Body = new RBlockBuilder(typeSystem, this);
        }

        public SreParameter DefineParameter(string name, SreType type, bool isOut, bool iTmpSreBackendf)
        {
            var dtype = (SreType) type;

            var typeSig = dtype.TypeRef.ToTypeSig();
            MethodDef.MethodSig.Params.Add(typeSig);
            MethodDef.Parameters.UpdateParameterTypes();

            var p = MethodDef.Parameters.Last();
            p.CreateParamDef();
            p.Name = name;

            var dp = new SreParameter(System, MethodDef, DeclaringTypeReference.Module, p);

            _parameters.Add(dp);

            return dp;
        }

        public SreMethodBuilder WithReturnType(SreType type)
        {
            MethodDef.ReturnType = ContextResolver.GetReference(type.ToTypeRef()).ToTypeSig();

            return this;
        }


        public RBlockBuilder Body { get; }

        public void Dump(TextWriter tw)
        {
            tw.Write("public ");

            if (IsStatic)
                tw.Write("static ");

            if(MethodDef.HasOverrides)
                tw.Write("override ");
            
            ReturnType.DumpRef(tw);

            tw.Space().W(Name);

            using (tw.Parenthesis())
            {
                var maxIndex = _parameters.Count - 1;
                var index = 0;

                foreach (var parameter in _parameters)
                {
                    parameter.Dump(tw);

                    if (maxIndex > 0 && index != maxIndex)
                        tw.W(", ");

                    index++;
                }
            }


            Body.Dump(tw);
        }


        
    }
}