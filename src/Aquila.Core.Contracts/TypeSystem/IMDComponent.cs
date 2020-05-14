using System.Collections.Generic;

namespace Aquila.Configuration.Contracts.TypeSystem
{
    public interface IMDComponent
    {
        List<string> EntityReferences { get; set; }
    }
}