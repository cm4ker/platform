using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Uwp.Controls;

namespace ZenPlatform.UIGeneration2.Uwp {
    [Serializable]
    public class SliderEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        Double _maximum;
        String _maximumPathText;
        Double _minimum;
        String _minimumPathText;
        Double _tickFrequency;
        String _tickFrequencyPathText;
        UwpTickPlacement _uwpTickPlacement = UwpTickPlacement.None;

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

        public Double TickFrequency {
            get { return _tickFrequency; }
            set {
                _tickFrequency = value;
                RaisePropertyChanged();
            }
        }

        public String TickFrequencyPathText {
            get { return _tickFrequencyPathText; }
            set {
                _tickFrequencyPathText = value;
                RaisePropertyChanged();
            }
        }

        public UwpTickPlacement UwpTickPlacement {
            get { return _uwpTickPlacement; }
            set {
                _uwpTickPlacement = value;
                RaisePropertyChanged();
            }
        }

        public SliderEditorProperties() {
            this.TemplateResourceKey = "uwpSliderEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new SliderFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
