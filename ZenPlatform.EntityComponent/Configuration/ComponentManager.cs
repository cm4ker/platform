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
using ZenPlatform.Configuration.Structure;
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

            inf.TypeManager.Register(c);

            return c;
        }

        public IComponentEditor Load(IProject proj, IComponentRef comRef, IFileSystem fs)
        {
            var com = fs.Deserialize<MDComponent>(comRef.Entry);
            var editor = new ComponentEditor(proj, com, fs);
            editor.Apply();

            return editor;
        }

        public void Save(IInfrastructure inf, IComponentRef comRef, IFileSystem fs)
        {
            var info = GetComponentInfo();
            var com = inf.TypeManager.FindComponent(info.ComponentId);

            var tm = inf.TypeManager;

            fs.Serialize(comRef.Entry, tm.Metadatas.First(x => x.Id == info.ComponentId));

            var mds = tm.Metadatas.Where(x => x.ParentId == info.ComponentId);

            foreach (var mr in mds)
            {
                var md = (MDEntity) mr.Metadata;

                fs.Serialize(FileSystemPath.Root.AppendDirectory("Entity").AppendFile(md.Name).ToString(), md);
            }
        }
    }
}