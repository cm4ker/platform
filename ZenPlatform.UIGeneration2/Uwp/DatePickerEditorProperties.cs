using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Uwp.Controls;

namespace ZenPlatform.UIGeneration2.Uwp {
    [Serializable]
    public class DatePickerEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        public String TemplateResourceKey { get; }

        public DatePickerEditorProperties() {
            this.TemplateResourceKey = "uwpDatePickerEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new DatePickerFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
