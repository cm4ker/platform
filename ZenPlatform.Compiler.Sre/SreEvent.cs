using System.Reflection;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Sre
{
    class SreEvent : SreMemberInfo, IEventInfo
    {
        public EventInfo Event { get; }

        public SreEvent(SreTypeSystem system, EventInfo ev) : base(system, ev)
        {
            Event = ev;
            Add = new SreMethod(system, ev.AddMethod);
        }

        public IMethod Add { get; }
        public bool Equals(IEventInfo other) => (other as SreEvent)?.Event.Equals(Event) == true;
    }
}