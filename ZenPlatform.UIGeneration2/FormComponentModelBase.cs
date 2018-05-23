using System;
using System.Collections.Generic;

namespace ZenPlatform.UIGeneration2 {
    public abstract class FormComponentModelBase {

        public String BindingConverter => ViewModel.BindingConverter;

        public BindingMode BindingMode => this.ViewModel.BindingMode;

        public String BindingPath => this.ViewModel.BindingPath;

        public GridLength? CellWidthGridLength => this.ViewModel.CellWidthGridLength;

        public RootObject ColumnRootObject { get; }

        public RootObject ControlLayoutRoot { get; }

        public String ControlNameText { get; } = String.Empty;

        public String HeightText { get; } = String.Empty;

        public String HorizontalAlignmentText { get; } = String.Empty;

        public Boolean IncludeNextControlInRow => this.ViewModel.IncludeNextControlInRow;

        public Boolean IsNonBindingControl => this.ViewModel.IsNonBindingControl;

        public String LabelImageName => this.ViewModel.LabelImageName;

        public String LabelText => this.ViewModel.LabelText;

        public Int32? LabelWidth => this.ViewModel.LabelWidth;

        public String LabelWidthText { get; } = String.Empty;

        public String MaximumLengthText { get; } = String.Empty;

        public String Name => this.ViewModel.Name;

        public Boolean RenderOnSharedRow { get; set; }

        public IList<FormComponentModel> SameRowFormComponentModels { get; } = new List<FormComponentModel>();

        public IList<GridLength> SameRowFormComponentModelsColumns { get; } = new List<GridLength>();

        public Boolean ShowLabel => this.ViewModel.ShowLabel;

        public String StringFormat => this.ViewModel.StringFormat;

        public String StringFormatText { get; } = String.Empty;

        public String TableSectionTitle { get; }

        public String VerticalAlignmentText { get; } = String.Empty;

        protected PropertyInformationViewModel ViewModel { get; }

        public String WidthText { get; } = String.Empty;

        protected FormComponentModelBase(GenerateFormModel generateFormModel, PropertyInformationViewModel viewModel) {
            if (generateFormModel == null) {
                throw new ArgumentNullException(nameof(generateFormModel));
            }
            if (viewModel == null) {
                throw new ArgumentNullException(nameof(viewModel));
            }

            this.ViewModel = viewModel;

            this.ColumnRootObject = generateFormModel.ColumnRootObject;

            this.ControlLayoutRoot = RootObject.None;

            if (generateFormModel.LabelPosition == LabelPosition.Left) {
                this.ControlLayoutRoot = RootObject.Grid;
            }

            if (generateFormModel.ColumnRootObject == RootObject.Grid) {
                this.ControlLayoutRoot = RootObject.None;
            }

            if (this.ViewModel.IncludeNextControlInRow) {
                this.ControlLayoutRoot = RootObject.Grid;
            }

            if (generateFormModel.ColumnRootObject == RootObject.TableView) {
                this.ControlLayoutRoot = RootObject.Grid;
            }

            // default these values to WPF & UWP & Silverlight
            var widthName = "Width";
            var heightName = "Height";
            var horizontalAlignmentName = "HorizontalAlignment";
            var verticalAlignmentName = "VerticalAlignment";

            if (generateFormModel.ProjectType == ProjectType.Xamarin) {
                widthName = "WidthRequest";
                heightName = "HeightRequest";
                horizontalAlignmentName = "HorizontalOptions";
                verticalAlignmentName = "VerticalOptions";

                if (!viewModel.Width.HasValue) {
                    switch (viewModel.ControlDefinition.ControlType) {
                        case ControlType.XamarinButton:
                            this.HorizontalAlignmentText = $"{horizontalAlignmentName}=\"Start\" ";
                            break;
                        case ControlType.XamarinImage:
                            this.HorizontalAlignmentText = $"{horizontalAlignmentName}=\"Center\" ";
                            this.VerticalAlignmentText = $"{verticalAlignmentName}=\"Center\" ";
                            break;
                        case ControlType.WpfButton:
                            this.HorizontalAlignmentText = $"{horizontalAlignmentName}=\"Left\" ";
                            this.VerticalAlignmentText = $"{verticalAlignmentName}=\"Center\" ";
                            break;
                    }
                }
            } else if (generateFormModel.ProjectType == ProjectType.Wpf || generateFormModel.ProjectType == ProjectType.Silverlight) {
                if (!viewModel.Width.HasValue) {
                    switch (viewModel.ControlDefinition.ControlType) {
                        case ControlType.WpfButton:
                        case ControlType.SilverlightButton:
                            this.HorizontalAlignmentText = $"{horizontalAlignmentName}=\"Left\" ";
                            this.VerticalAlignmentText = $"{verticalAlignmentName}=\"Center\" ";
                            break;
                        case ControlType.WpfImage:
                        case ControlType.SilverlightImage:
                            this.HorizontalAlignmentText = $"{horizontalAlignmentName}=\"Center\" ";
                            this.VerticalAlignmentText = $"{verticalAlignmentName}=\"Center\" ";
                            break;
                    }
                }
            }

            if (viewModel.Height.HasValue) {
                this.HeightText = $"{heightName}=\"{viewModel.Height.Value}\" ";
            }

            if (viewModel.Width.HasValue) {
                this.WidthText = $"{widthName}=\"{viewModel.Width.Value}\" ";
            }

            if (generateFormModel.AddNameToUIControls) {
                this.ControlNameText = $"x:Name=\"{viewModel.Name.Substring(0, 1).ToLower()}{viewModel.Name.Substring(1)}\" ";
            }

            if (viewModel.LabelWidth.HasValue) {
                this.LabelWidthText = $"{widthName}=\"{viewModel.LabelWidth.Value}\" ";
            }

            if (!String.IsNullOrWhiteSpace(viewModel.StringFormat)) {
                this.StringFormatText = $", StringFormat='{viewModel.StringFormat}'";
            }

            this.TableSectionTitle = viewModel.TableSectionTitle;
        }

    }
}
