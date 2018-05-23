using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Uwp.Controls;

namespace ZenPlatform.UIGeneration2.Uwp {
    [Serializable]
    public class ComboBoxEditorProperties : SelectorBase, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public ComboBoxEditorProperties() {
            this.TemplateResourceKey = "uwpComboBoxEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new ComboBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
