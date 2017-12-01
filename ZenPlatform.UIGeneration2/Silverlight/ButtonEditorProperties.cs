using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Silverlight.Controls;

namespace ZenPlatform.UIGeneration2.Silverlight {
    [Serializable]
    public class ButtonEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        String _command;
        String _content;

        public String Command {
            get { return _command; }
            set {
                _command = value;
                RaisePropertyChanged();
            }
        }

        public String Content {
            get { return _content; }
            set {
                _content = value;
                RaisePropertyChanged();
            }
        }

        public String TemplateResourceKey { get; }

        public ButtonEditorProperties() {
            this.TemplateResourceKey = "silverlightButtonEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            if (generateFormModel == null) {
                throw new ArgumentNullException(nameof(generateFormModel));
            }
            if (propertyInformationViewModel == null) {
                throw new ArgumentNullException(nameof(propertyInformationViewModel));
            }
            return new ButtonFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
