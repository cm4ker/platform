using System.Collections.Generic;

namespace ZenPlatform.Configuration.Structure
{
    public interface IXCSchedules
    {
        List<IXCFile> IncludedFiles { get; set; }
    }
}