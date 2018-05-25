using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    [Serializable]
    public class XmlConfData : IChildItem<XmlConfRoot>
    {
        private XmlConfRoot _parent;

        public XmlConfData()
        {
            IncludedFiles = new List<XmlConfFile>();
            Components = new ChildItemCollection<XmlConfData, XmlConfComponent>(this);
        }

        [XmlArray("IncludedFiles")]
        [XmlArrayItem(ElementName = "File", Type = typeof(XmlConfFile))]
        public List<XmlConfFile> IncludedFiles { get; set; }

        [XmlArray("Components")]
        [XmlArrayItem(ElementName = "Component", Type = typeof(XmlConfComponent))]
        public ChildItemCollection<XmlConfData, XmlConfComponent> Components { get; set; }


        public void LoadComponents()
        {
            foreach (var component in Components.Where(x => !x.IsLoaded))
            {
                component.LoadComponent();
            }
        }


        [XmlIgnore]
        public XmlConfRoot Parent => _parent;


        XmlConfRoot IChildItem<XmlConfRoot>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}