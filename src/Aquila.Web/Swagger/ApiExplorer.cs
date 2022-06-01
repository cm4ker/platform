using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace Aquila.Web.Swagger;

public class AquilaApplicationModelProvider : IApiDescriptionProvider
{
    private readonly IModelMetadataProvider _modelMetadataProvider;
    private readonly AquilaApiHolder _holder;

    public AquilaApplicationModelProvider(IModelMetadataProvider modelMetadataProvider, 
        AquilaApiHolder holder)
    {
        _modelMetadataProvider = modelMetadataProvider;
        _holder = holder;
    }


    // Executes before MVC's DefaultApiDescriptionProvider and GrpcHttpApiDescriptionProvider for no particular reason.
    public int Order => -1100;


    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        foreach (var endpoint in _holder.Endpoints)
        {
            context.Results.Add(CreateApiDescription(endpoint.Key.methodName, "get", endpoint.Value));
        }
    }

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
        
    }

    private ApiDescription CreateApiDescription(string methodName, string httpMethod, MethodInfo methodInfo)
    {
        // Swashbuckle uses the "controller" name to group endpoints together.
        // For now, put all methods defined the same declaring type together.
        string controllerName = "applicationController";


        var apiDescription = new ApiDescription
        {
            HttpMethod = httpMethod,
            GroupName = null,
            RelativePath = $"/{methodName}",
            ActionDescriptor = new ActionDescriptor
            {
                DisplayName = methodName,
                RouteValues =
                {
                    ["controller"] = controllerName,
                },
            },
        };

        foreach (var parameter in methodInfo.GetParameters())
        {
            var parameterDescription = CreateApiParameterDescription(parameter, null);

            if (parameterDescription is null)
            {
                //continue;
                
                parameterDescription = new ApiParameterDescription
                {
                    Name = parameter.Name,
                    ModelMetadata = CreateModelMetadata(parameter.ParameterType),
                    Source = BindingSource.Body,
                    Type =  typeof(int),
                    IsRequired = true,
                };
            }

            
            
            
            apiDescription.ParameterDescriptions.Add(parameterDescription);
        }

        // // Get IAcceptsMetadata.
        // var acceptsMetadata = routeEndpoint.Metadata.GetMetadata<IAcceptsMetadata>();
        // if (acceptsMetadata is not null)
        // {
        //     var acceptsRequestType = acceptsMetadata.RequestType;
        //     var isOptional = acceptsMetadata.IsOptional;
        //     var parameterDescription = new ApiParameterDescription
        //     {
        //         Name = acceptsRequestType is not null ? acceptsRequestType.Name : typeof(void).Name,
        //         ModelMetadata = CreateModelMetadata(acceptsRequestType ?? typeof(void)),
        //         Source = BindingSource.Body,
        //         Type = acceptsRequestType ?? typeof(void),
        //         IsRequired = !isOptional,
        //     };
        //     apiDescription.ParameterDescriptions.Add(parameterDescription);
        //
        //     var supportedRequestFormats = apiDescription.SupportedRequestFormats;
        //
        //     foreach (var contentType in acceptsMetadata.ContentTypes)
        //     {
        //         supportedRequestFormats.Add(new ApiRequestFormat
        //         {
        //             MediaType = contentType
        //         });
        //     }
        //}
        
        // AddSupportedResponseTypes(apiDescription.SupportedResponseTypes, methodInfo.ReturnType, routeEndpoint.Metadata);
        // AddActionDescriptorEndpointMetadata(apiDescription.ActionDescriptor, routeEndpoint.Metadata);

        return apiDescription;
    }

    private ApiParameterDescription? CreateApiParameterDescription(ParameterInfo parameter, RoutePattern pattern)
    {
        var (source, name, allowEmpty, paramType) = GetBindingSourceAndName(parameter, pattern);

        // Services are ignored because they are not request parameters.
        // We ignore/skip body parameter because the value will be retrieved from the IAcceptsMetadata.
        if (source == BindingSource.Services || source == BindingSource.Body)
        {
            return null;
        }

        // Determine the "requiredness" based on nullability, default value or if allowEmpty is set
        var nullabilityContext = new NullabilityInfoContext();
        var nullability = nullabilityContext.Create(parameter);
        var isOptional = parameter.HasDefaultValue || nullability.ReadState != NullabilityState.NotNull || allowEmpty;
        var parameterDescriptor = CreateParameterDescriptor(parameter);

        return new ApiParameterDescription
        {
            Name = name,
            ModelMetadata = CreateModelMetadata(paramType),
            Source = source,
            DefaultValue = parameter.DefaultValue,
            Type = parameter.ParameterType,
            IsRequired = !isOptional,
            ParameterDescriptor = parameterDescriptor
        };
    }

    internal sealed class EndpointParameterDescriptor : ParameterDescriptor, IParameterInfoParameterDescriptor
    {
        public ParameterInfo ParameterInfo { get; set; } = default!;
    }

    private static ParameterDescriptor CreateParameterDescriptor(ParameterInfo parameter)
        => new EndpointParameterDescriptor
        {
            Name = parameter.Name ?? string.Empty,
            ParameterInfo = parameter,
            ParameterType = parameter.ParameterType,
        };

    // TODO: Share more of this logic with RequestDelegateFactory.CreateArgument(...) using RequestDelegateFactoryUtilities
    // which is shared source.
    private (BindingSource, string, bool, Type) GetBindingSourceAndName(ParameterInfo parameter, RoutePattern pattern)
    {
        var attributes = parameter.GetCustomAttributes();

        if (attributes.OfType<IFromRouteMetadata>().FirstOrDefault() is { } routeAttribute)
        {
            return (BindingSource.Path, routeAttribute.Name ?? parameter.Name ?? string.Empty, false,
                parameter.ParameterType);
        }
        else if (attributes.OfType<IFromQueryMetadata>().FirstOrDefault() is { } queryAttribute)
        {
            return (BindingSource.Query, queryAttribute.Name ?? parameter.Name ?? string.Empty, false,
                parameter.ParameterType);
        }
        else if (attributes.OfType<IFromHeaderMetadata>().FirstOrDefault() is { } headerAttribute)
        {
            return (BindingSource.Header, headerAttribute.Name ?? parameter.Name ?? string.Empty, false,
                parameter.ParameterType);
        }
        else if (attributes.OfType<IFromBodyMetadata>().FirstOrDefault() is { } fromBodyAttribute)
        {
            return (BindingSource.Body, parameter.Name ?? string.Empty, fromBodyAttribute.AllowEmpty,
                parameter.ParameterType);
        }
        // else if (parameter.CustomAttributes.Any(a => typeof(IFromServiceMetadata).IsAssignableFrom(a.AttributeType)) ||
        //          parameter.ParameterType == typeof(HttpContext) ||
        //          parameter.ParameterType == typeof(HttpRequest) ||
        //          parameter.ParameterType == typeof(HttpResponse) ||
        //          parameter.ParameterType == typeof(ClaimsPrincipal) ||
        //          parameter.ParameterType == typeof(CancellationToken) ||
        //          //ParameterBindingMethodCache.HasBindAsyncMethod(parameter) ||
        //          _serviceProviderIsService?.IsService(parameter.ParameterType) == true)
        // {
        //     return (BindingSource.Services, parameter.Name ?? string.Empty, false, parameter.ParameterType);
        // }
        // else if (parameter.ParameterType == typeof(string) || ParameterBindingMethodCache.HasTryParseMethod(parameter))
        // {
        //     // complex types will display as strings since they use custom parsing via TryParse on a string
        //     var displayType = !parameter.ParameterType.IsPrimitive &&
        //                       Nullable.GetUnderlyingType(parameter.ParameterType)?.IsPrimitive != true
        //         ? typeof(string)
        //         : parameter.ParameterType;
        //     // Path vs query cannot be determined by RequestDelegateFactory at startup currently because of the layering, but can be done here.
        //     if (parameter.Name is { } name && pattern.GetParameter(name) is not null)
        //     {
        //         return (BindingSource.Path, name, false, displayType);
        //     }
        //     else
        //     {
        //         return (BindingSource.Query, parameter.Name ?? string.Empty, false, displayType);
        //     }
        // }
        else
        {
            return (BindingSource.Body, parameter.Name ?? string.Empty, false, parameter.ParameterType);
        }
    }

    private static void AddSupportedResponseTypes(
        IList<ApiResponseType> supportedResponseTypes,
        Type returnType,
        EndpointMetadataCollection endpointMetadata)
    {
        var responseType = returnType;

        // if (AwaitableInfo.IsTypeAwaitable(responseType, out var awaitableInfo))
        // {
        //     responseType = awaitableInfo.ResultType;
        // }

        // Can't determine anything about IResults yet that's not from extra metadata. IResult<T> could help here.
        if (typeof(IResult).IsAssignableFrom(responseType))
        {
            responseType = typeof(void);
        }

        // We support attributes (which implement the IApiResponseMetadataProvider) interface
        // and types added via the extension methods (which implement IProducesResponseTypeMetadata).
        var responseProviderMetadata = endpointMetadata.GetOrderedMetadata<IApiResponseMetadataProvider>();
        var producesResponseMetadata = endpointMetadata.GetOrderedMetadata<IProducesResponseTypeMetadata>();
        var errorMetadata = endpointMetadata.GetMetadata<ProducesErrorResponseTypeAttribute>();
        var defaultErrorType = errorMetadata?.Type ?? typeof(void);
        var contentTypes = new MediaTypeCollection();

        var responseProviderMetadataTypes = ApiResponseTypeProvider.ReadResponseMetadata(
            responseProviderMetadata, responseType, defaultErrorType, contentTypes);
        var producesResponseMetadataTypes = ReadResponseMetadata(producesResponseMetadata, responseType);

        // We favor types added via the extension methods (which implements IProducesResponseTypeMetadata)
        // over those that are added via attributes.
        var responseMetadataTypes = producesResponseMetadataTypes.Values.Concat(responseProviderMetadataTypes);

        if (responseMetadataTypes.Any())
        {
            foreach (var apiResponseType in responseMetadataTypes)
            {
                // void means no response type was specified by the metadata, so use whatever we inferred.
                // ApiResponseTypeProvider should never return ApiResponseTypes with null Type, but it doesn't hurt to check.
                if (apiResponseType.Type is null || apiResponseType.Type == typeof(void))
                {
                    apiResponseType.Type = responseType;
                }

                apiResponseType.ModelMetadata = CreateModelMetadata(apiResponseType.Type);

                if (contentTypes.Count > 0)
                {
                    AddResponseContentTypes(apiResponseType.ApiResponseFormats, contentTypes);
                }
                // Only set the default response type if it hasn't already been set via a
                // ProducesResponseTypeAttribute.
                else if (apiResponseType.ApiResponseFormats.Count == 0 &&
                         CreateDefaultApiResponseFormat(apiResponseType.Type) is { } defaultResponseFormat)
                {
                    apiResponseType.ApiResponseFormats.Add(defaultResponseFormat);
                }

                if (!supportedResponseTypes.Any(existingResponseType =>
                        existingResponseType.StatusCode == apiResponseType.StatusCode))
                {
                    supportedResponseTypes.Add(apiResponseType);
                }
            }
        }
        else
        {
            // Set the default response type only when none has already been set explicitly with metadata.
            var defaultApiResponseType = CreateDefaultApiResponseType(responseType);

            if (contentTypes.Count > 0)
            {
                // If metadata provided us with response formats, use that instead of the default.
                defaultApiResponseType.ApiResponseFormats.Clear();
                AddResponseContentTypes(defaultApiResponseType.ApiResponseFormats, contentTypes);
            }

            supportedResponseTypes.Add(defaultApiResponseType);
        }
    }

    private static Dictionary<int, ApiResponseType> ReadResponseMetadata(
        IReadOnlyList<IProducesResponseTypeMetadata> responseMetadata,
        Type? type)
    {
        var results = new Dictionary<int, ApiResponseType>();

        foreach (var metadata in responseMetadata)
        {
            var statusCode = metadata.StatusCode;

            var apiResponseType = new ApiResponseType
            {
                Type = metadata.Type,
                StatusCode = statusCode,
            };

            if (apiResponseType.Type == typeof(void))
            {
                if (type != null &&
                    (statusCode == StatusCodes.Status200OK || statusCode == StatusCodes.Status201Created))
                {
                    // Allow setting the response type from the return type of the method if it has
                    // not been set explicitly by the method.
                    apiResponseType.Type = type;
                }
            }

            var attributeContentTypes = new MediaTypeCollection();
            if (metadata.ContentTypes != null)
            {
                foreach (var contentType in metadata.ContentTypes)
                {
                    attributeContentTypes.Add(contentType);
                }
            }

            ApiResponseTypeProvider.CalculateResponseFormatForType(apiResponseType, attributeContentTypes,
                responseTypeMetadataProviders: null, modelMetadataProvider: null);

            if (apiResponseType.Type != null)
            {
                results[apiResponseType.StatusCode] = apiResponseType;
            }
        }

        return results;
    }

    private static ApiResponseType CreateDefaultApiResponseType(Type responseType)
    {
        var apiResponseType = new ApiResponseType
        {
            ModelMetadata = CreateModelMetadata(responseType),
            StatusCode = 200,
            Type = responseType,
        };

        if (CreateDefaultApiResponseFormat(responseType) is { } responseFormat)
        {
            apiResponseType.ApiResponseFormats.Add(responseFormat);
        }

        return apiResponseType;
    }

    private static ApiResponseFormat? CreateDefaultApiResponseFormat(Type responseType)
    {
        if (responseType == typeof(void))
        {
            return null;
        }
        else if (responseType == typeof(string))
        {
            // This uses HttpResponse.WriteAsync(string) method which doesn't set a content type. It could be anything,
            // but I think "text/plain" is a reasonable assumption if nothing else is specified with metadata.
            return new ApiResponseFormat { MediaType = "text/plain" };
        }
        else
        {
            // Everything else is written using HttpResponse.WriteAsJsonAsync<TValue>(T).
            return new ApiResponseFormat { MediaType = "application/json" };
        }
    }

    private static EndpointModelMetadata CreateModelMetadata(Type type) =>
        new(ModelMetadataIdentity.ForType(type));

    private static void AddResponseContentTypes(IList<ApiResponseFormat> apiResponseFormats,
        IReadOnlyList<string> contentTypes)
    {
        foreach (var contentType in contentTypes)
        {
            apiResponseFormats.Add(new ApiResponseFormat
            {
                MediaType = contentType,
            });
        }
    }

    private static void AddActionDescriptorEndpointMetadata(
        ActionDescriptor actionDescriptor,
        EndpointMetadataCollection endpointMetadata)
    {
        if (endpointMetadata.Count > 0)
        {
            // ActionDescriptor.EndpointMetadata is an empty array by
            // default so need to add the metadata into a new list.
            actionDescriptor.EndpointMetadata = new List<object>(endpointMetadata);
        }
    }
}

