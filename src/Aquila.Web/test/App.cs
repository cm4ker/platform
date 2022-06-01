// btest.App

using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

public class App : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder __builder)
    {
        __builder.OpenComponent<Router>(0);
        __builder.AddAttribute(1, "AppAssembly", RuntimeHelpers.TypeCheck(typeof(App).Assembly));
        __builder.AddAttribute(2, "Found", (RenderFragment<RouteData>)((RouteData routeData) => delegate(RenderTreeBuilder __builder2)
        {
            __builder2.OpenComponent<RouteView>(3);
            __builder2.AddAttribute(4, "RouteData", RuntimeHelpers.TypeCheck(routeData));
            __builder2.AddAttribute(5, "DefaultLayout", RuntimeHelpers.TypeCheck(typeof(MainLayout)));
            __builder2.CloseComponent();
            __builder2.AddMarkupContent(6, "\r\n        ");
            __builder2.OpenComponent<FocusOnNavigate>(7);
            __builder2.AddAttribute(8, "RouteData", RuntimeHelpers.TypeCheck(routeData));
            __builder2.AddAttribute(9, "Selector", "h1");
            __builder2.CloseComponent();
        }));
        __builder.AddAttribute(10, "NotFound", (RenderFragment)delegate(RenderTreeBuilder __builder2)
        {
            __builder2.OpenComponent<PageTitle>(11);
            __builder2.AddAttribute(12, "ChildContent", (RenderFragment)delegate(RenderTreeBuilder __builder3)
            {
                __builder3.AddContent(13, "Not found");
            });
            __builder2.CloseComponent();
            __builder2.AddMarkupContent(14, "\r\n        ");
            __builder2.OpenComponent<LayoutView>(15);
            __builder2.AddAttribute(16, "Layout", RuntimeHelpers.TypeCheck(typeof(MainLayout)));
            __builder2.AddAttribute(17, "ChildContent", (RenderFragment)delegate(RenderTreeBuilder __builder3)
            {
                __builder3.AddMarkupContent(18, "<p role=\"alert\">Sorry, there's nothing at this address.</p>");
            });
            __builder2.CloseComponent();
        });
        __builder.CloseComponent();
    }
}

public class MainLayout : LayoutComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder __builder)
    {
        __builder.OpenComponent<PageTitle>(0);
        __builder.AddAttribute(1, "ChildContent", (RenderFragment)delegate(RenderTreeBuilder __builder2)
        {
            __builder2.AddContent(2, "btest");
        });
        __builder.CloseComponent();
        __builder.AddMarkupContent(3, "\r\n\r\n");
        __builder.OpenElement(4, "div");
        __builder.AddAttribute(5, "class", "page");
        __builder.AddAttribute(6, "b-lu5l6b0u01");
        __builder.OpenElement(7, "div");
        __builder.AddAttribute(8, "class", "sidebar");
        __builder.AddAttribute(9, "b-lu5l6b0u01");
        __builder.OpenComponent<NavMenu>(10);
        __builder.CloseComponent();
        __builder.CloseElement();
        __builder.AddMarkupContent(11, "\r\n\r\n    ");
        __builder.OpenElement(12, "main");
        __builder.AddAttribute(13, "b-lu5l6b0u01");
        __builder.AddMarkupContent(14, "<div class=\"top-row px-4\" b-lu5l6b0u01><a href=\"https://docs.microsoft.com/aspnet/\" target=\"_blank\" b-lu5l6b0u01>About</a></div>\r\n\r\n        ");
        __builder.OpenElement(15, "article");
        __builder.AddAttribute(16, "class", "content px-4");
        __builder.AddAttribute(17, "b-lu5l6b0u01");
        __builder.AddContent(18, base.Body);
        __builder.CloseElement();
        __builder.CloseElement();
        __builder.CloseElement();
    }
}

public class NavMenu : ComponentBase
{
	private bool collapseNavMenu = true;

	private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

