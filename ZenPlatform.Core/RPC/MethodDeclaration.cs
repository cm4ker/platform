using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Core.RPC
{
    public static class MethodDeclaration
    {
        public static Method<TRequers, TResponce> GetMethod<TRequers, TResponce>(Route route, ISerializer serializer)
        {
            return new Method<TRequers, TResponce>(MethodType.ServerStreaming, route.ServiceName, route.MethodName,
                   Marshallers.Create(serializer.ToBytes<TRequers>, serializer.FromBytes<TRequers>),
                   Marshallers.Create(serializer.ToBytes<TResponce>, serializer.FromBytes<TResponce>));
        }
    }
}
