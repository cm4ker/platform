using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

[Route("/somepath/counter")]
public class Counter : ComponentBase
{
    private int currentCount = 0;

    protected override void BuildRenderTree(RenderTreeBuilder __builder)
    {
        __builder.OpenComponent<PageTitle>(0);
        __builder.AddAttribute(1, "ChildContent",
            (RenderFragment)delegate(RenderTreeBuilder __builder2) { __builder2.AddContent(2, "Counter"); });
        __builder.CloseComponent();
        __builder.AddMarkupContent(3, "\r\n\r\n");
        __builder.AddMarkupContent(4, "<h1>Counter</h1>\r\n\r\n");
        __builder.OpenElement(5, "p");
        __builder.AddAttribute(6, "role", "status");
        __builder.AddContent(7, "Current count: ");
        __builder.AddContent(8, currentCount);
        __builder.CloseElement();
        __builder.AddMarkupContent(9, "\r\n\r\n");
        __builder.OpenElement(10, "button");
        __builder.AddAttribute(11, "class", "btn btn-primary");
        __builder.AddAttribute(12, "onclick",
            EventCallback.Factory.Create<MouseEventArgs>(this, new Action(IncrementCount)));
        __builder.AddContent(13, "Click me");
        __builder.CloseElement();
    }

    private void IncrementCount()
    {
        currentCount++;
    }
}