using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Aquila.Web.Razor;

public class AqComponentBase : ComponentBase
{
    [CanBeNull]
    [Parameter]
    public string InstanceName { get; set; }

    /// <summary>
    /// Method defined for the creating symbol in il metadata
    /// </summary>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        return base.SetParametersAsync(parameters);
        //return Task.CompletedTask;
    }
}