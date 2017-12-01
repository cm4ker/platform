using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Xamarin.Controls;

namespace ZenPlatform.UIGeneration2.XamarinForms {
    [Serializable]
    public class TimePickerEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public TimePickerEditorProperties() {
            this.TemplateResourceKey = "xamarinFormsTimePickerEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new TimePickerFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
