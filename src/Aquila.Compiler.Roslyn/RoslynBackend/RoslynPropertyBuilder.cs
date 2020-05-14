using System;
using System.Collections.Generic;
using System.IO;
using dnlib.DotNet;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynPropertyBuilder : RoslynProperty
    {
        public RoslynPropertyBuilder WithSetter(RoslynMethod method)
        {
            _setter = method;
            PropertyDef.SetMethod = ((RoslynInvokableBase) method).MethodDef;
            return this;
        }

        public RoslynPropertyBuilder WithGetter(RoslynMethod method)
        {
            _getter = method;
            PropertyDef.GetMethod = ((RoslynInvokableBase) method).MethodDef;
            return this;
        }

        public void SetAttribute(RoslynCustomAttribute attr)
        {
            var dnlibAttr = attr;
            dnlibAttr.ImportAttribute(PropertyDef.Module);
            PropertyDef.CustomAttributes.Add(dnlibAttr.GetCA());
            ((List<RoslynCustomAttribute>) CustomAttributes).Add(dnlibAttr);
        }

        public RoslynPropertyBuilder(RoslynTypeSystem ts, PropertyDef property, ITypeDefOrRef declaringTypeInternal) :
            base(ts,
                property, declaringTypeInternal)
        {
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

            tw.W("public ");
            PropertyType.DumpRef(tw);

            tw.Space().W(Name);

            using (tw.CurlyBrace())
            {
                if (_getter != null)
                {
                    tw.W("get => ");
                    _getter.DumpRef(tw);
                    tw.W("()").Comma();
                }

                if (_setter != null)
                {
                    tw.W("set => ");
                    _setter.DumpRef(tw);
                    tw.W("(value)").Comma();
                }
            }
        }
    }
}