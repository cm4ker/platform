using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Aquila.WebServiceCore
{
    public interface IStartupService
    {
        void Register(Action<IApplicationBuilder> a);

        void RegisterWebServiceClass<T>() where T : class;

        void Configure(IApplicationBuilder buildedr);

        void ConfigureServices(IServiceCollection sc);
    }
}