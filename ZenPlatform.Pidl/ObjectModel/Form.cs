using System.Collections.Generic;
using Portable.Xaml.Markup;

namespace ZenPlatform.Pidl.ObjectModel
{
    [ContentProperty("Content")]
    public class Form
    {
        public Form()
        {
            Content = new List<Element>();
        }

        public List<Element> Content { get; set; }
    }
}
