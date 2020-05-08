using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Ide.Contracts
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewAttribute: Attribute
    {
        public Type ViewType { get; }
        public ViewAttribute(Type viewType)
        {
            ViewType = viewType;
        }
    }
}
