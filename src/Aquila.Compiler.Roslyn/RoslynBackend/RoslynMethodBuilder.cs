using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynMethodBuilder : RoslynMethod
    {
        private List<RoslynParameter> _parameters = new List<RoslynParameter>();


        public RoslynMethodBuilder(RoslynTypeSystem typeSystem, MethodDef method, ITypeDefOrRef declaringType) :
            base(typeSystem, method, method, declaringType)
        {
            Body = new RoslynEmitter(typeSystem, this);
        }

        public RoslynParameter DefineParameter(string name, RoslynType type, bool isOut, bool isRef)
        {
            var dtype = (RoslynType) type;

            var typeSig = dtype.TypeRef.ToTypeSig();
            MethodDef.MethodSig.Params.Add(typeSig);
            MethodDef.Parameters.UpdateParameterTypes();

            var p = MethodDef.Parameters.Last();
            p.CreateParamDef();
            p.Name = name;

            var dp = new RoslynParameter(System, MethodDef, DeclaringTypeReference.Module, p);

            _parameters.Add(dp);

            return dp;
        }

        public RoslynMethodBuilder WithReturnType(RoslynType type)
        {
            MethodDef.ReturnType = ContextResolver.GetReference(type.ToTypeRef()).ToTypeSig();

            return this;
        }

        public RoslynEmitter Body { get; }


        public void SetAttribute(RoslynCustomAttribute attr)
        {
            var dnlibAttr = attr;
            dnlibAttr.ImportAttribute(MethodDef.Module);
            MethodDef.CustomAttributes.Add(dnlibAttr.GetCA());
            ((List<RoslynCustomAttribute>) CustomAttributes).Add(dnlibAttr);
        }

        public void Dump(TextWriter tw)
        {
            foreach (var attribute in CustomAttributes)
            {
                using (tw.SquareBrace())
                {
                    attribute.AttributeType.DumpRef(tw);

                    if (attribute.Parameters.Count > 0)
                        using (tw.Parenthesis())
                        {
                            var wasFirst = false;

                            foreach (var parameter in attribute.Parameters)
                            {
                                if (wasFirst)
                                    tw.W(",");


                                Literal l = parameter switch
                                {
                                    int i => new Literal(i),
                                    string s => new Literal(s),
                                    double d => new Literal(d),
                                    _ => throw new Exception("Not supported")
                                };

                                l.Dump(tw);

                                wasFirst = true;
                            }
                        }
                }

                tw.WriteLine();
            }
            
            tw.Write("public ");

            if (IsStatic)
                tw.Write("static ");

            if (MethodDef.HasOverrides)
                tw.Write("override ");
            else if (MethodDef.IsVirtual)
                tw.Write("virtual ");


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