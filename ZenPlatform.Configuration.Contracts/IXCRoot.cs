using System;
using System.IO;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IConfigurationManipulator
    {
        IXCRoot Load(IXCConfigurationStorage storage);

        IXCRoot Create(string projectName);

        Stream SaveToStream(IXCRoot root);

        string GetHash(IXCRoot root);

        bool Equals(IXCRoot a, IXCRoot b);
    }


    public interface IXCRoot
    {
        IUniqueCounter Counter { get; }

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

        /// <summary>
        /// Сравнивает две конфигурации
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        object CompareConfiguration(IXCRoot another);
    }
}