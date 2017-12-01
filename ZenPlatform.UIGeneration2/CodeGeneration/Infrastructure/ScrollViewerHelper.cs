using System;
using System.Text;

namespace ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure {
    public class ScrollViewerHelper {

        readonly String _elementType = "ScrollViewer";

        public ScrollViewerHelper(ProjectType projectType) {
            if (projectType == ProjectType.Xamarin) {
                _elementType = "ScrollView";
            }
        }

        public String EndTag() {
            return $"</{_elementType}>";
        }

        public String StartTag(Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder($"<{_elementType} ");
            if (parentGridColumn != null) {
                sb.Append($"Grid.Column=\"{parentGridColumn.Value}\" ");
            }
            if (parentGridRow != null) {
                sb.Append($"Grid.Row=\"{parentGridRow.Value}\" ");
            }
            sb.AppendLine(">");

            return sb.ToString();
        }

    }
}
