using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Uwp.Controls;

namespace ZenPlatform.UIGeneration2.Uwp {
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
            this.TemplateResourceKey = "uwpImageEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new ImageFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
