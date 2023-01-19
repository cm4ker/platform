using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Aquila.Web.Swagger;

public class AquilaApiHolder
{
    #region ApiHolder

    public ConcurrentDictionary<(string instanceName, string objectName, string methodName, string operationType),
            MethodInfo>
        Cruds { get; } = new();

    public ConcurrentDictionary<(string instanceName, string viewName), Type> Views { get; } = new();

    public ConcurrentDictionary<(string instanceName, string methodName), MethodInfo> Endpoints { get; } = new();


    public bool TryGetCrud(string instanceName, string objectName, string methodName, string operationType,
        out MethodInfo method)
    {
        return Cruds.TryGetValue((instanceName.ToLowerInvariant(), objectName, methodName, operationType), out method);
    }

    public bool TryGetEndpoint(string instanceName, string methodName, out MethodInfo method)
    {
        return Endpoints.TryGetValue((instanceName.ToLowerInvariant(), methodName), out method);
    }

    public void AddCrud(string instanceName, string objectName, string methodName, string operationType,
        MethodInfo method)
    {
        Cruds.TryAdd((instanceName.ToLowerInvariant(), objectName, methodName, operationType), method);
    }

    public void AddEndpoint(string instanceName, string methodName, MethodInfo method)
    {
        Endpoints.TryAdd((instanceName.ToLowerInvariant(), methodName), method);
    }

    public void UnregisterInstance(string instanceName)
    {
        var crudKey = Cruds.Where(x =>
            x.Key.instanceName.Equals(instanceName, StringComparison.InvariantCultureIgnoreCase));

        foreach (var value in crudKey)
        {
            Cruds.TryRemove(value);
        }

        var epKey = Endpoints.Where(x =>
            x.Key.instanceName.Equals(instanceName, StringComparison.InvariantCultureIgnoreCase));

        foreach (var value in epKey)
        {
            Endpoints.TryRemove(value);
        }
    }

    #endregion
}