using System.Collections.Generic;
using Portable.Xaml.Markup;

namespace ZenPlatform.Pidl.ObjectModel
{
    [ContentProperty("Content")]
    public class Group : Element
    {
        public Group()
        {
            Content = new List<Element>();
        }

        public List<Element> Content { get; set; }
    }
}
