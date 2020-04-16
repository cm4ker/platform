using System.IO;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class RoslynFieldBuilder : RoslynField

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
    }
}