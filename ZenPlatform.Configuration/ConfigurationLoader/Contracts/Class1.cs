using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration.Data.Types.Complex;

//using ZenPlatform.Configuration.Data;

namespace ZenPlatform.Configuration.ConfigurationLoader.Contracts
{
    /// <summary>
    /// Интерфейс типа который может представлять компонент
    /// </summary>
    public interface IComponentType
    {
        Guid Id { get; }

        //TODO : здесь должна быть ссылка на компонент, пока оставил PComponent
        //PComponent OwnerComponent { get; }
    }

    public interface IRule
    {
        Guid ObjectId { get; }
        // PComponent ComponentOwner { get; }
    }


    /// <summary>
    /// Загрузчик конфигурации.
    /// </summary>
    public interface IComponenConfigurationLoader
    {
        /// <summary>
        /// Загрузить тип компонента
        /// </summary>
        /// <param name="path">Путь до файла компонента</param>
        /// <returns></returns>
        XCObjectTypeBase LoadObject(string path);

        /// <summary>
        /// Загрузить правила компонента, необходимо для RLS
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        XCDataRuleBase LoadRule(XCDataRuleContent content);
    }
}