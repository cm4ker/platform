using System;
using System.Collections.Generic;
using System.IO;
using Aquila.Compiler.Contracts;
using dnlib.DotNet;
using ICustomAttribute = Aquila.Compiler.Contracts.ICustomAttribute;
using IMethod = Aquila.Compiler.Contracts.IMethod;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynPropertyBuilder : RoslynProperty, IPropertyBuilder
    {
        private RoslynMethod _getter;
        private RoslynMethod _setter;

        public override IMethod Getter => _getter;
        public override IMethod Setter => _setter;


        public IPropertyBuilder WithSetter(IMethod method)
        {
            _setter = (RoslynMethod) method;
            PropertyDef.SetMethod = _setter.MethodDef;
            return this;
        }

        public IPropertyBuilder WithGetter(IMethod method)
        {
            _getter = (RoslynMethod) method;
            PropertyDef.GetMethod = _getter.MethodDef;
            return this;
        }

        public void SetAttribute(ICustomAttribute attr)
        {
            var dnlibAttr = (RoslynCustomAttribute) attr;
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