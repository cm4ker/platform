using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.SimpleRealization;

namespace ZenPlatform.Configuration
{
    public class ConfigurationManager
    {

        public PRootConfiguration Load(string mainPath)
        {
            var root = Deserialize(mainPath) as RootConfiguration;
            var proot = new PRootConfiguration(root.Id);
            proot.ConfigurationName = root.ConfigurationName;

            PComponent activeComponent = null;
            IConfigurationManagerComponent componentModule = null;

            /*
             * Внимание!!! Из за этой зашивки порядок ссылок на файлы очень важен.
             * Сначала должен идти компонент, только затем подвязанные к нему объекты.
             * В последующем алгоритм сканирования можно поменять
             */

            var types = new List<PTypeBase>() { new PBoolean(), new PDateTime(), new PString(), new PGuid(), new PNumeric() };

            foreach (var file in root.IncludedFiles)
            {
                var obj = Deserialize(file);
                switch (obj)
                {
                    case DataComponent dc:

                        activeComponent = new PComponent(dc.Id);
                        activeComponent.Name = dc.Name;
                        activeComponent.ComponentPath = dc.ComponentPath;
                        activeComponent.Id = dc.Id;

                        proot.RegisterDataComponent(activeComponent);

                        componentModule = GetComponentConfigurationManager(activeComponent);
                        break;
                    case DataComponentObject dco:

                        var pobj = componentModule.ConfigurationComponentObjectLoadHandler(activeComponent, dco);
                        types.Add(pobj);
                        break;
                }
            }
            /*
             * Мне просто лень заморачиваться сейчас и придумывать
             * какой-либо изящный алгоритм для загрузки.
             * Проблема: у нас есть свойства у объектов. Эти свойства имеют некий тип(ы).
             * Чтобы правильно загрузить тип, нам необходима ссылка на PTypeBase, которая
             * доступна только после того, как мы прогрузим объекты без свойств.
             * Для того чтобы корректно прогрузить свойства, мы идём пофайлам второй раз.
             */

            foreach (var file in root.IncludedFiles)
            {
                var obj = Deserialize(file);
                switch (obj)
                {
                    case DataComponentObject dco:

                        var pcomponent = proot.DataSectionComponents.First(x => x.Id == dco.ComponentId);
                        var pobj = pcomponent.GetObject(dco.Id);

                        var cm = GetComponentConfigurationManager(pcomponent);

                        foreach (var property in dco.Properties)
                        {
                            cm.ConfigurationObjectPropertyLoadHandler(pobj, property, types);
                        }
                        break;
                }
            }

            return proot;
        }

        private IConfigurationManagerComponent GetComponentConfigurationManager(PComponent pcomponent)
        {
            var componentAssembly = Assembly.LoadFrom(pcomponent.ComponentPath);
            var componentType = componentAssembly.DefinedTypes.First(x =>
                x.ImplementedInterfaces.Contains(typeof(IConfigurationManagerComponent)));
            return Activator.CreateInstance(componentType, pcomponent) as IConfigurationManagerComponent;
        }

        public void Unload(PRootConfiguration conf, string folder)
        {
            var root = new RootConfiguration();
            root.ConfigurationName = conf.ConfigurationName;
            root.Id = conf.Id;

            #region Component

            foreach (var pcomponent in conf.DataSectionComponents)
            {
                var com = new DataComponent();
                com.ComponentPath = pcomponent.ComponentPath;
                com.Name = pcomponent.Name;
                com.Id = pcomponent.Id;
                var componentAssembly = Assembly.LoadFrom(pcomponent.ComponentPath);
                var componentType = componentAssembly.DefinedTypes.First(x => x.ImplementedInterfaces.Contains(typeof(IConfigurationManagerComponent)));
                IConfigurationManagerComponent component = Activator.CreateInstance(componentType, pcomponent) as IConfigurationManagerComponent;

                var dataFolder = Path.Combine(folder, $"Data\\{com.Name}");
                var dataObjectFolder = Path.Combine(folder, $"Data\\{com.Name}\\Objects");

                if (!Directory.Exists(dataFolder))
                    Directory.CreateDirectory(dataFolder);

                if (!Directory.Exists(dataObjectFolder))
                    Directory.CreateDirectory(dataObjectFolder);

                var componentPath = Path.Combine(dataFolder, com.Name + ".json");
                root.IncludedFiles.Add(componentPath);

                foreach (var pobj in pcomponent.Objects)
                {
                    //TODO: Необходимо вынести менеджер конфигурации в отдельную сборку
                    /*
                     * Конфигурация должна загружаться и выгружаться с помощью стороннего компонента
                     * 
                     * Это необходимо также для генерации кода, когда мы делаем сборку приложения по конфигурации,
                     * в этот момент должно происходить следующее:
                     * Для компонента генерации кода передаётся путь для каталога с конфигурацией: для примера C:\Conf\
                     * ZenPlatform.CodeGeneration.exe C:\Conf\ C:\ProjectPath\
                     * 
                     * После этого в C:\ProjectPath\obj генерируются файлы проекта
                     * 
                     * Также необходимо проудмать, как мы будем осуществлять деплой проекта. 
                     * Готовый проект вместе с библиотеками заливается на MainServer после этого проходят миграции, если они необходимы.
                     * 
                     * Следующая схема наглядно демонстрирует зависимости:
                     * ConfigurationManager <- Configuration
                     * Platform <- ConfigurationManager, Configuration
                     * Migration <- ConfigurationManager, Configuration
                     * CodeGeneration <- ConfigurationManager, Configuration
                     */

                    var componentObject = component.ConfigurationUnloadHandler(pobj);

                    var componentObjectPath = Path.Combine(dataObjectFolder, componentObject.Name + ".json");

                    Serialize(componentObject, componentObjectPath);
                    root.IncludedFiles.Add(componentObjectPath);
                }

                Serialize(com, componentPath);
            }

            #endregion

            var rootPath = Path.Combine(folder, root.ConfigurationName + ".json");

            Serialize(root, rootPath);
        }

        private void Serialize(object obj, string path)
        {
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }));
            }
        }

        private object Deserialize(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return JsonConvert.DeserializeObject(sr.ReadToEnd(), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            }
        }
    }

    public class RootConfiguration
    {
        public RootConfiguration()
        {
            IncludedFiles = new List<string>();
        }

        public string ConfigurationName { get; set; }
        public List<string> IncludedFiles { get; set; }
        public Guid Id { get; set; }
    }

    public class DataComponent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ComponentPath { get; set; }
    }

    public class DataComponentObject
    {
        public DataComponentObject()
        {
            Properties = new List<DataComponentObjectProperty>();
            Container = new PropertyContainer();
        }

        public string Name { get; set; }
        public Guid Id { get; set; }
        public Guid ComponentId { get; set; }
        public List<DataComponentObjectProperty> Properties { get; set; }
        public PropertyContainer Container { get; set; }
        public string Description { get; set; }
    }

    public class DataComponentObjectProperty
    {
        public DataComponentObjectProperty()
        {
            Container = new PropertyContainer();
        }

        public Guid Id { get; set; }
        public List<Guid> Types { get; set; }
        public PropertyContainer Container { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public bool Unique { get; set; }
    }


    public class PropertyContainer
    {
        private Dictionary<string, object> _props = new Dictionary<string, object>();
        public void RegisterProperty(string name, object value)
        {
            _props.Add(name, value);
        }
    }

}
