using System;
using System.ComponentModel.Design;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.Builders
{
    public class PPropertyBuilder : PProperty
    {
        private Guid _typeId;
        private Guid _getterId;
        private Guid _setterId;
        private bool _isSelfLink;
        private bool _isSystem;
        private bool _isUnique;
        private bool _isReadOnly;
        private string _name;

        internal PPropertyBuilder(Guid id, Guid parentId, TypeManager ts) : base(id, parentId, ts)
        {
        }

        public override PType Type => TypeManager.FindType(_typeId);

        public override PMethod Getter => TypeManager.FindMethod(_getterId);

        public override PMethod Setter => TypeManager.FindMethod(_setterId);

        public override bool IsReadOnly => _isReadOnly;

        public override bool IsUnique => _isUnique;

        public override bool IsSystem => _isSystem;

        public override bool IsSelfLink => _isSelfLink;

        public override string Name => _name;

        public void SetName(string name)
        {
            _name = name;
        }

        public void SetIsReadOnly(bool value)
        {
            _isReadOnly = value;
        }

        public void SetGetter(Guid id)
        {
            _getterId = id;
        }

        public void SetSetter(Guid id)
        {
            _setterId = id;
        }

        public void SetType(Guid guid)
        {
            _typeId = guid;
        }

        public void SetIsSelfLink(bool value)
        {
            _isSelfLink = value;
        }

        public void SetIsSystem(bool value)
        {
            _isSystem = value;
        }

        public void SetIsUnique(bool value)
        {
            _isUnique = value;
        }
    }
}