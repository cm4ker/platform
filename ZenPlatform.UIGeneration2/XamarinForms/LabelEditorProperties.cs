using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Xamarin.Controls;

namespace ZenPlatform.UIGeneration2.XamarinForms {
    [Serializable]
    public class LabelEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        String _labelText;

        /// <summary>
        /// Gets or sets the label text. This property is used for UNBOUND Label controls only.
        /// </summary>
        /// <value>The label text.</value>
        public String LabelText {
            get { return _labelText; }
            set {
                _labelText = value;
                RaisePropertyChanged();
            }
        }

        public String TemplateResourceKey { get; }

        public LabelEditorProperties() {
            this.TemplateResourceKey = "xamarinFormsLabelEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new LabelFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
