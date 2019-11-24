using System.Collections.Generic;

namespace ZenPlatform.Configuration.Structure
{
    public interface IXCModules
    {
        /// <summary>
        /// Включенные файлы
        /// </summary>
        List<IXCFile> IncludedFiles { get; set; }
    }
}