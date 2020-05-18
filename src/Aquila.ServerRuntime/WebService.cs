using System.ServiceModel;
using Microsoft.AspNetCore.Builder;
using SoapCore;

namespace Aquila.ServerRuntime
{
    public static class WebService
    {
        public static void CreateWebService<T>(IApplicationBuilder builder, string serviceName)
        {
            builder.UseSoapEndpoint<T>($"/api/{serviceName}.svc", new BasicHttpBinding());
        }
    }
}