using System;
using System.Collections.Generic;
using System.Reflection;
using Grpc.Core;
using ZenPlatform.Core.Environment;

namespace ZenPlatform.Core.RPC
{
    public class Server
    {
        private readonly Grpc.Core.Server _server;
        private readonly ISerializer _serializer;
        //private readonly PlatformEnvironment _environment;

        internal Server(Grpc.Core.Server server, ISerializer serializer) 
        {
            _server = server;
            
            _serializer = serializer;
         //   _environment = environment;
        }

        public void Start()
        {
            _server.Start();
        }

        /// <summary>
        /// Добавляет все публичные функции класса в сервис RPC
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        public void AddService<TService>()
            where TService: class
        {
            AddService(typeof(TService));
        }

        /// <summary>
        /// Добавляет все публичные функции класса в сервис RPC
        /// </summary>
        /// <typeparam name="service"></typeparam>
        public void AddService(Type service)
        {
            if (!service.IsClass) throw new ArgumentException("Параметер service дожен быть классом.");

            ServerServiceDefinition.Builder _builder = ServerServiceDefinition.CreateBuilder();

            foreach (var methodInfo in service.GetMethods(BindingFlags.Public))
            {
                AddMethod(_builder, RouteFactory.Instance.GetRoute(methodInfo), methodInfo);
            }


            _server.Services.Add(_builder.Build());
        }


        private void AddMethod(ServerServiceDefinition.Builder builder, Route route, MethodInfo methodInfo)
        {
            builder.AddMethod(MethodDeclaration.GetMethod<Request, Responce>(route, _serializer),
                    async (request, context) =>
                    {
                        ///

                        return new Responce()
                        {
                            result = methodInfo.Invoke(request.@this, request.patameters),
                            @this = request.@this
                        };
                    }
                );
        }

        /// <summary>
        /// Добавляет имплементацию функций TServiceInterface классом TService в сервис RPC
        /// </summary>
        /// <typeparam name="serviceInterface"></typeparam>
        /// <typeparam name="service"></typeparam>
        public void AddService(Type serviceInterface, Type service) 
        {
            if (!service.IsClass) throw new ArgumentException("Параметер service дожен быть классом.");
            if (!serviceInterface.IsInterface) throw new ArgumentException("Параметер serviceInterface дожен быть интерфейсом.");

            var map = service.GetInterfaceMap(serviceInterface);

            ServerServiceDefinition.Builder _builder = ServerServiceDefinition.CreateBuilder();

            foreach (var methodInfo in map.TargetMethods)
            {
                AddMethod(_builder, RouteFactory.Instance.GetRoute(methodInfo), methodInfo);
            }


            _server.Services.Add(_builder.Build());
        }

        /// <summary>
        /// Добавляет имплементацию функций TServiceInterface классом TService в сервис RPC
        /// </summary>
        /// <typeparam name="TServiceInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        public void AddService<TServiceInterface, TService>()
            where TServiceInterface : class
            where TService : class

        {
            AddService(typeof(TServiceInterface), typeof(TService));
        }


    }
}
