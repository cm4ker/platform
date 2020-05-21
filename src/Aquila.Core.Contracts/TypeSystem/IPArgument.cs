namespace Aquila.Core.Contracts.TypeSystem
{
    public interface IPArgument : ITypeManagerProvider
    {
        public string Name { get; set; }

        public IPType Type { get; }
    }
}