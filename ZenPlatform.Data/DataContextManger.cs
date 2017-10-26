using System;
using System.Collections.Generic;
using System.Threading;

namespace ZenPlatform.Data
{
    public class DataContextManger
    {
        private Dictionary<int, DataContext> _contexts;

        public DataContextManger()
        {
            _contexts = new Dictionary<int, DataContext>();
        }

        public DataContext GetContext()
        {
            if (_contexts.TryGetValue(Thread.CurrentThread.ManagedThreadId, out var context))
            {
                return context;
            }
            else
            {
                //TODO: Необхоидмо добавить проект конфигурации нео

                throw new NotImplementedException();
                return new DataContext("");
            }
        }
    }
}