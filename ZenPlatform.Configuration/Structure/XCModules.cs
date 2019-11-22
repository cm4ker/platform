using System.Collections.Generic;
using System.Xml.Serialization;

namespace ZenPlatform.Configuration.Structure
{


    /// <summary>
    /// Хостит в себе все подключенные модули
    /// </summary>
    public class XCModules : IXCModules
    {
        /// <summary>
        /// Включенные файлы
        /// </summary>
        public List<IXCFile> IncludedFiles { get; set; }
    }
}