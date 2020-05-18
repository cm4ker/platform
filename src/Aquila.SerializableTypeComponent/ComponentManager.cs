using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.Configuration.Store;
using Aquila.Core.Contracts.Data;
using Aquila.Configuration;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.SerializableTypeComponent.Configuration;
using Aquila.SerializableTypeComponent.Configuration.Editors;
using SharpFileSystem;
using IComponent = Aquila.Core.Contracts.TypeSystem.IComponent;

namespace Aquila.SerializableTypeComponent
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
        public virtual void LoadObject(IComponent component, IInfrastructure infrastructure, MDSerializableType typeMd)
        {
        }


        public IXCComponentInformation GetComponentInfo()
        {
            return new Info();
        }

        public object GetComponentImpl(IComponent c)
        {
            var impl = new SerializableTypeComponent(c);
            impl.OnInitializing();
            return impl;
        }

        public IComponent CreateAndRegisterComponent(IInfrastructure inf, MDComponent com)
        {
            var c = inf.TypeManager.Component();

            c.ComponentImpl = GetComponentImpl(c);
            c.Info = GetComponentInfo();

            inf.TypeManager.Register(c);

            inf.TypeManager.AddMD(c.Id, Guid.Empty, com);

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

            if (!fs.Exists(FileSystemPath.Root.AppendDirectory("SerializableType")))
                fs.CreateDirectory(FileSystemPath.Root.AppendDirectory("SerializableType"));

            foreach (var mr in mds)
            {
                var md = (MDSerializableType) mr.Metadata;

                fs.Serialize(FileSystemPath.Root.AppendDirectory("SerializableType").AppendFile(md.Name).ToString(), md);
            }
        }
    }
}