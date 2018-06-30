using System.Collections.Generic;
using ZenPlatform.Core.Authentication;
using ZenPlatform.Data;

namespace ZenPlatform.Core
{
    public interface ISession
    {
        int Id { get; }
        DataContextManger DataContextManger { get; }
        PlatformEnvironment Environment { get; }
        void SetGlobalParameter(string key, object value);
        object GetGlobalParameter(string key, object value);
    }

    public abstract class Session : ISession
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

        public UserManager GetUserManager()
        {
            return new UserManager(this);
        }

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
        private readonly User _user;

        public UserSesion(PlatformEnvironment env, User user, int id) : base(env, id)
        {
            _user = user;
        }

        public User User => _user;
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