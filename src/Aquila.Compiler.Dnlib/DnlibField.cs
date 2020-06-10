using System;
using Aquila.Compiler.Contracts;
using dnlib.DotNet;
using ICustomAttribute = Aquila.Compiler.Contracts.ICustomAttribute;
using IField = Aquila.Compiler.Contracts.IField;
using IType = Aquila.Compiler.Contracts.IType;

namespace Aquila.Compiler.Dnlib
{
    public class DnlibField : IField, IFieldBuilder
    {
        private readonly DnlibTypeSystem _ts;
        private DnlibContextResolver _cr;

        public FieldDef FieldDef { get; }

        public DnlibField(DnlibTypeSystem ts, FieldDef fieldDef)
        {
            _ts = ts;
            FieldDef = fieldDef;

            _cr = new DnlibContextResolver(_ts, FieldDef.Module);
        }

        public bool Equals(IField other)
        {
            throw new NotImplementedException();
        }

        public string Name => FieldDef.Name;
        public IType FieldType => _cr.GetType(FieldDef.FieldType);
        public IType DeclaringType { get; }

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

        public void SetAttribute(ICustomAttribute attr)
        {
            throw new NotImplementedException();
        }
    }
}