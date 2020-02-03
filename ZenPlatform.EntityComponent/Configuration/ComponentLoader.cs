using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.Completion;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Statements;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class ComponentLoader : IComponentManager
    {
        private Assembly _asm;

        private Dictionary<string, object> _loadedObjects;

        public ComponentLoader()
        {
            _asm = typeof(ComponentLoader).Assembly;
        }

        /// <summary>
        /// Загрузить объект компонента
        /// </summary>
        /// <param name="component"></param>
        /// <param name="loader"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public virtual void LoadObject(IComponent component, ILoader loader, MDEntity typeMd)
        {
            var tm = loader.TypeManager;

            BuildObject(component, loader.Counter, tm, typeMd);
            BuildDto(component, loader.Counter, tm, typeMd);
        }

        private void BuildObject(IComponent component, IUniqueCounter counter, ITypeManager tm, MDEntity md)
        {
            var oType = tm.Type();
            oType.IsObject = true;

            oType.Id = md.ObjectId;
            oType.Name = md.Name;
            oType.IsCodeAvaliable = true;
            oType.IsQueryAvaliable = true;

            oType.ComponentId = component.Info.ComponentId;
            oType.Metadata = md;

            oType.SystemId = counter.GetId(oType.Id);

            tm.Register(oType);

            foreach (var prop in md.Properties)
            {
                var p = tm.Property();

                p.Name = prop.Name;
                p.Metadata = prop;
            }

            foreach (var table in md.Tables)
            {
                var t = tm.Table();
                t.Name = table.Name;
                t.Id = table.Guid;

                t.SystemId = counter.GetId(t.Id);
            }
        }

        private void BuildDto(IComponent component, IUniqueCounter counter, ITypeManager tm, MDEntity md)
        {
            var oType = tm.Type();
            oType.IsDto = true;

            oType.Id = md.ObjectId;
            oType.Name = "_" + md.Name;

            oType.ComponentId = component.Info.ComponentId;
            oType.Metadata = md;

            oType.SystemId = counter.GetId(oType.Id);

            tm.Register(oType);

            foreach (var prop in md.Properties)
            {
                var p = tm.Property();

                p.Name = prop.Name;
                p.Metadata = prop;
            }

            foreach (var table in md.Tables)
            {
                var t = tm.Table();
                t.Name = table.Name;
                t.Id = table.Guid;

                t.SystemId = counter.GetId(t.Id);
            }
        }

        private void BuildLink(IComponent component, IUniqueCounter counter, ITypeManager tm, MDEntity md)
        {
            var oType = tm.Type();
            oType.IsLink = true;
            oType.IsQueryAvaliable = true;

            oType.Id = md.LinkId;
            oType.Name = md.Name + "Link";

            oType.ComponentId = component.Info.ComponentId;
            oType.Metadata = md;

            oType.SystemId = counter.GetId(oType.Id);

            tm.Register(oType);

            foreach (var prop in md.Properties)
            {
                var p = tm.Property();

                p.Name = prop.Name;
                p.Metadata = prop;
            }

            foreach (var table in md.Tables)
            {
                var t = tm.Table();
                t.Name = table.Name;
                t.Id = table.Guid;

                t.SystemId = counter.GetId(t.Id);
            }
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

        public void Load(IComponentRef comRef, ILoader loader)
        {
            var com = loader.LoadObject<MDComponent>(comRef.Entry);

            var c = loader.TypeManager.Component();

            c.ComponentImpl = GetComponentImpl(c);
            c.Info = GetComponentInfo();
            c.Metadata = com;

            loader.TypeManager.Register(c);

            foreach (var typeRef in com.EntityReferences)
            {
                var type = loader.LoadObject<MDEntity>(typeRef);

                LoadObject(c, loader, type);
            }
        }

        public void Save(IComponentRef comRef, IXCSaver saver)
        {
            var info = GetComponentInfo();
            var com = saver.TypeManager.FindComponent(info.ComponentId);
            var md = (MDComponent) com.Metadata;

            foreach (var type in com.GetTypes().Where(x => x.IsObject))
            {
                var typeMd = type.Metadata as MDEntity ??
                             throw new Exception("This type not support by this component");

                saver.SaveObject("some path", typeMd);
            }

            saver.SaveObject(comRef.Entry, md);
        }
    }
}