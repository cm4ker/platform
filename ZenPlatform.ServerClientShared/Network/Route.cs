using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Network
{
    [Serializable]
    public class Route
    {

        public Route(string path)
        {
            Path = path;
          //  RouteID = HashCode.Combine(path);
        }

        public string Path { get; private set; }
    //    public int RouteID { get; set; }

        public override bool Equals(object obj)
        {
            Route route = obj as Route;
            return route.Path == Path;

        }

        public string GetService()
        {
            var split = Path.Split('\\');

            return split[0];

        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        public override string ToString()
        {
            return Path;
        }
    }
}
