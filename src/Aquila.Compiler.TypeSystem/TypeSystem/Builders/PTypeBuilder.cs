using System;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem.Builders
{
    /// <summary>
    /// Describes type for platform in different parts
    /// </summary>
    public sealed class PTypeBuilder : PType
    {
        private readonly TypeManager _tm;
        private Guid _id;
        private Guid _baseId;
        private Guid? _componentId;
        private ScopeAffects _scope;
        private string _name;

        public PTypeBuilder(Guid id, TypeManager tm) : base(tm)
        {
            _tm = tm;
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

        public void SetParentId(Guid? parentId)
        {
            SetParentIdCore(parentId);
        }

        public void SetComponent(Guid? componentId)
        {
            _componentId = componentId;
        }

        public void SetFullName(string fullName)
        {
            SetFullNameCore(fullName);
        }

        public void SetNamespace(string @namespace)
        {
        }

        public void SetScope(ScopeAffects scope)
        {
            _scope = scope;
        }

        public PPropertyBuilder DefineProperty()
        {
            var prop = new PPropertyBuilder(Guid.NewGuid(), Id, _tm);
            _tm.Register(prop);
            return prop;
        }

        public PMethodBuilder DefineMethod()
        {
            var method = new PMethodBuilder(Guid.NewGuid(), Id, _tm);
            _tm.Register(method);
            return method;
        }

        public PFieldBuilder DefineField()
        {
            var field = new PFieldBuilder(Guid.NewGuid(), Id, _tm);

            _tm.Register(field);
            return field;
        }
    }
}