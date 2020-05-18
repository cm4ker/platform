using System.Reactive.Linq;
using System.Reactive.Subjects;
using Aquila.Component.Shared.Configuration.Editors;
using Aquila.EntityComponent.IDE;
using Aquila.Ide.Common;
using Aquila.Ide.Contracts;
using AvaloniaEdit.Document;
using ReactiveUI;

namespace Aquila.Component.Shared.IDE
{
    [View(typeof(CodeEditorView))]
    public class ModuleConfigurationItem : ConfigurationItemBase
    {
        private readonly ModuleEditor _editor;
        private ObservableAsPropertyHelper<bool> _isChanged;
        private Subject<bool> _changeSubject;

        public ModuleConfigurationItem(ModuleEditor editor)
        {
            _editor = editor;
            _changeSubject = new Subject<bool>();
            Document = new TextDocument(_editor.ModuleText);

            _isChanged = Observable.Merge(
                    Document.WhenAnyValue(doc => doc.Text).Select(t => true).Skip(1), _changeSubject)
                .ToProperty(this, vm => vm.IsChanged);
        }


        public override string Caption
        {
            get => _editor.ModuleName;
            set { }
        }

        public override bool CanEdit => true;

        public override bool CanDelete => true;

        public override void Save()
        {
            _editor.ModuleText = Document.Text;
            _changeSubject.OnNext(false);
        }

        public override void DiscardChange()
        {
            Document.Text = _editor.ModuleText;
            _changeSubject.OnNext(false);
        }

        public TextDocument Document { get; set; }
        public override bool CanSave => true;
        public override bool IsChanged => _isChanged.Value;
        public string Title => Caption;
    }
}