internal class ApiResponseTypeProvider
{
    private readonly IModelMetadataProvider _modelMetadataProvider;
    private readonly IActionResultTypeMapper _mapper;
    private readonly MvcOptions _mvcOptions;

    public ApiResponseTypeProvider(
        IModelMetadataProvider modelMetadataProvider,
        IActionResultTypeMapper mapper,
        MvcOptions mvcOptions)
    {
        _modelMetadataProvider = modelMetadataProvider;
        _mapper = mapper;
        _mvcOptions = mvcOptions;
    }

    public ICollection<ApiResponseType> GetApiResponseTypes(ControllerActionDescriptor action)
    {
        // We only provide response info if we can figure out a type that is a user-data type.
        // Void /Task object/IActionResult will result in no data.
        var declaredReturnType = GetDeclaredReturnType(action);

        var runtimeReturnType = GetRuntimeReturnType(declaredReturnType);

        var responseMetadataAttributes = GetResponseMetadataAttributes(action);
        if (!HasSignificantMetadataProvider(responseMetadataAttributes) &&
            action.Properties.TryGetValue(typeof(ApiConventionResult), out var result))
        {
            // Action does not have any conventions. Use conventions on it if present.
            var apiConventionResult = (ApiConventionResult)result!;
            responseMetadataAttributes.AddRange(apiConventionResult.ResponseMetadataProviders);
        }

        var defaultErrorType = typeof(void);
        if (action.Properties.TryGetValue(typeof(ProducesErrorResponseTypeAttribute), out result))
        {
            defaultErrorType = ((ProducesErrorResponseTypeAttribute)result!).Type;
        }

        var apiResponseTypes = GetApiResponseTypes(responseMetadataAttributes, runtimeReturnType, defaultErrorType);
        return apiResponseTypes;
    }

