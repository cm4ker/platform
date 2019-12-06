using System.Collections.Generic;

namespace UIModel.XML
{
    public class Form
    {
        public Form()
        {
            Childs = new List<object>();
        }

        public List<object> Childs { get; }
    }

    public class Button
    {
        public string OnClick { get; set; }

        public string Text { get; set; }
    }
}