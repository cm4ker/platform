using ZenPlatform.Configuration.Contracts;

namespace ZenPlatform.Configuration.Structure
{
    public class ComponentRef : IComponentRef
    {
        public string Name { get; set; }
        public string Entry { get; set; }
    }
}