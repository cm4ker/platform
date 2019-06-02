namespace ZenPlatform.Compiler.Contracts
{
    public interface IFileSource
    {
        string FilePath { get; }
        byte[] FileContents { get; }
    }
}