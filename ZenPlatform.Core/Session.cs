using System.Collections.Generic;
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

    /// <summary>
    /// Пользовательская сессия - по виду этой сессии мы можем запрещать незакконные операции - например мигрирование базы данных
    /// Пользователь не может выполнять инструкции связанные с изменением схемы, он лишь манипулирует данными
    /// </summary>
    public class UserSesion : Session
    {
        public UserSesion(PlatformEnvironment env, int id) : base(env, id)
        {

        }
    }

    /// <summary>
    /// Системная сессия, нужна для того, чтобы выполнять операции, которые не может выполнить пользователь
    /// Например обновление схемы данных
    /// </summary>
    public class SystemSession : Session
    {
        public SystemSession(PlatformEnvironment env, int id) : base(env, id)
        {

        }
    }
}