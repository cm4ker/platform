namespace Aquila.UI.Ast
{
    public class UIButton : UINode
    {
        public UIButton(string text = "")
        {
            Text = text;
        }

        public string Text { get; set; }

        public string OnClick { get; set; }
    }
}