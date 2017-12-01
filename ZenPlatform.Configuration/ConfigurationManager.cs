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

        public PRootConfiguration Load(string path)
        {
            return new PRootConfiguration();
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

                var componentAssembly = Assembly.LoadFrom(pcomponent.ComponentPath);
                var componentType = componentAssembly.DefinedTypes.First(x => x.BaseType.Name == "DataComponentBase");
                var componentLoadMethod = componentType.GetDeclaredMethod("ConfigurationUnLoadHandler");
                var component = Activator.CreateInstance(componentType, pcomponent);

                var dataFolder = Path.Combine(folder, $"Data\\{com.Name}");
                var dataObjectFolder = Path.Combine(folder, $"Data\\{com.Name}\\Objects");

                if (!Directory.Exists(dataFolder))
                    Directory.CreateDirectory(dataFolder);

                if (!Directory.Exists(dataObjectFolder))
                    Directory.CreateDirectory(dataObjectFolder);

                foreach (var pobj in pcomponent.Objects)
                {
                    //TODO: Необходимо вынести менеджер конфигурации в отдельную сборку

                    /*
                     * 
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
                     * 
                     */

                    var componentObject = componentLoadMethod.Invoke(component, new object[] { pobj }) as DataComponentObject;

                    var componentObjectPath = Path.Combine(dataObjectFolder, componentObject.Name + ".json");

                    using (StreamWriter sw = new StreamWriter(componentObjectPath, false))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(componentObject, Formatting.Indented));
                    }

                    root.IncludedFiles.Add(componentObjectPath);
                }

                var componentPath = Path.Combine(dataFolder, com.Name + ".json");

                root.IncludedFiles.Add(componentPath);

                using (StreamWriter sw = new StreamWriter(componentPath, false))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(com, Formatting.Indented));
                }
            }

            #endregion

            var rootPath = Path.Combine(folder, root.ConfigurationName + ".json");
            using (StreamWriter sw = new StreamWriter(rootPath, false))
            {
                sw.WriteLine(JsonConvert.SerializeObject(root, Formatting.Indented));
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

    public class SimpleObjectConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(PSimpleObjectType) == objectType;
        }
    }

    public class ComponentConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new PComponent();

            if (reader.TokenType == JsonToken.Null)
                return null;
            var contract = serializer.ContractResolver.ResolveContract(objectType) as JsonObjectContract;
            if (contract == null)
                throw new JsonSerializationException("invalid type " + objectType.FullName);
            var value = existingValue ?? contract.DefaultCreator();
            var jObj = JObject.Load(reader);

            var objectsPropertyValue = jObj.GetValue(nameof(PComponent.Objects));

            foreach (var jToken in objectsPropertyValue.Children())
            {
                var child = (JObject)jToken;
                var item = result.CreateObject<PSimpleObjectType>(child?.GetValue(nameof(PSimpleObjectType.Name)).ToString());


                var itemPropertiesValue = child.GetValue(nameof(PObjectType.Properties)).Children();
                foreach (var itemProperty in itemPropertiesValue)
                {
                    var property = new PSimpleProperty(item);
                    item.Properties.Add(property);
                    serializer.Populate(itemProperty.CreateReader(), property);
                }

                //serializer.Populate(child.CreateReader(), item);
            }

            result.ComponentPath = jObj.GetValue(nameof(PComponent.ComponentPath)).ToString();
            result.Name = jObj.GetValue(nameof(PComponent.Name)).ToString();

            return result;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(PComponent) == objectType;
        }
    }
}
