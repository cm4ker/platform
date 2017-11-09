using System.Collections.Generic;
using ZenPlatform.Core.Entity;
using ZenPlatform.Data;

namespace ZenPlatform.Core
{
    public class Session
    {
        public Session(PlatformEnvironment env, int id)
        {
            Environment = env;
            Id = id;

            DataContextManger = new DataContextManger();

        }

        public int Id { get; }

        public DataContextManger DataContextManger { get; }
        public PlatformEnvironment Environment { get; }

        //TODO: Все компоненты инициализируются для сессии, так что необходимо, чтобы компоненты были доступны из сессии, либо на уровне ниже


        public void SetGlobalParameter(string key, object value)
        {
            Environment.Globals[key] = value;
        }

        public object GetGlobalParameter(string key, object value)
        {
            return Environment.Globals[key];
        }

    }
}