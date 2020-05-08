//using AvaloniaEdit.Document;

namespace Aquila.Ide.Contracts
{
    public interface IConfigurationDocument
    {
        bool IsChanged { get; }

        string Title { get; }

        void Save();
    }
}