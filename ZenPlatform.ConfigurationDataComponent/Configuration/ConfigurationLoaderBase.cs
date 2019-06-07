﻿using System;
using System.IO;
using System.Xml.Serialization;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Exceptions;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.DataComponent.Configuration
{
    public abstract class ConfigurationLoaderBase<TObjectType> : IXComponentLoader
        where TObjectType : XCObjectTypeBase
    {
        #region Events

        /// <summary>
        /// Вызывается после загрузки объекта
        /// </summary>
        /// <param name="conf">Конфигурация объекта</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        protected virtual void AfterObjectLoad(TObjectType conf)
        {
        }

        /// <summary>
        /// Вызывается перед инициализацией
        /// </summary>
        /// <param name="conf">Конфигурация объекта</param>
        protected virtual void BeforeInitialize(TObjectType conf)
        {
        }

        /// <summary>
        /// Вызывается после инициализации
        /// </summary>
        /// <param name="conf">Конфигурация объекта</param>
        protected virtual void AfterInitialize(TObjectType conf)
        {
        }

        #endregion

        protected virtual XCDataRuleBase LoadRuleAction(XCDataRuleContent content)
        {
            throw new NotImplementedException();
        }

        public virtual IDataComponent GetComponentImpl(XCComponent component)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Загрузить объект компонента
        /// </summary>
        /// <param name="component"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public virtual XCObjectTypeBase LoadObject(XCComponent component, XCBlob blob)
        {
            TObjectType conf;
            using (var stream = component.Root.Storage.GetBlob(blob.Name, $"Data/{component.Info.ComponentName}"))
            {
                conf = XCHelper.DeserializeFromStream<TObjectType>(stream) ??
                       throw new InvalidLoadConfigurationException(blob.Name);
            }

            if (conf.Name is null) throw new NullReferenceException("Configuration broken fill the name");
            if (conf.Guid == Guid.Empty) throw new NullReferenceException("Configuration broken fill the id field");

            AfterObjectLoad(conf);

            //Сразу же указываем родителя
            ((IChildItem<XCComponent>) conf).Parent = component;

            component.Parent.RegisterType(conf);

            BeforeInitialize(conf);
            conf.Initialize();
            AfterInitialize(conf);

            return conf;
        }

        /// <summary>
        /// Сохранить объект. Эта функция входит в обязательный набор API для реализации компонента
        /// </summary>
        /// <param name="conf"></param>
        public virtual void SaveObject(XCObjectTypeBase conf)
        {
            var storage = conf.Parent.Root.Storage;
            var component = conf.Parent;
            XCBlob blob;
            if (conf.AttachedBlob is null)
            {
                blob = new XCBlob(conf.Name);
                conf.Parent.Include.Add(blob);
            }
            else
            {
                blob = conf.AttachedBlob;
            }

            using (var stream = conf.SerializeToStream())
                storage.SaveBlob(blob.Name, $"Data/{component.Info.ComponentName}", stream);
        }

        public XCDataRuleBase LoadRule(XCDataRuleContent content)
        {
            return LoadRuleAction(content);
        }

        public void SaveRule(XCDataRuleBase rule)
        {
        }
    }
}