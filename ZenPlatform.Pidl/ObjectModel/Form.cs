using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Portable.Xaml.Markup;
using ZenPlatform.Pidl.ObjectModel;

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
