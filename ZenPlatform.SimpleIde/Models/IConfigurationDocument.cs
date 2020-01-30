using AvaloniaEdit.Document;

namespace ZenPlatform.SimpleIde.Models
{
    public interface IConfigurationDocument
    {
        bool IsChanged { get; }

        string Title { get; }
        TextDocument Content { get; }

        void Save();
    }
}