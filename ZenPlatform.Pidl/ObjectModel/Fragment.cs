using System;
using System.Collections.Generic;

namespace ZenPlatform.Pidl.ObjectModel
{
    public class Fragment : Element
    {
        public Fragment()
        {
            Content = new List<Element>();
        }

        public Guid Id { get; set; }

        public List<Element> Content { get; set; }

        public Guid DataType { get; set; }
    }

    public class FragmentRef : Element
    {
        public Guid Id { get; set; }
    }
}