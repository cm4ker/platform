using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Wpf.Controls;

namespace ZenPlatform.UIGeneration2.Wpf {
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
            this.TemplateResourceKey = "wpfImageEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new ImageFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
