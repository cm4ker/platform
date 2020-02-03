using System;
using System.Collections.Generic;
using System.IO;
using SharpFileSystem;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IConfigurationManipulator
    {
        IProject Load(IFileSystem storage);

        IProject Create(string projectName);

        Stream SaveToStream(IProject project);

        string GetHash(IProject project);

        bool Equals(IProject a, IProject b);
    }


    public interface IProject
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

        IInfrastructure Infrastructure { get; }

        List<IComponentRef> ComponentReferences { get; }

        /// <summary>
        /// Созранить объект в контексте другого хранилища
        /// </summary>
        /// <param name="storage"></param>
        void Save(IFileSystem fileSystem);


        void Attach(IComponentRef comRef, IComponentManager mrg);
    }
}