    private static List<IApiResponseMetadataProvider> GetResponseMetadataAttributes(ControllerActionDescriptor action)
    {
        if (action.FilterDescriptors == null)
        {
            return new List<IApiResponseMetadataProvider>();
        }

        // This technique for enumerating filters will intentionally ignore any filter that is an IFilterFactory
        // while searching for a filter that implements IApiResponseMetadataProvider.
        //
        // The workaround for that is to implement the metadata interface on the IFilterFactory.
        return action.FilterDescriptors
            .Select(fd => fd.Filter)
            .OfType<IApiResponseMetadataProvider>()
            .ToList();
    }

    private ICollection<ApiResponseType> GetApiResponseTypes(
        IReadOnlyList<IApiResponseMetadataProvider> responseMetadataAttributes,
        Type? type,
        Type defaultErrorType)
    {
        var contentTypes = new MediaTypeCollection();
        var responseTypeMetadataProviders = _mvcOptions.OutputFormatters.OfType<IApiResponseTypeMetadataProvider>();

        var responseTypes = ReadResponseMetadata(
            responseMetadataAttributes,
            type,
            defaultErrorType,
            contentTypes,
            responseTypeMetadataProviders);

        // Set the default status only when no status has already been set explicitly
        if (responseTypes.Count == 0 && type != null)
        {
            responseTypes.Add(new ApiResponseType
            {
                StatusCode = StatusCodes.Status200OK,
                Type = type,
            });
        }

        if (contentTypes.Count == 0)
        {
            // None of the IApiResponseMetadataProvider specified a content type. This is common for actions that
            // specify one or more ProducesResponseType but no ProducesAttribute. In this case, formatters will participate in conneg
            // and respond to the incoming request.
            // Querying IApiResponseTypeMetadataProvider.GetSupportedContentTypes with "null" should retrieve all supported
            // content types that each formatter may respond in.
            contentTypes.Add((string)null!);
        }

        foreach (var apiResponse in responseTypes)
        {
            CalculateResponseFormatForType(apiResponse, contentTypes, responseTypeMetadataProviders,
                _modelMetadataProvider);
        }

        return responseTypes;
    }

