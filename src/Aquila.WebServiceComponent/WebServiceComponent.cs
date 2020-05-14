using Aquila.Core.Contracts.Data;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.WebServiceComponent
{
    public class WebServiceComponent : IBuildingParticipant
    {
        private readonly IComponent _c;

        public WebServiceComponent(IComponent c)
        {
            _c = c;

            Generator = new WSPlatformGenerator(_c);
        }

        public IPlatformGenerator Generator { get; }


        public void OnInitializing()
        {
        }
    }
}