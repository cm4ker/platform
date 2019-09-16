using System.Collections.Generic;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    /// <summary>
    /// Коллекция программных модулей для объекта
    /// </summary>
    public class XCProgramModuleCollection<TBaseType, TProperty> : ChildItemCollection<TBaseType, TProperty>
        where TProperty : XCProgramModuleBase, IChildItem<TBaseType> where TBaseType : class
    {
        public XCProgramModuleCollection(TBaseType parent) : base(parent)
        {
        }

        public XCProgramModuleCollection(TBaseType parent, IList<TProperty> collection) : base(parent, collection)
        {
        }
    }
}