using System.Collections.Generic;
using ZenPlatform.Configuration.Common;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class MDInterface
    {
        public string Name { get; set; }

        public InterfaceType Type { get; set; }

        public string Markup { get; set; }

        //TODO: Здесь должны быть метаданные подключаемой модели
        public Dictionary<string, MDType> ViewModel { get; set; }
    }
}