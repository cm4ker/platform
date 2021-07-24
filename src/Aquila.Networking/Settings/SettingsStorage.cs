using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Aquila.Core.Settings
{
    public class ConfigFileNameAttribute : Attribute
    {
        public string Name { get; set; }
    }

    public interface ISettingsStorage
    {
        T Get<T>() where T : class, new();
        void Save();
    }

    public class FileSettingsStorage : ISettingsStorage
    {
        private Dictionary<Type, object> _cache = new Dictionary<Type, object>();
        private string _settingsPath;

        public FileSettingsStorage()
        {
            _settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Settings");
            if (!Directory.Exists(_settingsPath))
                Directory.CreateDirectory(_settingsPath);
        }

        public T Get<T>() where T : class, new()
        {
            if (_cache.ContainsKey(typeof(T)))
                return (T) _cache[typeof(T)];

            var attr = typeof(T).GetCustomAttribute<ConfigFileNameAttribute>();

            string path;

            if (attr != null)
                path = Path.Combine(_settingsPath, attr.Name);
            else
                path = Path.Combine(_settingsPath, typeof(T).Name);

            if (File.Exists(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader sr = new StreamReader(path))
                using (JsonReader jr = new JsonTextReader(sr))
                {
                    var result = serializer.Deserialize<T>(jr);

                    _cache.Add(typeof(T), result);

                    return result;
                }
            }
            else
            {
                var newSettings = new T();
                _cache.Add(typeof(T), newSettings);

                JsonSerializer serializer = new JsonSerializer();
                using (StreamWriter sr = new StreamWriter(path))
                using (JsonWriter jr = new JsonTextWriter(sr))
                {
                    serializer.Serialize(jr, newSettings);
                }

                return newSettings;
            }
        }

        public void Save()
        {
            foreach (var item in _cache.Values)
            {
                Save(item);
            }
        }

        private void Save(object setting)
        {
            var path = Path.Combine(_settingsPath, setting.GetType().Name);

            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                serializer.Serialize(jw, setting);
            }
        }
    }
}