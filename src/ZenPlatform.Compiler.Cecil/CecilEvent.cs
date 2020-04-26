using Mono.Cecil;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Cecil
{
    class CecilEvent : IEventInfo
    {
        private readonly TypeReference _declaringType;
        public CecilTypeSystem TypeSystem { get; }
        public EventDefinition Event { get; }

        public CecilEvent(CecilTypeSystem typeSystem, EventDefinition ev, TypeReference declaringType)
        {
            _declaringType = declaringType;
            TypeSystem = typeSystem;
            Event = ev;
        }

        public bool Equals(IEventInfo other) => other is CecilEvent cp && cp.Event == Event;
        public string Name => Event.Name;

        private IMethod _getter;

        public IMethod Add => Event.AddMethod == null
            ? null
            : _getter ?? (_getter = TypeSystem.Resolve(Event.AddMethod, _declaringType));
    }
}