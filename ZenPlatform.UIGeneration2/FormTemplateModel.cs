using System;
using System.Collections.Generic;
using ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure;

namespace ZenPlatform.UIGeneration2 {
    public class FormTemplateModel {

        readonly GenerateFormModel _generateFormModel;

        public Boolean AddNameToUIControls => _generateFormModel.AddNameToUIControls;

        public BorderHelper BorderHelper { get; }

        public String ColumnRootControlEndTag { get; private set; } = String.Empty;

        public RootObject ColumnRootObject => _generateFormModel.ColumnRootObject;

        public CreateObjectDefinition CreateObjectDefinition => _generateFormModel.CreateObjectDefinition;

        public IList<IList<FormComponentModel>> FormComponentModelCollection { get; }

        public String FormHeader {
            get {
                if (String.IsNullOrWhiteSpace(_generateFormModel.FormHeader)) {
                    return String.Empty;
                }

                return TextBlockHelper.MakeTag(_generateFormModel.FormHeader);
            }
        }

        public Boolean GenerateRootObject => this.RootObject != RootObject.None;

        public GridHelper GridHelper { get; }

        public Boolean IncludeFormHeader => !String.IsNullOrWhiteSpace(_generateFormModel.FormHeader);

        public LabelHelper LabelHelper { get; }

        public LabelPosition LabelPosition => _generateFormModel.LabelPosition;

        public ProjectType ProjectType => _generateFormModel.ProjectType;

        public String RootControlEndTag { get; private set; } = String.Empty;

        public String RootControlStartTag { get; private set; } = String.Empty;

        public Int32 RootGridColumnCount => _generateFormModel.ColumnLayouts.Count;

        public IList<GridLength> RootGridColumns { get; } = new List<GridLength>();

        public IList<GridLength> RootGridRows { get; } = new List<GridLength>();

        public RootObject RootObject { get; }

        public ScrollViewerHelper ScrollViewerHelper { get; }

        public StackPanelHelper StackPanelHelper { get; }

        public TableRootHelper TableRootHelper { get; }

        public TableSectionHelper TableSectionHelper { get; }

        public TableViewHelper TableViewHelper { get; }

        public TextBlockHelper TextBlockHelper { get; }

        public ViewCellHelper ViewCellHelper { get; }

        public Boolean WrapGeneratedCodeInBorder => _generateFormModel.WrapGeneratedCodeInBorder && this.GenerateRootObject;

        public FormTemplateModel(GenerateFormModel generateFormModel) {
            if (generateFormModel == null) {
                throw new ArgumentNullException(nameof(generateFormModel));
            }
            _generateFormModel = generateFormModel;
            this.RootObject = generateFormModel.RootObject;
            this.FormComponentModelCollection = GetFormComponentModelCollection();
            this.BorderHelper = new BorderHelper(generateFormModel.ProjectType);
            this.GridHelper = new GridHelper();
            this.LabelHelper = new LabelHelper(generateFormModel.ProjectType);
            this.ScrollViewerHelper = new ScrollViewerHelper(generateFormModel.ProjectType);
            this.StackPanelHelper = new StackPanelHelper(generateFormModel.ProjectType);
            this.TextBlockHelper = new TextBlockHelper(generateFormModel.ProjectType);
            this.TableViewHelper = new TableViewHelper();
            this.TableRootHelper = new TableRootHelper();
            this.TableSectionHelper = new TableSectionHelper();
            this.ViewCellHelper = new ViewCellHelper();

            SetRootGridRows();
            SetRootGridColumns();
            MakeRootControl();
            InitializeParentControlWithControlsIncludedOnSameRow();
        }

        public String MakeColumnRootControlStartTag(Int32 columnIndex) {
            Int32? parentGridColumn = null;
            Int32? parentGridRow = null;
            if (this.RootObject == RootObject.Grid) {
                parentGridColumn = columnIndex;
                parentGridRow = this.RootGridRows.Count - 1;
            }

            switch (this.ColumnRootObject) {
                case RootObject.None:
                    return String.Empty;
                case RootObject.Grid:
                    this.ColumnRootControlEndTag = this.GridHelper.EndTag();
                    return this.GridHelper.StartTag(CalculateGridColumnsForColumn(), CalculateGridRowsForColumn(columnIndex), parentGridColumn, parentGridRow);
                case RootObject.ScrollView:
                    this.ColumnRootControlEndTag = this.ScrollViewerHelper.EndTag();
                    return this.ScrollViewerHelper.StartTag(parentGridColumn, parentGridRow);
                case RootObject.StackLayout:
                    this.ColumnRootControlEndTag = this.StackPanelHelper.EndTag();
                    return this.StackPanelHelper.StartTag(Orientation.Vertical, parentGridColumn, parentGridRow);
                case RootObject.TableView:
                    this.ColumnRootControlEndTag = this.TableViewHelper.EndTag();
                    return this.TableViewHelper.StartTag(parentGridColumn, parentGridRow);
            }
            return String.Empty;
        }

