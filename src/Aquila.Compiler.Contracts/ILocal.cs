namespace Aquila.Compiler.Contracts
{
    public interface ILocal
    {
        int Index { get; }

        IType Type { get; }
    }
}