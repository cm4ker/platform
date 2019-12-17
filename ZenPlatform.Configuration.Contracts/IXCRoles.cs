using System.Collections.Generic;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
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