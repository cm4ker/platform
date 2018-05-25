using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ZenPlatform.Configuration.ConfigurationLoader.Contracts;

namespace ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration
{
    /// <summary>
    /// Описание компонента
    /// </summary>
    public class XCComponent : IChildItem<XmlConfData>
    {
        private XmlConfData _parent;
        private bool _isLoaded;
        private ComponentInformation _info;
        private IComponenConfigurationLoader _loader;


        /// <summary>
        /// Информация о компоненте
        /// </summary>
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

        [XmlElement] public XmlConfFile File { get; set; }

        [XmlIgnore] public Assembly ComponentAssembly { get; set; }

        public void LoadComponent()
        {
            FileInfo fi = new FileInfo(Path.Combine(XmlConfHelper.BaseDirectory, this.File.Path));
            ComponentAssembly = Assembly.LoadFile(fi.FullName);

            var typeInfo = ComponentAssembly.GetTypes().FirstOrDefault(x => x.BaseType == typeof(ComponentInformation));

            if (typeInfo != null)
                _info = (ComponentInformation) Activator.CreateInstance(typeInfo);
            else
                _info = new ComponentInformation();

            var loaderType = ComponentAssembly.GetTypes()
                                 .FirstOrDefault(x =>
                                     x.IsPublic && !x.IsAbstract &&
                                     x.GetInterfaces().Contains(typeof(IComponenConfigurationLoader))) ??
                             throw new InvalidComponentException();


            _loader = (IComponenConfigurationLoader) Activator.CreateInstance(loaderType);
        }

        [XmlIgnore] public XmlConfRoot Root => _parent.Parent;

        [XmlIgnore] public XmlConfData Parent => _parent;

        [XmlIgnore] public IComponenConfigurationLoader Loader => _loader;

        XmlConfData IChildItem<XmlConfData>.Parent
        {
            get => _parent;
            set => _parent = value;
        }
    }
}