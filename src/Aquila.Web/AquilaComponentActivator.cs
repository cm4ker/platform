using System;
using Aquila.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace Aquila.Web;

public class AquilaComponentActivator : IComponentActivator
{
    private readonly IHttpContextAccessor _accessor;

    public AquilaComponentActivator(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }
        
        
    public IComponent CreateInstance(Type componentType)
    {
        var context = _accessor.HttpContext.TryGetOrCreateContext();

        if (componentType.GetConstructor(new[] { typeof(AqContext) }) == null)
        {
            return Activator.CreateInstance(componentType) as IComponent ?? 
                   throw new InvalidOperationException("This is not a component");
        }

        if (Activator.CreateInstance(componentType, context) is not IComponent com)
        {
            throw new InvalidOperationException("Invalid platform component");
        }

        return com;
    }
}