using System;
using System.Collections.Generic;

namespace Aquila.WebServiceComponent.Configuration
{
    /// <summary>
    /// Тип данных, который может сериализоваться (отсутствуют циклические зависимости)
    /// </summary>
    public class MDWebService
    {
        public MDWebService()
        {
            Methods = new List<MDMethod>();
            RefId = Guid.NewGuid();
        }

        public Guid RefId { get; set; }

        public string Name { get; set; }

        public string Module { get; set; }

        public List<MDMethod> Methods { get; set; }
    }
}