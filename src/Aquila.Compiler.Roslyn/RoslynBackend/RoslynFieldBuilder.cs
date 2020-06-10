using System.IO;
using Aquila.Compiler.Contracts;
using dnlib.DotNet;
using ICustomAttribute = Aquila.Compiler.Contracts.ICustomAttribute;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynFieldBuilder : RoslynField, IFieldBuilder

    {
        public RoslynFieldBuilder(RoslynTypeSystem ts, FieldDef fieldDef) : base(ts, fieldDef)
        {
        }

        public void Dump(TextWriter tw)
        {
            if (IsPublic)
                tw.W("public ");
            if (IsStatic)
                tw.W("static ");

            FieldType.DumpRef(tw);
            tw.Space().W(Name).Comma();
        }

        public void SetAttribute(ICustomAttribute attr)
        {
        }
    }
}