using System;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class RoslynField
    {
        private readonly RoslynTypeSystem _ts;
        private RoslynContextResolver _cr;

        public FieldDef FieldDef { get; }

        public RoslynField(RoslynTypeSystem ts, FieldDef fieldDef)
        {
            _ts = ts;
            FieldDef = fieldDef;

            _cr = new RoslynContextResolver(_ts, FieldDef.Module);
        }

        public bool Equals(RoslynField other)
        {
            throw new NotImplementedException();
        }

        public string Name => FieldDef.Name;
        public RoslynType FieldType => _cr.GetType(FieldDef.FieldType);

        public RoslynType DeclaringType => _cr.GetType(FieldDef.DeclaringType);

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