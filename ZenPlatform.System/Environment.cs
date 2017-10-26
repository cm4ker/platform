using System;
using System.Collections.Generic;
using System.Linq;

namespace ZenPlatform.System
{
    public class Environment
    {
        private object _locking;

        public Environment()
        {
            _locking = new object();
            Sessions = new List<Session>();
            Globals = new Dictionary<string, object>();
        }

        public Dictionary<string, object> Globals { get; set; }

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

        public void RemoveSession(Session session)
        {
            lock (_locking)
            {
                Sessions.Remove(session);
            }
        }

        public void RemoveSession(int id)
        {
            lock (_locking)
            {
                var session = Sessions.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Session not found");
                Sessions.Remove(session);
            }
        }
    }
}
