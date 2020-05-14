namespace Aquila.Compiler.Contracts
{
    public interface IFileSource
    {
        string FilePath { get; }
        byte[] FileContents { get; }
    }
}