	protected override void BuildRenderTree(RenderTreeBuilder __builder)
	{
		__builder.OpenElement(0, "div");
		__builder.AddAttribute(1, "class", "top-row ps-3 navbar navbar-dark");
		__builder.AddAttribute(2, "b-clseustna0");
		__builder.OpenElement(3, "div");
		__builder.AddAttribute(4, "class", "container-fluid");
		__builder.AddAttribute(5, "b-clseustna0");
		__builder.AddMarkupContent(6, "<a class=\"navbar-brand\" href b-clseustna0>btest</a>\r\n        ");
		__builder.OpenElement(7, "button");
		__builder.AddAttribute(8, "title", "Navigation menu");
		__builder.AddAttribute(9, "class", "navbar-toggler");
		__builder.AddAttribute(10, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Action(ToggleNavMenu)));
		__builder.AddAttribute(11, "b-clseustna0");
		__builder.AddMarkupContent(12, "<span class=\"navbar-toggler-icon\" b-clseustna0></span>");
		__builder.CloseElement();
		__builder.CloseElement();
		__builder.CloseElement();
		__builder.AddMarkupContent(13, "\r\n\r\n");
		__builder.OpenElement(14, "div");
		__builder.AddAttribute(15, "class", NavMenuCssClass);
		__builder.AddAttribute(16, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, new Action(ToggleNavMenu)));
		__builder.AddAttribute(17, "b-clseustna0");
		__builder.OpenElement(18, "nav");
		__builder.AddAttribute(19, "class", "flex-column");
		__builder.AddAttribute(20, "b-clseustna0");
		__builder.OpenElement(21, "div");
		__builder.AddAttribute(22, "class", "nav-item px-3");
		__builder.AddAttribute(23, "b-clseustna0");
		__builder.OpenComponent<NavLink>(24);
		__builder.AddAttribute(25, "class", "nav-link");
		__builder.AddAttribute(26, "href", "");
		__builder.AddAttribute(27, "Match", RuntimeHelpers.TypeCheck(NavLinkMatch.All));
		__builder.AddAttribute(28, "ChildContent", (RenderFragment)delegate(RenderTreeBuilder __builder2)
		{
			__builder2.AddMarkupContent(29, "<span class=\"oi oi-home\" aria-hidden=\"true\" b-clseustna0></span> Home\r\n            ");
		});
		__builder.CloseComponent();
		__builder.CloseElement();
		__builder.AddMarkupContent(30, "\r\n        ");
		__builder.OpenElement(31, "div");
		__builder.AddAttribute(32, "class", "nav-item px-3");
		__builder.AddAttribute(33, "b-clseustna0");
		__builder.OpenComponent<NavLink>(34);
		__builder.AddAttribute(35, "class", "nav-link");
		__builder.AddAttribute(36, "href", "counter");
		__builder.AddAttribute(37, "ChildContent", (RenderFragment)delegate(RenderTreeBuilder __builder2)
		{
			__builder2.AddMarkupContent(38, "<span class=\"oi oi-plus\" aria-hidden=\"true\" b-clseustna0></span> Counter\r\n            ");
		});
		__builder.CloseComponent();
		__builder.CloseElement();
		__builder.AddMarkupContent(39, "\r\n        ");
		__builder.OpenElement(40, "div");
		__builder.AddAttribute(41, "class", "nav-item px-3");
		__builder.AddAttribute(42, "b-clseustna0");
		__builder.OpenComponent<NavLink>(43);
		__builder.AddAttribute(44, "class", "nav-link");
		__builder.AddAttribute(45, "href", "fetchdata");
		__builder.AddAttribute(46, "ChildContent", (RenderFragment)delegate(RenderTreeBuilder __builder2)
		{
			__builder2.AddMarkupContent(47, "<span class=\"oi oi-list-rich\" aria-hidden=\"true\" b-clseustna0></span> Fetch data\r\n            ");
		});
		__builder.CloseComponent();
		__builder.CloseElement();
		__builder.CloseElement();
		__builder.CloseElement();
	}

	private void ToggleNavMenu()
	{
		collapseNavMenu = !collapseNavMenu;
	}
}

