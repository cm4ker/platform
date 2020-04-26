namespace ZenPlatform.Compiler.Contracts
{
    public interface IWellKnownTypes
    {
        IType Int { get; }

        IType String { get; }

        IType Object { get; }

        IType Char { get; }

        IType Boolean { get; }

        IType Double { get; }

        IType Guid { get; }

        IType Void { get; }

        IType Byte { get; }

        IType DateTime { get; }
    }
}