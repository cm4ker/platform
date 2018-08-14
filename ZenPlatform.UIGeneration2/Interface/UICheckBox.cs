namespace ZenPlatform.UIBuilder.Interface
{
    public class UICheckBox : UINode
    {
        public UICheckBox(string text = "")
        {
            Text = text;
        }

        public string Text { get; set; }
    }


    public class UIButton : UINode
    {
        public UIButton()
        {

        }
    }
}