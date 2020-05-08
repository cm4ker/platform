using System.Collections.Generic;


namespace Aquila.Pidl.ObjectModel
{

    public class Form
    {
        public Form()
        {
            Content = new List<Element>();
        }

        public List<Element> Content { get; set; }
    }
}
