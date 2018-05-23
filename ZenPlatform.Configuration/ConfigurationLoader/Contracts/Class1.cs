using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Data;

namespace ZenPlatform.Configuration.ConfigurationLoader.Contracts
{

    /// <summary>
    /// Интерфейс типа который может представлять компонент
    /// </summary>
    public interface IComponentType
    {
        Guid Id { get; }

        //TODO : здесь должна быть ссылка на компонент, пока оставил PComponent
        PComponent OwnerComponent { get; }
    }

    public interface IRule
    {
        Guid ObjectId { get; }
        PComponent ComponentOwner { get; }
    }


    /// <summary>
    /// Загрузчик конфигурации.
    /// </summary>
    public interface IComponenConfigurationLoader
    {
        /// <summary>
        /// Первичная загрузка xml файла конфигурации.
        /// Это касается только лишь типов. Т.е. мы не можем загрузить тип
        /// полностью, пока не прогрузим все типы, потому что тут могут быть циклические зависимости
        /// </summary>
        /// <param name="pathToXml"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        IComponentType LoadComponentType(string pathToXml, PComponent component);

        /// <summary>
        /// Загрузить зависимости для типа
        /// Поиск компонента будет осуществляться 
        /// </summary>
        /// <param name="pathToXml"></param>
        /// <param name="supportedObjects"></param>
        /// <returns></returns>
        IComponentType LoadComponentTypeDependencies(string pathToXml, List<IComponentType> supportedObjects);

        /// <summary>
        /// Загрузить роль из контента
        /// </summary>
        /// <param name="obj">Объект, к которому прикрепляются правила</param>
        /// <param name="xmlContent">xml фрагмент, сериализованной роли в компоненте</param>
        /// <returns></returns>
        IRule LoadComponentRole(IComponentType obj, string xmlContent);
    }

}
