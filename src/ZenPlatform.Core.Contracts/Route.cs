using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.Contracts
{

    public class Route
    {

        public Route(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }

        public override bool Equals(object obj)
        {
            Route route = obj as Route;
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
