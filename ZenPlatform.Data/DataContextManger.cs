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
                //TODO: Необходимо, чтобы на уровене платформы была какая-то конфигурация, откуда можно было бы взять connectionString
                //DefaultPath, TimeOuts, ConnectionCount и так далее.

                //throw new NotImplementedException();
                return new DataContext("Data source=(local);Initial catalog=TestDatabase; Integrated security=true;");
            }
        }
    }
}