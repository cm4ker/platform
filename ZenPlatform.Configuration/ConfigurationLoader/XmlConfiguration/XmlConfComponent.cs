using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    public class XmlConfComponent : IChildItem<XmlConfData>
    {
        private XmlConfData _parent;
        private bool _isLoaded;
        private ComponentInformation _info;

        [XmlIgnore]
        public ComponentInformation Info
        {
            get => _info;
        }


        [XmlIgnore]
        public bool IsLoaded
        {
            get => _isLoaded;
        }

        public Assembly ComponentAssembly { get; set; }

        public void LoadComponent()
        {
            FileInfo fi = new FileInfo(Path.Combine(XmlConfHelper.BaseDirectory, this.File.Path));
            ComponentAssembly = Assembly.LoadFile(fi.FullName);

            var typeInfo = ComponentAssembly.GetTypes().FirstOrDefault(x => x.BaseType == typeof(ComponentInformation));

            if (typeInfo != null)
                _info = (ComponentInformation)Activator.CreateInstance(typeInfo);
            else
                _info = new ComponentInformation();
        }

        [XmlIgnore]
        public XmlConfRoot Root => _parent.Parent;

        [XmlIgnore]
        public XmlConfData Parent => _parent;

        #region SerializebleElements


        [XmlElement]
        public XmlConfFile File { get; set; }

        [XmlArray("Attaches")]
        [XmlArrayItem(ElementName = "Attach", Type = typeof(XmlConfAttach))]
        public List<XmlConfAttach> Attaches { get; set; }

        #endregion

        XmlConfData IChildItem<XmlConfData>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}