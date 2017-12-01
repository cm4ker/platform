using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Xamarin.Controls;

namespace ZenPlatform.UIGeneration2.XamarinForms {
    [Serializable]
    public class SwitchEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public SwitchEditorProperties() {
            this.TemplateResourceKey = "xamarinFormsSwitchEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new SwitcherFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
