using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing.Template;

namespace Aquila.WebServiceCore.Swagger;

public static class GeneratorExtensions
{
    internal static bool IsDefaultResponse(this ApiResponseType apiResponseType)
    {
        var propertyInfo = apiResponseType.GetType().GetProperty("IsDefaultResponse");
        if (propertyInfo != null)
        {
            return (bool)propertyInfo.GetValue(apiResponseType);
        }

        // ApiExplorer < 2.1.0 does not support default response.
        return false;
    }

    internal static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        var cameCasedParts = value.Split('.')
            .Select(part => char.ToLowerInvariant(part[0]) + part.Substring(1));

        return string.Join(".", cameCasedParts);
    }

    internal static string RelativePathSansParameterConstraints(this ApiDescription apiDescription)
    {
        var routeTemplate = TemplateParser.Parse(apiDescription.RelativePath);
        var sanitizedSegments = routeTemplate
            .Segments
            .Select(s => string.Concat(s.Parts.Select(p => p.Name != null ? $"{{{p.Name}}}" : p.Text)));
        return string.Join("/", sanitizedSegments);
    }

    internal static bool IsFromBody(this ApiParameterDescription apiParameter)
    {
        return (apiParameter.Source == BindingSource.Body);
    }

    internal static bool IsFromForm(this ApiParameterDescription apiParameter)
    {
        var source = apiParameter.Source;
        var elementType = apiParameter.ModelMetadata?.ElementType;

        return (source == BindingSource.Form || source == BindingSource.FormFile)
               || (elementType != null && typeof(IFormFile).IsAssignableFrom(elementType));
    }
}