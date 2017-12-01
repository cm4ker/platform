using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Xamarin.Controls;

namespace ZenPlatform.UIGeneration2.XamarinForms {
    [Serializable]
    public class StepperEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        Double _increment;
        String _incrementPathText;
        Double _maximum;
        String _maximumPathText;
        Double _minimum;
        String _minimumPathText;

        public Double Increment {
            get { return _increment; }
            set {
                _increment = value;
                RaisePropertyChanged();
            }
        }

        public String IncrementPathText {
            get { return _incrementPathText; }
            set {
                _incrementPathText = value;
                RaisePropertyChanged();
            }
        }

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

        public StepperEditorProperties() {
            this.TemplateResourceKey = "xamarinFormsStepperEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new StepperFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
