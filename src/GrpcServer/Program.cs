using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SoapCore;


namespace GrpcServer
{
    class GreeterImpl : Greeter.GreeterBase
    {
        // Server side handler of the SayHello RPC
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply {Message = "Hello " + request.Name});
        }

        public override Task<HelloReply> SayHelloAgain(HelloRequest request, ServerCallContext context)
        {
            return base.SayHelloAgain(request, context);
        }
    }

    class Program
    {
        const int Port = 50051;

        public static void Main(string[] args)
        {
            // Server server = new Server
            // {
            //     Services = {Greeter.BindService(new GreeterImpl())},
            //     Ports = {new ServerPort("localhost", Port, ServerCredentials.Insecure)}
            // };
            // server.Start();
            //
            // Console.WriteLine("Greeter server listening on port " + Port);
            // Console.WriteLine("Press any key to stop the server...");
            // Console.ReadKey();
            //
            // server.ShutdownAsync().Wait();


            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .ConfigureServices(x => x.AddRouting())
                .UseContentRoot(Directory.GetCurrentDirectory())
                .Build();
            host.Run();
        }
    }

    public class Startup
    {
        private static void HandleMapTest1(IApplicationBuilder app)
        {
            app.Run(async context => { await context.Response.WriteAsync("Map Test 1"); });
        }

        private static void HandleMapTest2(IApplicationBuilder app)
        {
            app.Run(async context => { await context.Response.WriteAsync("Map Test 2"); });
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<ISampleService, SampleService>();
            services.AddSoapCore();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/hello/{name}/{name2}", async context =>
                {
                    var name = context.Request.RouteValues["name"];
                    var name2 = context.Request.RouteValues["name2"];

                    await context.Response.WriteAsync($"Hello {name} {name2}!");
                });
            });

            app.UseSoapEndpoint<ISampleService>("/Service.svc", new BasicHttpBinding(),
                SoapSerializer.DataContractSerializer);
            app.UseSoapEndpoint<ISampleService>("/Service.asmx", new BasicHttpBinding(), SoapSerializer.XmlSerializer);

            app.Run(context =>
                context.Response.WriteAsync("Default"));
        }
    }
}