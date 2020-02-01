using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using dnlib.DotNet;
using SharpFileSystem;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class ComponentManager : IComponentManager
    {
        private Assembly _asm;

        private Dictionary<string, object> _loadedObjects;

        public ComponentManager()
        {
            _asm = typeof(ComponentManager).Assembly;
        }

        /// <summary>
        /// Загрузить объект компонента
        /// </summary>
        /// <param name="component"></param>
        /// <param name="loader"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public virtual void LoadObject(IComponent component, IInfrastructure infrastructure, MDEntity typeMd)
        {
        }


        public IXCComponentInformation GetComponentInfo()
        {
            return new Info();
        }

        public IDataComponent GetComponentImpl(IComponent c)
        {
            var impl = new EntityComponent(c);
            impl.OnInitializing();
            return impl;
        }

        public IComponent CreateAndRegisterComponent(IInfrastructure inf, MDComponent com)
        {
            var c = inf.TypeManager.Component();

            c.ComponentImpl = GetComponentImpl(c);
            c.Info = GetComponentInfo();
            c.Metadata = com;

            inf.TypeManager.Register(c);

            return c;
        }

        public void Load(IInfrastructure inf, IComponentRef comRef, IFileSystem fs)
        {
            var com = fs.Deserialize<MDComponent>(comRef.Entry);

            var c = CreateAndRegisterComponent(inf, com);

            foreach (var mdFile in fs.GetEntities(FileSystemPath.Parse("/Entity/")))
            {
                var type = fs.Deserialize<MDEntity>(mdFile.Path);

                LoadObject(c, inf, type);
            }
        }

        public void Save(IInfrastructure inf, IComponentRef comRef, IFileSystem fs)
        {
            var info = GetComponentInfo();
            var com = inf.TypeManager.FindComponent(info.ComponentId);
            var md = (MDComponent) com.Metadata;

            foreach (var type in com.GetTypes().Where(x => x.IsObject))
            {
            }

            fs.Serialize(comRef.Entry, md);
        }
    }
}