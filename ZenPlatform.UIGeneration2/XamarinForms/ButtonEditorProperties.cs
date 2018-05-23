using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Xamarin.Controls;

namespace ZenPlatform.UIGeneration2.XamarinForms {
    [Serializable]
    public class ButtonEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        String _command;
        String _text;

        public String Command {
            get { return _command; }
            set {
                _command = value;
                RaisePropertyChanged();
            }
        }

        public String TemplateResourceKey { get; }

        public String Text {
            get { return _text; }
            set {
                _text = value;
                RaisePropertyChanged();
            }
        }

        public String TextText {
            get {
                if (String.IsNullOrWhiteSpace(this.Text)) {
                    return String.Empty;
                }
                return $"Text=\"{this.Text}\" ";
            }
        }

        public ButtonEditorProperties() {
            this.TemplateResourceKey = "xamarinFormsButtonEditorTemplate";
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
