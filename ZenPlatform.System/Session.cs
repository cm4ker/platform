using ZenPlatform.Data;

namespace ZenPlatform.System
{
    public class Session
    {
        private readonly Environment _env;

        public Session(Environment env, int id)
        {
            _env = env;
            Id = id;

            DataContextManger = new DataContextManger();
        }

        public int Id { get; }

        public DataContextManger DataContextManger { get; }
        //TODO: Все компоненты инициализируются для сессии, так что необходимо, чтобы компоненты были доступны из сессии, либо на уровне ниже

        public void SetGlobalParameter(string key, object value)
        {
            _env.Globals[key] = value;
        }

        public object GetGlobalParameter(string key, object value)
        {
            return _env.Globals[key];
        }

    }
}