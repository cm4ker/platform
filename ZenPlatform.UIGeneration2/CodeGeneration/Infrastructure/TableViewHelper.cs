using System;
using System.Text;

namespace ZenPlatform.UIGeneration2.CodeGeneration.Infrastructure {
    public class TableViewHelper {

        public TableViewHelper() {
        }

        public String EndTag() {
            return "</TableView>";
        }

        public String StartTag(Int32? parentGridColumn = null, Int32? parentGridRow = null) {
            var sb = new StringBuilder("<TableView  Intent=\"Form\" HasUnevenRows=\"True\" ");
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
