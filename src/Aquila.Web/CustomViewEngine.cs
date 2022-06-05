using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.ViewEngines;

public class CustomViewEngine : IRazorViewEngine
{
    public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
    {
        throw new NotImplementedException();
    }

    public ViewEngineResult GetView(string? executingFilePath, string viewPath, bool isMainPage)
    {
        throw new NotImplementedException();
    }

    public RazorPageResult FindPage(ActionContext context, string pageName)
    {
        throw new NotImplementedException();
    }

    public RazorPageResult GetPage(string executingFilePath, string pagePath)
    {
        throw new NotImplementedException();
    }

    public string? GetAbsolutePath(string? executingFilePath, string? pagePath)
    {
        throw new NotImplementedException();
    }
}


public class CustomFeatureProvider : IApplicationFeatureProvider<ViewsFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ViewsFeature feature)
    {
        foreach (var provider in parts.OfType<IRazorCompiledItemProvider>())
        {
        }
    }
}