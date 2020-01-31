using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class ObjectBuilder
    {
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
    }
}