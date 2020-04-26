using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Ide.Contracts
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
