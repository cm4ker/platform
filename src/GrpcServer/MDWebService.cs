using System.Collections.Generic;
using ZenPlatform.Configuration.Common;

namespace GrpcServer
{
    public class MDWebService
    {
        public string Name { get; set; }
    }

    /// <summary>
    /// Тип данных, который может сериализоваться (отсутствуют циклические зависимости)
    /// </summary>
    public class MDSerializableType
    {
        public string Name { get; set; }

        public List<MDSerializableProperty> Properties { get; set; }
    }

    public class MDSerializableProperty
    {
        public string Name { get; set; }

        public MDType Type { get; set; }
    }

    /*
        var type      = $SerializedTypes.MyType1();
        type.SomeProp = store.Test; 
    */
}