using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using SqlPlusDbSync.Shared;

namespace SqlPlusDbSync.Data
{
    public class LocalFileStorage
    {
        public static void Save(object obj)
        {
            Type type = obj.GetType();
            var data = JsonConvert.SerializeObject(obj, Formatting.Indented);

            using (FileStream fs = File.Open(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{type.Name}_storage.json"), FileMode.Create))
            using (TextWriter tw = new StreamWriter(fs))
            {
                tw.Write(data);
            }
        }

        public static T Load<T>()
        {
            var type = typeof(T);

            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{type.Name}_storage.json")))
            {
                Logger.LogDebug("File not exists");
                var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { }, null);
                var obj = constructor.Invoke(new object[] { });
                Save(obj);
            }

            using (FileStream fs = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{type.Name}_storage.json")))
            using (TextReader tr = new StreamReader(fs))
            {
                var instance = JsonConvert.DeserializeObject<T>(tr.ReadToEnd());
                if (instance == null)
                    throw new Exception("can't load store");

                return instance;
            }
        }
    }
}
