using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Xamarin.Controls;

namespace ZenPlatform.UIGeneration2.XamarinForms {
    [Serializable]
    public class ImageEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        String _source;

        public String Source {
            get { return _source; }
            set {
                _source = value;
                RaisePropertyChanged();
            }
        }

        public String TemplateResourceKey { get; }

        public ImageEditorProperties() {
            this.TemplateResourceKey = "xamarinFormsImageEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new ImageFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
