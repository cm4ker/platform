namespace Aquila.Core.Contracts.TypeSystem
{
    public interface IPMethod: IPMember
    {
        public IPType ReturnType { get; }
    }

    public interface IPConstructor : IPMember
    {
    }
}