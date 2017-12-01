using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Wpf.Controls;

namespace ZenPlatform.UIGeneration2.Wpf {
    [Serializable]
    public class TextBoxEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public TextBoxEditorProperties() {
            this.TemplateResourceKey = "wpfTextBoxEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new TextBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
