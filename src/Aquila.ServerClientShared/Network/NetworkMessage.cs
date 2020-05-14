using System;
using System.Collections.Generic;
using System.Text;
using Aquila.Core.Authentication;
using Aquila.Core.Contracts;

namespace Aquila.Core.Network
{

    public interface INetworkMessage
    {
        Guid Id { get; }
        Guid RequestId { get; }
    }

    public interface IInvokeMessage : INetworkMessage
    {
        
    }

    [Serializable]
    public class OkNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }

        public Guid RequestId { get; private set; }

        public OkNetworkMessage(Guid requestId)
        {
            Id = Guid.NewGuid();
            RequestId = requestId;
        }
    }

    [Serializable]
    public class PingNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }

        public Guid RequestId { get; private set; }
    }

    [Serializable]
    public class RequestInvokeUnaryNetworkMessage : IInvokeMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public Route Route { get; private set; }
        public object[] Args { get; private set; }

        public RequestInvokeUnaryNetworkMessage() { }

        public RequestInvokeUnaryNetworkMessage(Route route, object[] args)
        {
            Id = Guid.NewGuid();
            Route = route;
            Args = args;
        }

    }

    [Serializable]
    public class ResponceInvokeUnaryNetworkMessage : IInvokeMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public object Result { get; private set; }

        public ResponceInvokeUnaryNetworkMessage() { }

        public ResponceInvokeUnaryNetworkMessage(Guid InvokeId, object result)
        {
            Id = Guid.NewGuid();
            RequestId = InvokeId;
            Result = result;
        }
    }

    [Serializable]
    public class RequestInvokeUnaryByteArgsNetworkMessage : IInvokeMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public Route Route { get; private set; }
        public byte[][] Args { get; private set; }

        public RequestInvokeUnaryByteArgsNetworkMessage() { }

        public RequestInvokeUnaryByteArgsNetworkMessage(Route route, byte[][] args)
        {
            Id = Guid.NewGuid();
            Route = route;
            Args = args;
        }

    }

    [Serializable]
    public class ResponceInvokeUnaryByteArgsNetworkMessage : IInvokeMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public byte[] Result { get; private set; }

        public ResponceInvokeUnaryByteArgsNetworkMessage() { }

        public ResponceInvokeUnaryByteArgsNetworkMessage(Guid InvokeId, byte[] result)
        {
            Id = Guid.NewGuid();
            RequestId = InvokeId;
            Result = result;
        }
    }


    [Serializable]
    public class ErrorNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }

        public string ErrorMessage { get; private set; }
        public Exception Exception { get; private set; }

        public ErrorNetworkMessage() { }

        public ErrorNetworkMessage(Guid InvokeId, string errorMessage, Exception exception = null)
        {
            Id = Guid.NewGuid();
            RequestId = InvokeId;
            ErrorMessage = errorMessage;
            Exception = exception;
        }
    }


    [Serializable]
    public class RequestEnvironmentListNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }

        public RequestEnvironmentListNetworkMessage()
        {
            Id = Guid.NewGuid();
        }

    }


    [Serializable]
    public class RequestEnvironmentUseNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public string Name { get; private set; }

        public RequestEnvironmentUseNetworkMessage() { }

        public RequestEnvironmentUseNetworkMessage(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

    }

    [Serializable]
    public class ResponceEnvironmentUseNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public string Name { get; private set; }
        public ResponceEnvironmentUseNetworkMessage() { }
        public ResponceEnvironmentUseNetworkMessage(RequestEnvironmentUseNetworkMessage request)
        {
            Id = Guid.NewGuid();
            RequestId = request.Id;
            Name = request.Name;
        }

    }


    [Serializable]
    public class ResponceEnvironmentListNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public List<string> List { get; private set; }


        public ResponceEnvironmentListNetworkMessage() { }
        public ResponceEnvironmentListNetworkMessage(Guid requestId, List<string> list)
        {
            Id = Guid.NewGuid();
            RequestId = requestId;
            List = list;
        }
    }

    [Serializable]
    public class DataStreamNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public byte[] Data { get; private set; }

        public DataStreamNetworkMessage() { }

        public DataStreamNetworkMessage(Guid requestId, byte[] data)
        {
            Id = Guid.NewGuid();
            Data = data;
            RequestId = requestId;
        }
    }

    [Serializable]
    public class StartInvokeStreamNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public Route Route { get; private set; }
        public object Request { get; private set; }

        public StartInvokeStreamNetworkMessage() { }

        public StartInvokeStreamNetworkMessage(Route route, object request)
        {
            Id = Guid.NewGuid();
            Route = route;
            Request = request;
        }
    }

    [Serializable]
    public class EndInvokeStreamNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }

        public EndInvokeStreamNetworkMessage() { }

        public EndInvokeStreamNetworkMessage(Guid requestId)
        {
            Id = Guid.NewGuid();
            RequestId = requestId;
        }
    }

    [Serializable]
    public class RequestAuthenticationNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public IAuthenticationToken Token { get; private set; }

        public RequestAuthenticationNetworkMessage() { }

        public RequestAuthenticationNetworkMessage(IAuthenticationToken token)
        {
            Id = Guid.NewGuid();
            Token = token;
        }

    }

    [Serializable]
    public class ResponceAuthenticationNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }

        public ResponceAuthenticationNetworkMessage() { }

        public ResponceAuthenticationNetworkMessage(RequestAuthenticationNetworkMessage request)
        {
            Id = Guid.NewGuid();
            RequestId = request.Id;

        }

    }

    [Serializable]
    public class RequestInvokeInstanceProxy: INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public string InterfaceName { get; private set; }
        public RequestInvokeInstanceProxy(string interfaceName)
        {
            Id = Guid.NewGuid();
            InterfaceName = interfaceName;
        }
    }

    [Serializable]
    public class RequestInvokeDisposeProxy : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public RequestInvokeDisposeProxy(Guid requestI)
        {
            Id = Guid.NewGuid();
            RequestId = requestI;
        }
    }

    [Serializable]
    public class RequestInvokeMethodProxy : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public string MethodName { get; private set; }
        public object[] Args { get; private set; }

        public RequestInvokeMethodProxy(Guid requestId, string methodName, object[] args)
        {
            Id = Guid.NewGuid();
            MethodName = methodName;
            RequestId = requestId;
            Args = args;
        }
    }

    [Serializable]
    public class ResponceInvokeMethodProxy : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public object Result { get; private set; }

        public ResponceInvokeMethodProxy(Guid requestId, object result )
        {
            RequestId = requestId;
            Result = result;
        }
    }

    public class RequestInvokeStreamProxy : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public string MethodName { get; private set; }
        public object[] Args { get; private set; }

        public RequestInvokeStreamProxy(Guid requestId, string methodName, object[] args)
        {
            Id = Guid.NewGuid();
            MethodName = methodName;
            RequestId = requestId;
            Args = args;
        }
    }

    

}

