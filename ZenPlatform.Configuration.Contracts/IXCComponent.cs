using System;
using System.Collections.Generic;
using System.Reflection;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCBlob
    {
        string Name { get; set; }

        Uri URI { get; set; }

        string Hash { get; set; }
    }

    public interface IXCBlobCollection : IList<IXCBlob>
    {
    }


    public interface IXCComponent : IChildItem<IXCRoot>
    {
        /// <summary>
        /// Информация о компоненте
        /// </summary>
        IXCComponentInformation Info { get; }

        bool IsLoaded { get; }


        /// <summary>
        /// Включенные файлы в компонент. Эти файлы будут загружены строго после загрузки компонента
        /// </summary>
        // IXCBlobCollection Include { get; set; }

        Assembly ComponentAssembly { get; set; }

        IXComponentLoader Loader { get; }

        IDataComponent ComponentImpl { get; }

        /// <summary>
        /// Зарегистрировать правило для генерации кода
        /// Это действие иммутабельно. В последствии нельзя отменить регистрацию.
        /// Нельзя создавать два одинаковых правила генерации кода с одним типом. Это приведёт к ошибке
        /// </summary>
        /// <param name="rule"></param>
        void RegisterCodeRule(CodeGenRule rule);

        /// <summary>
        ///  Получить правило генерации кода по его типу.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        CodeGenRule GetCodeRule(CodeGenRuleType type);

        string GetCodeRuleExpression(CodeGenRuleType type);


        IXCObjectType GetTypeByName(string typeName);


    }
}