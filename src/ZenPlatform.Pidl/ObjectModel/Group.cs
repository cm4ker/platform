using System.Collections.Generic;


namespace ZenPlatform.Pidl.ObjectModel
{
    public class Group : Element
    {
        public Group()
        {
            Content = new List<Element>();
        }

        public List<Element> Content { get; set; }
    }
}
