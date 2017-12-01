using System;
using System.Security;
using System.Text;
using ZenPlatform.UIGeneration2.Infrastructure;

namespace ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure {
    public class TextBlockHelper {

        readonly ProjectType _projectType;

        public TextBlockHelper(ProjectType projectType) {
            _projectType = projectType;
        }

        public String MakeTag(String text, Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var escapedText = SecurityElement.Escape(text);
            var sb = new StringBuilder();
            if (_projectType == ProjectType.Xamarin) {
                sb.Append($"<Label Text=\"{escapedText}\" ");
            } else {
                sb.Append($"<TextBlock Text=\"{escapedText}\" ");
            }
            if (parentGridColumn != null) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }

            sb.Append("/>");
            return sb.ToString().CompactString();
        }

    }
}
