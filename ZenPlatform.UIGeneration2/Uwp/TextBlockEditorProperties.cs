using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Uwp.Controls;

namespace ZenPlatform.UIGeneration2.Uwp {
    [Serializable]
    public class TextBlockEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public TextBlockEditorProperties() {
            this.TemplateResourceKey = "uwpTextBlockEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new TextBlockFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
