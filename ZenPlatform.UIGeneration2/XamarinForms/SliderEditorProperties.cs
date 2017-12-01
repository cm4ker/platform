using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Xamarin.Controls;

namespace ZenPlatform.UIGeneration2.XamarinForms {
    [Serializable]
    public class SliderEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        Double _maximum;
        String _maximumPathText;
        Double _minimum;
        String _minimumPathText;

        public Double Maximum {
            get { return _maximum; }
            set {
                _maximum = value;
                RaisePropertyChanged();
            }
        }

        public String MaximumPathText {
            get { return _maximumPathText; }
            set {
                _maximumPathText = value;
                RaisePropertyChanged();
            }
        }

        public Double Minimum {
            get { return _minimum; }
            set {
                _minimum = value;
                RaisePropertyChanged();
            }
        }

        public String MinimumPathText {
            get { return _minimumPathText; }
            set {
                _minimumPathText = value;
                RaisePropertyChanged();
            }
        }

        public String TemplateResourceKey { get; }

        public SliderEditorProperties() {
            this.TemplateResourceKey = "xamarinFormsSliderEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new SliderFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
