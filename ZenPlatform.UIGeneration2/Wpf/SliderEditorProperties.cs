using System;
using ZenPlatform.UIGeneration2.CodeGeneration.Wpf.Controls;

namespace ZenPlatform.UIGeneration2.Wpf {
    [Serializable]
    public class SliderEditorProperties : ObservableObject, IEditEditor, IConstructControlFactory {

        Double _maximum;
        String _maximumPathText;
        Double _minimum;
        String _minimumPathText;
        Double _tickFrequency;
        String _tickFrequencyPathText;
        WpfTickPlacement _wpfTickPlacement = WpfTickPlacement.None;

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

        public WpfTickPlacement WpfTickPlacement {
            get { return _wpfTickPlacement; }
            set {
                _wpfTickPlacement = value;
                RaisePropertyChanged();
            }
        }

        public SliderEditorProperties() {
            this.TemplateResourceKey = "wpfSliderEditorTemplate";
        }

        public IControlFactory Make(GenerateFormModel generateFormModel, PropertyInformationViewModel propertyInformationViewModel) {
            return new SliderFactory(generateFormModel, propertyInformationViewModel);
        }

    }
}
