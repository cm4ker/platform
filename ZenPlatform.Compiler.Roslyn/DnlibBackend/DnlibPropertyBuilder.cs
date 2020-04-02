using System.Collections.Generic;
using System.IO;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class SrePropertyBuilder : SreProperty
    {
        public SrePropertyBuilder WithSetter(SreMethod method)
        {
            _setter = method;
            PropertyDef.SetMethod = ((SreInvokableBase) method).MethodDef;
            return this;
        }

        public SrePropertyBuilder WithGetter(SreMethod method)
        {
            _getter = method;
            PropertyDef.GetMethod = ((SreInvokableBase) method).MethodDef;
            return this;
        }

        public void SetAttribute(SreCustomAttribute attr)
        {
            var dnlibAttr = attr;
            dnlibAttr.ImportAttribute(PropertyDef.Module);
            PropertyDef.CustomAttributes.Add(dnlibAttr.GetCA());
            ((List<SreCustomAttribute>) CustomAttributes).Add(dnlibAttr);
        }

        public SrePropertyBuilder(SreTypeSystem ts, PropertyDef property, ITypeDefOrRef declaringTypeInternal) : base(ts,
            property, declaringTypeInternal)
        {
        }

        public void Dump(TextWriter tw)
        {
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