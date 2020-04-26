namespace ZenPlatform.UI.Ast
{
    public class UILabel : UINode
    {
        public UILabel(string text = "")
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}