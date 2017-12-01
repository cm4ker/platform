using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Silverlight.Controls;

namespace ZenPlatform.UIGeneration2.Silverlight {
    [Serializable]
    public class TextBoxEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public TextBoxEditorProperties() {
            this.TemplateResourceKey = "silverlightTextBoxEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new TextBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
