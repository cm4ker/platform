using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ZenPlatform.UIGeneration2.CodeGeneration
{
    public partial class UIFormTemplate : UIFormTemplateBase
    {

        public FormTemplateModel Model { get; }

        public UIFormTemplate(GenerateFormModel generateFormModel)
        {
            if (generateFormModel == null)
            {
                throw new ArgumentNullException(nameof(generateFormModel));
            }

            this.Model = new FormTemplateModel(generateFormModel);
        }


        public string TransformText()
        {
            if (this.Model.WrapGeneratedCodeInBorder)
            {
                WriteLine(this.Model.BorderHelper.StartTag());
            }

            WriteLine(this.Model.RootControlStartTag);

            WriteLine(this.Model.FormHeader);

            var index = 0;
            foreach (var columnSet in this.Model.FormComponentModelCollection)
            {
                WriteLine(this.Model.MakeColumnRootControlStartTag(index));

                if (this.Model.ColumnRootObject == RootObject.TableView)
                {
                    WriteLine(this.Model.TableRootHelper.StartTag());
                }

                Int32? currentRootRow = null;
                if (this.Model.ColumnRootObject == RootObject.Grid)
                {
                    currentRootRow = 0;
                }

                var lastTableSectionTitle = String.Empty;

                foreach (var formComponentModel in columnSet)
                {

                    // skip items that will be rendered on same row with another control
                    if (formComponentModel.RenderOnSharedRow)
                    {
                        continue;
                    }

                    if (this.Model.ColumnRootObject == RootObject.TableView)
                    {
                        // start a new table section
                        if (lastTableSectionTitle != formComponentModel.TableSectionTitle)
                        {
                            // if not the first table section, write closing table section tag
                            if (!String.IsNullOrWhiteSpace(lastTableSectionTitle))
                            {
                                WriteLine(this.Model.TableSectionHelper.EndTag());
                            }

                            WriteLine(this.Model.TableSectionHelper.StartTag(formComponentModel.TableSectionTitle));

                            lastTableSectionTitle = formComponentModel.TableSectionTitle;
                        }
                    }

                    // when requested write label above control
                    if (this.Model.LabelPosition == LabelPosition.Top && formComponentModel.ShowLabel)
                    {
                        WriteLine(this.Model.LabelHelper.MakeTag(formComponentModel.LabelText, formComponentModel.LabelImageName, formComponentModel.LabelWidthText, null, currentRootRow));
                        if (this.Model.ColumnRootObject == RootObject.Grid)
                        {
                            currentRootRow += 1;
                        }
                    }

                    // column root = stack layout && control layout is grid
                    if (formComponentModel.ColumnRootObject == RootObject.StackLayout && formComponentModel.ControlLayoutRoot == RootObject.Grid)
                    {
                        var currentColumn = 0;
                        var currentInnerGridColumn = 0;
                        var componentColumns = new List<GridLength>();

                        if (!formComponentModel.IncludeNextControlInRow || this.Model.LabelPosition == LabelPosition.Left)
                        {
                            componentColumns.Add(new GridLength(85));
                        }
                        if (formComponentModel.IncludeNextControlInRow && formComponentModel.CellWidthGridLength.HasValue)
                        {
                            componentColumns.Add(formComponentModel.CellWidthGridLength.Value);
                        }
                        else
                        {
                            componentColumns.Add(new GridLength(1, GridUnitType.Star));
                        }

                        componentColumns.AddRange(formComponentModel.SameRowFormComponentModelsColumns);

                        WriteLine(this.Model.GridHelper.StartTag(componentColumns, null, currentColumn, currentRootRow));
                        if (this.Model.ColumnRootObject == RootObject.Grid)
                        {
                            currentRootRow += 1;
                        }

                        if (this.Model.LabelPosition == LabelPosition.Left && formComponentModel.ShowLabel)
                        {
                            WriteLine(this.Model.LabelHelper.MakeTag(formComponentModel.LabelText, formComponentModel.LabelImageName, formComponentModel.LabelWidthText, 0, 0));
                            currentInnerGridColumn += 1;
                        }

                        WriteLine(formComponentModel.ControlFactory.MakeControl(currentInnerGridColumn));
                        currentInnerGridColumn += 1;

                        foreach (var item in formComponentModel.SameRowFormComponentModels)
                        {
                            if (item.ShowLabel)
                            {
                                WriteLine(this.Model.LabelHelper.MakeTag(item.LabelText, item.LabelImageName, item.LabelWidthText, currentInnerGridColumn, null));
                                currentInnerGridColumn += 1;
                            }

                            WriteLine(item.ControlFactory.MakeControl(currentInnerGridColumn));
                            currentInnerGridColumn += 1;
                        }

                        WriteLine(this.Model.GridHelper.EndTag());

                        // column root = grid layout && control layout is grid
                    }
                    else if (formComponentModel.ColumnRootObject == RootObject.Grid && formComponentModel.ControlLayoutRoot == RootObject.Grid)
                    {

                        var currentColumn = 0;
                        var currentInnerGridColumn = 0;
                        var componentColumns = new List<GridLength>();

                        if (!formComponentModel.IncludeNextControlInRow)
                        {
                            componentColumns.Add(new GridLength(85));
                        }
                        if (formComponentModel.IncludeNextControlInRow && formComponentModel.CellWidthGridLength.HasValue)
                        {
                            componentColumns.Add(formComponentModel.CellWidthGridLength.Value);
                        }
                        else
                        {
                            componentColumns.Add(new GridLength(1, GridUnitType.Star));
                        }

                        componentColumns.AddRange(formComponentModel.SameRowFormComponentModelsColumns);

                        if (this.Model.LabelPosition == LabelPosition.Left && formComponentModel.ShowLabel && formComponentModel.IncludeNextControlInRow)
                        {
                            WriteLine(this.Model.LabelHelper.MakeTag(formComponentModel.LabelText, formComponentModel.LabelImageName, formComponentModel.LabelWidthText, 0, currentRootRow));
                            currentColumn += 1;
                        }

                        WriteLine(this.Model.GridHelper.StartTag(componentColumns, null, currentColumn, currentRootRow));
                        if (this.Model.ColumnRootObject == RootObject.Grid)
                        {
                            currentRootRow += 1;
                        }

                        if (this.Model.LabelPosition == LabelPosition.Left && formComponentModel.ShowLabel && !formComponentModel.IncludeNextControlInRow)
                        {
                            WriteLine(this.Model.LabelHelper.MakeTag(formComponentModel.LabelText, formComponentModel.LabelImageName, formComponentModel.LabelWidthText, 0, 0));
                            currentInnerGridColumn += 1;
                        }

                        WriteLine(formComponentModel.ControlFactory.MakeControl(currentInnerGridColumn));
                        currentInnerGridColumn += 1;

                        foreach (var item in formComponentModel.SameRowFormComponentModels)
                        {
                            if (item.ShowLabel)
                            {
                                WriteLine(this.Model.LabelHelper.MakeTag(item.LabelText, item.LabelImageName, item.LabelWidthText, currentInnerGridColumn, null));
                                currentInnerGridColumn += 1;
                            }

                            WriteLine(item.ControlFactory.MakeControl(currentInnerGridColumn));
                            currentInnerGridColumn += 1;
                        }

                        WriteLine(this.Model.GridHelper.EndTag());

                        // column root = table view && control layout is grid
                    }
                    else if (formComponentModel.ColumnRootObject == RootObject.TableView && formComponentModel.ControlLayoutRoot == RootObject.Grid)
                    {
                        WriteLine(this.Model.ViewCellHelper.StartTag());

                        var currentColumn = 0;
                        var currentInnerGridColumn = 0;
                        var componentColumns = new List<GridLength>();

                        //if (!formComponentModel.IncludeNextControlInRow) {
                        //	componentColumns.Add(new GridLength(85));
                        //}

                        componentColumns.Add(new GridLength(85));

                        if (formComponentModel.IncludeNextControlInRow && formComponentModel.CellWidthGridLength.HasValue)
                        {
                            componentColumns.Add(formComponentModel.CellWidthGridLength.Value);
                        }
                        else
                        {
                            componentColumns.Add(new GridLength(1, GridUnitType.Star));
                        }

                        componentColumns.AddRange(formComponentModel.SameRowFormComponentModelsColumns);

                        WriteLine(this.Model.GridHelper.StartTag(componentColumns, null, currentColumn, currentRootRow));
                        if (this.Model.ColumnRootObject == RootObject.Grid)
                        {
                            currentRootRow += 1;
                        }

                        if (this.Model.LabelPosition == LabelPosition.Left && formComponentModel.ShowLabel)
                        {
                            WriteLine(this.Model.LabelHelper.MakeTag(formComponentModel.LabelText, formComponentModel.LabelImageName, formComponentModel.LabelWidthText, 0, 0));
                            currentInnerGridColumn += 1;
                        }

                        WriteLine(formComponentModel.ControlFactory.MakeControl(currentInnerGridColumn));
                        currentInnerGridColumn += 1;

                        foreach (var item in formComponentModel.SameRowFormComponentModels)
                        {
                            if (item.ShowLabel)
                            {
                                WriteLine(this.Model.LabelHelper.MakeTag(item.LabelText, item.LabelImageName, item.LabelWidthText, currentInnerGridColumn, null));
                                currentInnerGridColumn += 1;
                            }

                            WriteLine(item.ControlFactory.MakeControl(currentInnerGridColumn));
                            currentInnerGridColumn += 1;
                        }

                        WriteLine(this.Model.GridHelper.EndTag());

                        WriteLine(this.Model.ViewCellHelper.EndTag());

                        // render control with out row wrapping control, 
                    }
                    else
                    {
                        if (this.Model.LabelPosition == LabelPosition.Left && formComponentModel.ShowLabel)
                        {
                            WriteLine(this.Model.LabelHelper.MakeTag(formComponentModel.LabelText, String.Empty, formComponentModel.LabelWidthText, null, currentRootRow));
                            WriteLine(formComponentModel.ControlFactory.MakeControl(1, currentRootRow));
                        }
                        else
                        {
                            WriteLine(formComponentModel.ControlFactory.MakeControl(null, currentRootRow));
                        }

                        if (this.Model.ColumnRootObject == RootObject.Grid)
                        {
                            currentRootRow += 1;
                        }
                    }

                    // when requested write label below control
                    if (this.Model.LabelPosition == LabelPosition.Bottom && formComponentModel.ShowLabel)
                    {
                        WriteLine(this.Model.LabelHelper.MakeTag(formComponentModel.LabelText, String.Empty, formComponentModel.LabelWidthText, null, currentRootRow));
                        if (this.Model.ColumnRootObject == RootObject.Grid)
                        {
                            currentRootRow += 1;
                        }
                    }
                }

                // close last table section and the table root
                if (this.Model.ColumnRootObject == RootObject.TableView)
                {
                    WriteLine(this.Model.TableSectionHelper.EndTag());

                    WriteLine(this.Model.TableRootHelper.EndTag());
                }

                WriteLine(this.Model.ColumnRootControlEndTag);
                index += 1;
            }

            WriteLine(this.Model.RootControlEndTag);

            if (this.Model.WrapGeneratedCodeInBorder)
            {
                WriteLine(this.Model.BorderHelper.EndTag());
            }

            return this.GenerationEnvironment.ToString();
        }
    }
    public class UIFormTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }

        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0)
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }

        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private IFormatProvider formatProviderField = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                Type t = objectToConvert.GetType();
                MethodInfo method = t.GetMethod("ToString", new Type[] {
                            typeof(IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
}
