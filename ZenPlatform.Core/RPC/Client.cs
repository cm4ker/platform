using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZenPlatform.Core.RPC
{
    public class Client
    {
        private readonly Channel _channel;
        private DefaultCallInvoker _invoker;
        private readonly ISerializer _serializer;

        internal Client(Channel channel, ISerializer serializer)
        {
            
            _channel = channel;
            _invoker = new DefaultCallInvoker(channel);
            _serializer = serializer;


        }
        public async Task<object> Invoke(Route route, object @this, object[] parameters)
        {

            Request request = new Request()
            {
                patameters = parameters,
                @this = @this
            };

            var callOptions = new CallOptions();
            using (var call = _invoker.AsyncUnaryCall<Request, Responce>(
                MethodDeclaration.GetMethod<Request, Responce>(route, _serializer), 
                null, callOptions, request))
            {
                var result = await call.ResponseAsync.ConfigureAwait(false);

                if (result.@this != null)
                    CopyAll(result.@this, @this);

                return result.result;
                 
            }
            
        }

        private void CopyAll(object source, object target)
        {
           
            var type = source.GetType();

            if (type != target.GetType()) throw new ArgumentException("Параметры должны быть одного типа");

            foreach (var sourceProperty in type.GetProperties())
            {
                var targetProperty = type.GetProperty(sourceProperty.Name);
                targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
            }
            foreach (var sourceField in type.GetFields())
            {
                var targetField = type.GetField(sourceField.Name);
                targetField.SetValue(target, sourceField.GetValue(source));
            }
        }
    }
}
