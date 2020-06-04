using System;
using System.Collections.Generic;
using System.IO;
using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.Configuration.Store;
using Aquila.Core.Contracts.TypeSystem;
using SharpFileSystem;

namespace Aquila.Core.Contracts
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
        TypeManager TypeManager { get; }

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