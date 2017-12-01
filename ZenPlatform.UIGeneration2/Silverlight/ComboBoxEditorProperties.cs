using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Silverlight.Controls;

namespace ZenPlatform.UIGeneration2.Silverlight {
    [Serializable]
    public class ComboBoxEditorProperties : SelectorBase, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public ComboBoxEditorProperties() {
            this.TemplateResourceKey = "silverlightComboBoxEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new ComboBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
