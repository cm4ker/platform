namespace Aquila.Core.Contracts.TypeSystem
{
    public interface IPParameter : IPUniqueObject
    {
        public string Name { get; set; }

        public IPType Type { get; }
    }
}