    // Shared with EndpointMetadataApiDescriptionProvider
    internal static List<ApiResponseType> ReadResponseMetadata(
        IReadOnlyList<IApiResponseMetadataProvider> responseMetadataAttributes,
        Type? type,
        Type defaultErrorType,
        MediaTypeCollection contentTypes,
        IEnumerable<IApiResponseTypeMetadataProvider>? responseTypeMetadataProviders = null,
        IModelMetadataProvider? modelMetadataProvider = null)
    {
        var results = new Dictionary<int, ApiResponseType>();

        // Get the content type that the action explicitly set to support.
        // Walk through all 'filter' attributes in order, and allow each one to see or override
        // the results of the previous ones. This is similar to the execution path for content-negotiation.
        if (responseMetadataAttributes != null)
        {
            foreach (var metadataAttribute in responseMetadataAttributes)
            {
                // All ProducesXAttributes, except for ProducesResponseTypeAttribute do
                // not allow multiple instances on the same method/class/etc. For those
                // scenarios, the `SetContentTypes` method on the attribute continuously
                // clears out more general content types in favor of more specific ones
                // since we iterate through the attributes in order. For example, if a
                // Produces exists on both a controller and an action within the controller,
                // we favor the definition in the action. This is a semantic that does not
                // apply to ProducesResponseType, which allows multiple instances on an target.
                if (metadataAttribute is not ProducesResponseTypeAttribute)
                {
                    metadataAttribute.SetContentTypes(contentTypes);
                }

                var statusCode = metadataAttribute.StatusCode;

                var apiResponseType = new ApiResponseType
                {
                    Type = metadataAttribute.Type,
                    StatusCode = statusCode,
                    IsDefaultResponse = metadataAttribute is IApiDefaultResponseMetadataProvider,
                };

                if (apiResponseType.Type == typeof(void))
                {
                    if (type != null && (statusCode == StatusCodes.Status200OK ||
                                         statusCode == StatusCodes.Status201Created))
                    {
                        // ProducesResponseTypeAttribute's constructor defaults to setting "Type" to void when no value is specified.
                        // In this event, use the action's return type for 200 or 201 status codes. This lets you decorate an action with a
                        // [ProducesResponseType(201)] instead of [ProducesResponseType(typeof(Person), 201] when typeof(Person) can be inferred
                        // from the return type.
                        apiResponseType.Type = type;
                    }
                    else if (IsClientError(statusCode))
                    {
                        // Determine whether or not the type was provided by the user. If so, favor it over the default
                        // error type for 4xx client errors if no response type is specified..
                        var setByDefault = metadataAttribute is ProducesResponseTypeAttribute { };
                        apiResponseType.Type = setByDefault ? defaultErrorType : apiResponseType.Type;
                    }
                    else if (apiResponseType.IsDefaultResponse)
                    {
                        apiResponseType.Type = defaultErrorType;
                    }
                }

                // We special case the handling of ProcuesResponseTypeAttributes since
                // multiple ProducesResponseTypeAttributes are permitted on a single
                // action/controller/etc. In that scenario, instead of picking the most-specific
                // set of content types (like we do with the Produces attribute above) we process
                // the content types for each attribute independently.
                if (metadataAttribute is ProducesResponseTypeAttribute)
                {
                    var attributeContentTypes = new MediaTypeCollection();
                    metadataAttribute.SetContentTypes(attributeContentTypes);
                    CalculateResponseFormatForType(apiResponseType, attributeContentTypes,
                        responseTypeMetadataProviders, modelMetadataProvider);
                }

                if (apiResponseType.Type != null)
                {
                    results[apiResponseType.StatusCode] = apiResponseType;
                }
            }
        }

        return results.Values.ToList();
    }

