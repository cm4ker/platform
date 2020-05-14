using Aquila.Core.Contracts.Data;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.SerializableTypeComponent
{
    public class SerializableTypeComponent : IBuildingParticipant
    {
        private readonly IComponent _c;

        public SerializableTypeComponent(IComponent c)
        {
            _c = c;

            Generator = new PlatformGenerator(_c);
        }

        public IPlatformGenerator Generator { get; }


        public void OnInitializing()
        {
        }
    }
}