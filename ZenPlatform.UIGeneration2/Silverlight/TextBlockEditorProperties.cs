using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Silverlight.Controls;

namespace ZenPlatform.UIGeneration2.Silverlight {
    [Serializable]
    public class TextBlockEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public TextBlockEditorProperties() {
            this.TemplateResourceKey = "silverlightTextBlockEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new TextBlockFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
