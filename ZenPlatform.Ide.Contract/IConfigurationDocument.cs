//using AvaloniaEdit.Document;

namespace ZenPlatform.Ide.Contracts
{
    public interface IConfigurationDocument
    {
        bool IsChanged { get; }

        string Title { get; }

        void Save();
    }
}