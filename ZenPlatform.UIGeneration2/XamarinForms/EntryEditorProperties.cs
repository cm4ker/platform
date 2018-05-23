using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Xamarin.Controls;

namespace ZenPlatform.UIGeneration2.XamarinForms {
    [Serializable]
    public class EntryEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        Boolean _isPassword;
        String _keyboard = Default;
        String _placeholder = String.Empty;

        public Boolean IsPassword {
            get { return _isPassword; }
            set {
                _isPassword = value;
                RaisePropertyChanged();
            }
        }

        public String IsPasswordText {
            get {
                if (!this.IsPassword) {
                    return String.Empty;
                }
                return "IsPassword=\"True\" ";
            }
        }

        public String Keyboard {
            get { return _keyboard; }
            set {
                _keyboard = value;
                RaisePropertyChanged();
            }
        }

        public String KeyboardText {
            get {
                if (String.IsNullOrWhiteSpace(this.Keyboard) || this.Keyboard == Default) {
                    return String.Empty;
                }
                return $"Keyboard=\"{this.Keyboard}\" ";
            }
        }

        public String Placeholder {
            get { return _placeholder; }
            set {
                _placeholder = value;
                RaisePropertyChanged();
            }
        }

        public String PlaceholderText {
            get {
                if (String.IsNullOrWhiteSpace(this.Placeholder)) {
                    return String.Empty;
                }
                return $"Placeholder=\"{this.Placeholder}\" ";
            }
        }

        public String TemplateResourceKey { get; }

        public EntryEditorProperties() {
            this.TemplateResourceKey = "xamarinFormsEntryEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new EntryFactory(generateFormModel, propertyInformationViewModel);
        }

        const String Default = "Default";

    }
}
