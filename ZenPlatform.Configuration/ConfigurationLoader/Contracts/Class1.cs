﻿using System;
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

    /// <summary>
    /// Загрузчик конфигурации.
    /// </summary>
    public interface IComponenConfigurationtLoader
    {
        /// <summary>
        /// Первичная загрузка xml файла конфигурации.
        /// Это касается только лишь типов. Т.е. мы не можем загрузить тип
        /// полностью, пока не прогрузим все типы, потому что тут могут быть циклические зависимости
        /// </summary>
        /// <param name="pathToXml"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        IComponentType Load(string pathToXml, PComponent component);

        /// <summary>
        /// Загрузить зависимости типа
        /// </summary>
        /// <param name="pathToXml"></param>
        /// <param name="supportedObjects"></param>
        /// <returns></returns>
        IComponentType LoadDependencies(string pathToXml, List<IComponentType> supportedObjects);
    }

}
