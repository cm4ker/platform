using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Contracts.TypeSystem;
using dnlib.DotNet;
using IPType = Aquila.Core.Contracts.TypeSystem.IPType;

namespace Aquila.Configuration.Common.TypeSystem
{
    public class PProperty : PMember, IPProperty
    {
        private Guid _typeId;

        internal PProperty(Guid parentId, TypeManager ts) : base(parentId, ts)
        {
        }

        internal PProperty(Guid id, Guid parentId, TypeManager ts) : base(id, parentId, ts)
        {
        }

        public bool IsSelfLink { get; set; }

        public bool IsSystem { get; set; }

        public bool IsUnique { get; set; }

        public bool IsReadOnly { get; set; }

        public IPType Type => TypeManager.FindType(_typeId);

        public void SetType(Guid guid)
        {
            _typeId = guid;
        }
    }


    public class PMethod : PInvokable
    {
        private Guid _typeId;

        internal PMethod(Guid id, Guid parentId, TypeManager tm) : base(id, parentId, tm)
        {
        }

        internal PMethod(Guid parentId, TypeManager tm) : base(parentId, tm)
        {
        }

        public IPType ReturnType => TypeManager.FindType(_typeId);

        public void SetReturnType(Guid typeId)
        {
            _typeId = typeId;
        }

        public void SetReturnType(IPType type)
        {
            _typeId = type.Id;
        }
    }
}