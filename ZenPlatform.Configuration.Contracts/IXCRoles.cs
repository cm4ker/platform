using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure
{
    public interface IXCRoles : IChildItem<IXCRoot>
    {
        List<IXCBlob> Blobs { get; set; }
        ChildItemCollection<IXCRoles, IXCRole> Items { get; }
        /// <summary>
        /// Сохранить роли
        /// </summary>
        void Save();

        /// <summary>
        /// Загрузить роли
        /// </summary>
        void Load();
    }
}