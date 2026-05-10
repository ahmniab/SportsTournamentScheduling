using STS.Resources.API.Attributes;
using Authzed.Api.V1;
using Grpc.Core.Interceptors;
using Grpc.Core;
using STS.Resources.Application.Interfaces;

namespace STS.Resources.API.Interceptors;

public class ResourceGuardInterceptor: Interceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILeaguePermissionService _permissionsService;
    
    public ResourceGuardInterceptor(
        IHttpContextAccessor httpContextAccessor, 
        ILeaguePermissionService permissionsService
        )
    {
        _permissionsService = permissionsService;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var endpoint = httpContext?.GetEndpoint();
        var secureResourceAttribute = endpoint?.Metadata.GetMetadata<SecureResourceAttribute>();
        
        if (secureResourceAttribute == null)
        {
            return await continuation(request, context);
        }

        var user = httpContext?.User;
        var secureOwnerId = user?.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(secureOwnerId))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Missing ownership context."));
        }
        
        var idProperty = typeof(TRequest).GetProperty("Id");
        var resourceId = idProperty?.GetValue(request)?.ToString();
        if (string.IsNullOrEmpty(resourceId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Request must contain a valid Id."));
        }
        
        bool isTheOwner = await _permissionsService.HasAccessAsync(resourceId, secureOwnerId, secureResourceAttribute.Type);
        if (isTheOwner)
        {
            return await continuation(request, context);
        }
        throw new RpcException(new Status(StatusCode.PermissionDenied, "You don't have permission to perform this action."));

    }

    
    
}