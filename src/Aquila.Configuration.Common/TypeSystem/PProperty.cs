using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Core.Contracts.TypeSystem;
using dnlib.DotNet;
using IPType = Aquila.Core.Contracts.TypeSystem.IPType;

namespace Aquila.Configuration.Common.TypeSystem
{
    public class PProperty : IPProperty
    {
        private readonly TypeManager _ts;

        private Guid _typeId;

        internal PProperty(TypeManager ts)
        {
            _ts = ts;
        }

        public Guid Id { get; set; }

        public Guid ParentId { get; set; }

        public string Name { get; set; }

        public bool IsSelfLink { get; set; }

        public bool IsSystem { get; set; }

        public bool IsUnique { get; set; }

        public bool IsReadOnly { get; set; }

        public IPType Type => _ts.FindType(_typeId);


        public void SetType(Guid guid)
        {
            _typeId = guid;
        }

        public ITypeManager TypeManager => _ts;
    }
}