        IList<GridLength> CalculateGridColumnsForColumn() {
            var list = new List<GridLength>();
            if (this.LabelPosition == LabelPosition.Left) {
                list.Add(new GridLength(85));
                list.Add(new GridLength(1, GridUnitType.Star));
            } else {
                list.Add(new GridLength(1, GridUnitType.Star));
            }

            return list;
        }

        IList<GridLength> CalculateGridRowsForColumn(Int32 columnIndex) {
            var numberOfRows = 0;
            var isBuildingRow = false;
            foreach (var model in this.FormComponentModelCollection[columnIndex]) {
                if (isBuildingRow) {
                    isBuildingRow = model.IncludeNextControlInRow;
                    continue;
                }
                isBuildingRow = model.IncludeNextControlInRow;
                numberOfRows += 1;

                if (model.ShowLabel && (this.LabelPosition == LabelPosition.Top || this.LabelPosition == LabelPosition.Bottom)) {
                    numberOfRows += 1;
                }
            }

            var list = new List<GridLength>();
            for (var i = 0; i < numberOfRows; i++) {
                list.Add(new GridLength(0, GridUnitType.Auto));
            }

            return list;
        }

        IList<IList<FormComponentModel>> GetFormComponentModelCollection() {
            var list = new List<IList<FormComponentModel>>();

            foreach (var columnLayout in _generateFormModel.ColumnLayouts) {
                var modelList = new List<FormComponentModel>();

                foreach (var viewModel in columnLayout) {
                    modelList.Add(new FormComponentModel(_generateFormModel, viewModel));
                }

                list.Add(modelList);
            }

            return list;
        }

        void InitializeParentControlWithControlsIncludedOnSameRow() {
            foreach (var columnCollection in this.FormComponentModelCollection) {
                FormComponentModel parent = null;
                foreach (var model in columnCollection) {
                    if (parent != null) {
                        model.RenderOnSharedRow = true;
                        parent.SameRowFormComponentModels.Add(model);
                        GridLength gridLength;

                        //if (this.LabelPosition == LabelPosition.Left && model.ShowLabel) {
                        if (model.ShowLabel) {
                            gridLength = model.LabelWidth.HasValue ? new GridLength(model.LabelWidth.Value) : new GridLength(0, GridUnitType.Auto);
                            parent.SameRowFormComponentModelsColumns.Add(gridLength);
                        }
                        if (model.CellWidthGridLength.HasValue) {
                            gridLength = model.CellWidthGridLength.Value;
                        } else {
                            gridLength = new GridLength(0, GridUnitType.Auto);
                        }
                        parent.SameRowFormComponentModelsColumns.Add(gridLength);
                        if (!model.IncludeNextControlInRow) {
                            parent = null;
                        }
                        continue;
                    }
                    if (model.IncludeNextControlInRow) {
                        parent = model;
                    }
                }
            }
        }

        void MakeRootControl() {
            if ((this.ProjectType == ProjectType.Wpf || this.ProjectType == ProjectType.Uwp) && 
                this.FormComponentModelCollection.Count == 1 && 
                this.RootObject == RootObject.Grid && 
                !this.WrapGeneratedCodeInBorder && 
                String.IsNullOrWhiteSpace(this.FormHeader)) {
                return;
            }

            switch (this.RootObject) {
                case RootObject.None:
                    break;
                case RootObject.Grid:
                    this.RootControlStartTag = this.GridHelper.StartTag(this.RootGridColumns, this.RootGridRows);
                    this.RootControlEndTag = this.GridHelper.EndTag();
                    break;
                case RootObject.ScrollView:
                    this.RootControlStartTag = this.ScrollViewerHelper.StartTag();
                    this.RootControlEndTag = this.ScrollViewerHelper.EndTag();
                    break;
                case RootObject.StackLayout:
                    this.RootControlStartTag = this.StackPanelHelper.StartTag(Orientation.Vertical);
                    this.RootControlEndTag = this.StackPanelHelper.EndTag();
                    break;
            }
        }

        void SetRootGridColumns() {
            if (_generateFormModel.RootObject == RootObject.Grid) {
                for (var i = 0; i < _generateFormModel.ColumnLayouts.Count; i++) {
                    this.RootGridColumns.Add(new GridLength(1, GridUnitType.Star));
                }
            }
        }

        void SetRootGridRows() {
            if (_generateFormModel.RootObject == RootObject.Grid) {
                if (this.IncludeFormHeader) {
                    this.RootGridRows.Add(new GridLength(0, GridUnitType.Auto));
                    this.RootGridRows.Add(new GridLength(1, GridUnitType.Star));
                } else {
                    this.RootGridRows.Add(new GridLength(1, GridUnitType.Star));
                }
            }
        }

    }
}
