namespace ZenPlatform.Configuration.Contracts.TypeSystem
{
    public interface ITypeSpec : IType
    {
        int Scale { get; set; }
        int Precision { get; set; }
        int Size { get; set; }
    }
}