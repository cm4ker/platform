using System;
using Microsoft.Extensions.DependencyInjection;

namespace ZenPlatform.Configuration.Storage
{
    public class DatabaseConfigurationKeeper : ConfigurationKeeper
    {
        public DatabaseConfigurationKeeper(IServiceProvider serviceProvider)
            : base(serviceProvider.GetRequiredService<DatabaseDataStorage>(), 
                serviceProvider.GetRequiredService<FileDataStorage>(), 
                serviceProvider.GetRequiredService<MemoryDataStorage>())
        {

        }
    }
}