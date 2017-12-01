using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Wpf.Controls;

namespace ZenPlatform.UIGeneration2.Wpf {
    [Serializable]
    public class ComboBoxEditorProperties : SelectorBase, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public ComboBoxEditorProperties() {
            this.TemplateResourceKey = "wpfComboBoxEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new ComboBoxFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
