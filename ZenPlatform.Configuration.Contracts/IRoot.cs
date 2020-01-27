using System;
using System.IO;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IConfigurationManipulator
    {
        IRoot Load(IXCConfigurationStorage storage);

        IRoot Create(string projectName);

        Stream SaveToStream(IRoot root);

        string GetHash(IRoot root);

        bool Equals(IRoot a, IRoot b);
    }


    public interface IRoot
    {
        /// <summary>
        /// Идентификатор конфигурации
        /// </summary>
        Guid ProjectId { get; set; }

        /// <summary>
        /// Имя конфигурации
        /// </summary>
        string ProjectName { get; set; }

        /// <summary>
        /// Версия конфигурации
        /// </summary>
        string ProjectVersion { get; set; }

        /// <summary>
        /// Менеджер типов платформы
        /// </summary>
        ITypeManager TypeManager { get; }

        /// <summary>
        /// Созранить объект в контексте другого хранилища
        /// </summary>
        /// <param name="storage"></param>
        void Save(IXCConfigurationStorage storage);
    }
}