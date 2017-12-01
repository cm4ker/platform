using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Wpf.Controls;

namespace ZenPlatform.UIGeneration2.Wpf {
    [Serializable]
    public class TextBlockEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public TextBlockEditorProperties() {
            this.TemplateResourceKey = "wpfTextBlockEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new TextBlockFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
