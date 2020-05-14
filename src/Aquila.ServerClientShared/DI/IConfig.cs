using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Core.DI
{
    public interface IConfig<out T> where T: class
    {
        T Value { get; }
    }
}
