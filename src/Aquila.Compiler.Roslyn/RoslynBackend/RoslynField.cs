using System;
using dnlib.DotNet;
using IField = Aquila.Compiler.Contracts.IField;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynField : IField
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

        protected bool Equals(RoslynField other)
        {
            throw new NotImplementedException();
        }

        public string Name => FieldDef.Name;
        public IType FieldType => _cr.GetType(FieldDef.FieldType);

        public IType DeclaringType => _cr.GetType(FieldDef.DeclaringType);

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

        public bool Equals(IField other)
        {
            throw new NotImplementedException();
        }
    }
}