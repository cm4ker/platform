using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Aquila.Core
{
    public class ViewBag : Dictionary<string, object>
    {
        public ViewBag()
        {
        }

        public bool Has(string name)
        {
            return ContainsKey(name);
        }

        public T Get<T>(string name)
        {
            return (T) this[name];
        }
    }
}