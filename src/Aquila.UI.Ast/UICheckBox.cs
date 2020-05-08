namespace Aquila.UI.Ast
{
    public class UICheckBox : UINode
    {
        public UICheckBox(string text = "")
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}