using System;
using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.Core
{
    public class Environment
    {
        private object _locking;

        public Environment()
        {
            _locking = new object();
            Sessions = new List<Session>();
        }

        public IList<Session> Sessions { get; }

        public Session CreateSession()
        {
            lock (_locking)
            {
                var id = Sessions.Max(x => x.Id) + 1;
                var session = new Session(this, id);

                Sessions.Add(session);

                return session;
            }
        }
    }

    public class Session
    {
        private readonly Environment _env;

        public Session(Environment env, int id)
        {
            _env = env;
            Id = id;
        }

        public int Id { get; }
    }
}
