using System;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    /// <summary>
    /// Describes type for platform in different parts
    /// </summary>
    public sealed class PTypeBuilder : PType, IPTypeBuilder
    {
        private Guid _id;
        private Guid _baseId;
        private Guid? _componentId;
        private ScopeAffects _scope;

        public PTypeBuilder(Guid id, TypeManager tm) : base(tm)
        {
            _id = Guid.NewGuid();
        }

        public PTypeBuilder(TypeManager ts) : this(Guid.NewGuid(), ts)
        {
        }

        public override Guid Id => _id;

        public override Guid? BaseId => _baseId;

        public override Guid? ComponentId => _componentId;

        public override ScopeAffects Scope => _scope;

        public void SetBase(Guid baseId)
        {
            _baseId = baseId;
        }

        public void SetComponent(Guid? componentId)
        {
            _componentId = componentId;
        }

        public void SetName(string name)
        {
        }

        public void SetNamespace(string @namespace)
        {
        }

        public void SetScope(ScopeAffects scope)
        {
            _scope = scope;
        }
    }
}