    // Shared with EndpointMetadataApiDescriptionProvider
    internal static void CalculateResponseFormatForType(ApiResponseType apiResponse,
        MediaTypeCollection declaredContentTypes,
        IEnumerable<IApiResponseTypeMetadataProvider>? responseTypeMetadataProviders,
        IModelMetadataProvider? modelMetadataProvider)
    {
        // If response formats have already been calculate for this type,
        // then exit early. This avoids populating the ApiResponseFormat for
        // types that have already been handled, specifically ProducesResponseTypes.
        if (apiResponse.ApiResponseFormats.Count > 0)
        {
            return;
        }

        // Given the content-types that were declared for this action, determine the formatters that support the content-type for the given
        // response type.
        // 1. Responses that do not specify an type do not have any associated content-type. This usually is meant for status-code only responses such
        // as return NotFound();
        // 2. When a type is specified, use GetSupportedContentTypes to expand wildcards and get the range of content-types formatters support.
        // 3. When no formatter supports the specified content-type, use the user specified value as is. This is useful in actions where the user
        // dictates the content-type.
        // e.g. [Produces("application/pdf")] Action() => FileStream("somefile.pdf", "application/pdf");
        var responseType = apiResponse.Type;
        if (responseType == null || responseType == typeof(void))
        {
            return;
        }

        apiResponse.ModelMetadata = modelMetadataProvider?.GetMetadataForType(responseType);

        foreach (var contentType in declaredContentTypes)
        {
            var isSupportedContentType = false;

            if (responseTypeMetadataProviders != null)
            {
                foreach (var responseTypeMetadataProvider in responseTypeMetadataProviders)
                {
                    var formatterSupportedContentTypes = responseTypeMetadataProvider.GetSupportedContentTypes(
                        contentType,
                        responseType);

                    if (formatterSupportedContentTypes == null)
                    {
                        continue;
                    }

                    isSupportedContentType = true;

                    foreach (var formatterSupportedContentType in formatterSupportedContentTypes)
                    {
                        apiResponse.ApiResponseFormats.Add(new ApiResponseFormat
                        {
                            Formatter = (IOutputFormatter)responseTypeMetadataProvider,
                            MediaType = formatterSupportedContentType,
                        });
                    }
                }
            }


            if (!isSupportedContentType && contentType != null)
            {
                // No output formatter was found that supports this content type. Add the user specified content type as-is to the result.
                apiResponse.ApiResponseFormats.Add(new ApiResponseFormat
                {
                    MediaType = contentType,
                });
            }
        }
    }

