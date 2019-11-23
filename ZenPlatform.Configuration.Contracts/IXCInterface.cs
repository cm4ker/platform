using System.Collections.Generic;

namespace ZenPlatform.Configuration.Structure
{
    public interface IXCInterface
    {
        List<IXCFile> IncludedFiles { get; set; }
    }
}