using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Core.Authentication;

namespace ZenPlatform.ServerClientShared.Network
{

    public interface INetworkMessage
    {
        Guid Id { get; }
        Guid RequestId { get; }
    }

    public interface IInvokeMessage : INetworkMessage
    {
        
    }

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

    public class PingNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }

        public Guid RequestId { get; private set; }
    }

    public class RequestInvokeUnaryNetworkMessage : IInvokeMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public Route Route { get; private set; }
        public object Request { get; private set; }

        public RequestInvokeUnaryNetworkMessage() { }

        public RequestInvokeUnaryNetworkMessage(Route route, object request)
        {
            Id = Guid.NewGuid();
            Route = route;
            Request = request;
        }

    }

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


    public class RequestEnvironmentListNetworkMessage : INetworkMessage
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }

        public RequestEnvironmentListNetworkMessage()
        {
            Id = Guid.NewGuid();
        }

    }


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

}