    private Type? GetDeclaredReturnType(ControllerActionDescriptor action)
    {
        var declaredReturnType = action.MethodInfo.ReturnType;
        if (declaredReturnType == typeof(void) ||
            declaredReturnType == typeof(Task) ||
            declaredReturnType == typeof(ValueTask))
        {
            return typeof(void);
        }

        // Unwrap the type if it's a Task<T>. The Task (non-generic) case was already handled.
        var unwrappedType = declaredReturnType;
        if (declaredReturnType.IsGenericType &&
            (declaredReturnType.GetGenericTypeDefinition() == typeof(Task<>) ||
             declaredReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>)))
        {
            unwrappedType = declaredReturnType.GetGenericArguments()[0];
        }

        // If the method is declared to return IActionResult or a derived class, that information
        // isn't valuable to the formatter.
        if (typeof(IActionResult).IsAssignableFrom(unwrappedType))
        {
            return null;
        }

        // If we get here, the type should be a user-defined data type or an envelope type
        // like ActionResult<T>. The mapper service will unwrap envelopes.
        unwrappedType = _mapper.GetResultDataType(unwrappedType);
        return unwrappedType;
    }

    private Type? GetRuntimeReturnType(Type? declaredReturnType)
    {
        // If we get here, then a filter didn't give us an answer, so we need to figure out if we
        // want to use the declared return type.
        //
        // We've already excluded Task, void, and IActionResult at this point.
        //
        // If the action might return any object, then assume we don't know anything about it.
        if (declaredReturnType == typeof(object))
        {
            return null;
        }

        return declaredReturnType;
    }

    private static bool IsClientError(int statusCode)
    {
        return statusCode >= 400 && statusCode < 500;
    }

    private static bool HasSignificantMetadataProvider(IReadOnlyList<IApiResponseMetadataProvider> providers)
    {
        for (var i = 0; i < providers.Count; i++)
        {
            var provider = providers[i];

            if (provider is ProducesAttribute producesAttribute && producesAttribute.Type is null)
            {
                // ProducesAttribute that does not specify type is considered not significant.
                continue;
            }

            // Any other IApiResponseMetadataProvider is considered significant
            return true;
        }

        return false;
    }
}

