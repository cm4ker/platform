using Microsoft.VisualBasic;

namespace ZenPlatform.UIBuilder.Interface
{
    public class UITextBox : UINode
    {
        public UITextBox()
        {
            Height = 28;
            Width = 100;
        }

        public string DataSource { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }
    }
}