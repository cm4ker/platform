using System;
using Microsoft.Extensions.DependencyInjection;

namespace Aquila.Configuration.Storage
{
    public class FileConfigurationKeeper : ConfigurationKeeper
    {
        public FileConfigurationKeeper(IServiceProvider serviceProvider)
            : base(serviceProvider.GetRequiredService<FileDataStorage>(),
                serviceProvider.GetRequiredService<DatabaseDataStorage>(),
                serviceProvider.GetRequiredService<MemoryDataStorage>())
        {

        }
    }
}