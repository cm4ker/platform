using ZenPlatform.Configuration.Structure;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.EntityComponent.Configuration
{
    /// <summary>
    /// Программный модуль для сущности
    /// </summary>
    public class XCSingleEntityModule : XCProgramModuleBase, IChildItem<XCSingleEntity>
    {
        private XCSingleEntity _parent;

        public XCSingleEntityModuleType ModuleType { get; set; }

        public XCSingleEntity Parent => _parent;

        XCSingleEntity IChildItem<XCSingleEntity>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }


    public enum XCSingleEntityModuleType
    {
        ObjectModule,
        ExtendModule
    }
}