using AvaloniaEdit.Document;

namespace ZenPlatform.SimpleIde.Models
{
    public interface IConfigurationDocument
    {
        bool IsChanged { get; }
        TextDocument Text { get; }

        void Save();
    }
}