using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.DI
{
    public static class DependencyHelper
    {
        public static IServiceCollection AddConfig<T>(this IServiceCollection services, T config) where T : class
        {
            services.AddSingleton<IConfig<T>>((sp) => new SimpleConfig<T>(config));

            return services;
        }



    }
    

 
    
}
