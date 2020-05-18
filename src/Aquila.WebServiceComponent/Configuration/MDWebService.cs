using System;
using System.Collections.Generic;
using Aquila.Component.Shared.Configuration;

namespace Aquila.WebServiceComponent.Configuration
{
    /// <summary>
    /// Тип данных, который может сериализоваться (отсутствуют циклические зависимости)
    /// </summary>
    public class MDWebService
    {
        public MDWebService()
        {
            Modules = new List<MDProgramModule>();
            RefId = Guid.NewGuid();
        }

        public Guid RefId { get; set; }

        public string Name { get; set; }

        public List<MDProgramModule> Modules { get; set; }
    }
}