internal class EndpointModelMetadata : ModelMetadata
{
    public EndpointModelMetadata(ModelMetadataIdentity identity) : base(identity)
    {
        IsBindingAllowed = true;
    }

    public override IReadOnlyDictionary<object, object> AdditionalValues { get; } =
        ImmutableDictionary<object, object>.Empty;

    public override string? BinderModelName { get; }
    public override Type? BinderType { get; }
    public override BindingSource? BindingSource { get; }
    public override bool ConvertEmptyStringToNull { get; }
    public override string? DataTypeName { get; }
    public override string? Description { get; }
    public override string? DisplayFormatString { get; }
    public override string? DisplayName { get; }
    public override string? EditFormatString { get; }
    public override ModelMetadata? ElementMetadata { get; }
    public override IEnumerable<KeyValuePair<EnumGroupAndName, string>>? EnumGroupedDisplayNamesAndValues { get; }
    public override IReadOnlyDictionary<string, string>? EnumNamesAndValues { get; }
    public override bool HasNonDefaultEditFormat { get; }
    public override bool HideSurroundingHtml { get; }
    public override bool HtmlEncode { get; }
    public override bool IsBindingAllowed { get; }
    public override bool IsBindingRequired { get; }
    public override bool IsEnum { get; }
    public override bool IsFlagsEnum { get; }
    public override bool IsReadOnly { get; }
    public override bool IsRequired { get; }

    public override ModelBindingMessageProvider ModelBindingMessageProvider { get; } =
        new DefaultModelBindingMessageProvider();

    public override string? NullDisplayText { get; }
    public override int Order { get; }
    public override string? Placeholder { get; }
    public override ModelPropertyCollection Properties { get; } = new(Enumerable.Empty<ModelMetadata>());
    public override IPropertyFilterProvider? PropertyFilterProvider { get; }
    public override Func<object, object>? PropertyGetter { get; }
    public override Action<object, object?>? PropertySetter { get; }
    public override bool ShowForDisplay { get; }
    public override bool ShowForEdit { get; }
    public override string? SimpleDisplayProperty { get; }
    public override string? TemplateHint { get; }
    public override bool ValidateChildren { get; }
    public override IReadOnlyList<object> ValidatorMetadata { get; } = Array.Empty<object>();
}