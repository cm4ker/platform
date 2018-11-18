using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Structure.Data.Types.Complex
{
    /// <summary>
    /// Коллекция свойст, предлагает расширение для класса ChildItemCollection
    /// </summary>
    /// <typeparam name="TBaseType">Тип базового объекта</typeparam>
    /// <typeparam name="TProperty">Тип элементов коллекции свойств</typeparam>
    public class XCPropertyCollection<TBaseType, TProperty> : ChildItemCollection<TBaseType, TProperty>
        where TProperty : XCObjectPropertyBase, IChildItem<TBaseType> where TBaseType : class
    {
        public XCPropertyCollection(TBaseType parent) : base(parent)
        {
        }

        public XCPropertyCollection(TBaseType parent, IList<TProperty> collection) : base(parent, collection)
        {
        }


        public TProperty GetProperty(Guid guid)
        {
            return this.FirstOrDefault(x => x.Guid == guid);
        }
    }
}