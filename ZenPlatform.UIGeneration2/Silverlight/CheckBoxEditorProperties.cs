using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Silverlight.Controls;

namespace ZenPlatform.UIGeneration2.Silverlight {
    [Serializable]
    public class CheckBoxEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        String _content;

        public String Content {
            get { return _content; }
            set {
                _content = value;
                RaisePropertyChanged();
            }
        }

        public String TemplateResourceKey { get; }

        public CheckBoxEditorProperties() {
            this.TemplateResourceKey = "silverlightCheckBoxEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new CheckBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
