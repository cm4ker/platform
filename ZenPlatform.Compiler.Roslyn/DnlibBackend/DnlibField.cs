using System;
using System.IO;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class SreFieldBuilder : SreField

    {
        public SreFieldBuilder(SreTypeSystem ts, FieldDef fieldDef) : base(ts, fieldDef)
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

    public class SreField
    {
        private readonly SreTypeSystem _ts;
        private SreContextResolver _cr;

        public FieldDef FieldDef { get; }

        public SreField(SreTypeSystem ts, FieldDef fieldDef)
        {
            _ts = ts;
            FieldDef = fieldDef;

            _cr = new SreContextResolver(_ts, FieldDef.Module);
        }

        public bool Equals(SreField other)
        {
            throw new NotImplementedException();
        }

        public string Name => FieldDef.Name;
        public SreType FieldType => _cr.GetType(FieldDef.FieldType);

        public SreType DeclaringType => _cr.GetType(FieldDef.DeclaringType);

        public bool IsPublic => FieldDef.IsPublic;

        public bool IsStatic => FieldDef.IsStatic;

        public bool IsLiteral => FieldDef.IsLiteral;

        public object GetLiteralValue()
        {
            if (FieldDef.IsLiteral && FieldDef.HasConstant)
            {
                return FieldDef.Constant.Value;
            }

            return null;
        }
    }
}