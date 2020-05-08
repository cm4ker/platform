using System;

namespace Aquila.EntityComponent.Entity
{
    /// <summary>
    /// Используется при генерации мапингов
    /// </summary>
    public class MapToAttribute : Attribute
    {
        public MapToAttribute(string dbColumnName)
        {
            DbColumnName = dbColumnName;
        }

        public string DbColumnName { get; set; }
    }

    public class NeedInitAttribute : Attribute
    {
    }
}