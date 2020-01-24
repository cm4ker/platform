using System;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class ComponentLoader : IComponentLoader
    {
        /// <summary>
        /// Загрузить объект компонента
        /// </summary>
        /// <param name="component"></param>
        /// <param name="loader"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public virtual void LoadObject(IComponent component, ILoader loader, string reference)
        {
            uint sysId = 0;

            var md = loader.LoadObject<MDEntity, MDEntity>(reference);

            var tm = loader.TypeManager;
            var oType = tm.Type();
            oType.IsObject = true;

            oType.Id = md.ObjectId;
            oType.Name = md.Name;
            oType.IsCodeAccess = true;

            oType.ComponentId = component.Info.ComponentId;
            oType.Metadata = md;

            oType.SystemId = loader.Counter.GetId(oType.Id);

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

                t.SystemId = loader.Counter.GetId(t.Id);
            }
        }

        public IDataComponent GetComponentImpl(IComponent c)
        {
            return new EntityComponent(c);
        }
    }
}