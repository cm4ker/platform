using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Network
{

    public class Route_old
    {

        public Route_old(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }

        public override bool Equals(object obj)
        {
            Route_old route = obj as Route_old;
            return route.Path == Path;

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
