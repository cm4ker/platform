using System.Collections.Generic;

namespace Aquila.Core.Contracts.TypeSystem
{
    public interface IMDComponent
    {
        List<string> EntityReferences { get; set; }
    }
}