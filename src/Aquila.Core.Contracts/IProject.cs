using System;
using System.Collections.Generic;
using System.IO;
using SharpFileSystem;
using Aquila.Configuration.Contracts.Store;
using Aquila.Configuration.Contracts.TypeSystem;
using Aquila.Configuration.Structure;
using Aquila.Shared.ParenChildCollection;

namespace Aquila.Configuration.Contracts
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

        /// <summary>
        /// Регистрирует редактор компанента
        /// </summary>
        /// <param name="editor"></param>
        void RegisterComponentEditor(IComponentEditor editor);
    }
}