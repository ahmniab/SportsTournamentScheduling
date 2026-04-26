using Microsoft.AspNetCore.Http;
using STS.Resources.API.Attributes;
using System.Reflection;
using Grpc.Core.Interceptors;
using Grpc.Core;
using STS.Resources.Application.Interfaces;

namespace STS.Resources.API.Interceptors;

public class OwnershipInterceptor: Interceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILeagueService _leagueService;
    private readonly ITeamService _teamService;
    private readonly IStadiumService _stadiumService;
    private readonly ITimeSlotService _timeSlotService;
    
    public OwnershipInterceptor(
        IHttpContextAccessor httpContextAccessor, 
        ILeagueService leagueService,
        ITeamService teamService,
        IStadiumService stadiumService,
        ITimeSlotService timeSlotService
        )
    {
        _httpContextAccessor = httpContextAccessor;
        _leagueService = leagueService;
        _stadiumService = stadiumService;
        _teamService = teamService;
        _timeSlotService = timeSlotService;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var endpoint = httpContext?.GetEndpoint();
        var requireOwnershipAttribute = endpoint?.Metadata.GetMetadata<RequireOwnershipAttribute>();
        
        if (requireOwnershipAttribute == null)
        {
            return await continuation(request, context);
        }
        
        var secureOwnerId = context.RequestHeaders.FirstOrDefault(h => h.Key == "x-owner-id")?.Value;
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


        try{
            bool isTheOwner = await VerifyOwnershipAsync(resourceId, secureOwnerId, requireOwnershipAttribute.Type);
            if (isTheOwner)
            {
                return await continuation(request, context);
            }
            throw new RpcException(new Status(StatusCode.PermissionDenied, "You don't have permission to perform this action."));
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }

    }

    private async Task<bool> VerifyOwnershipAsync(
        string resourceId, 
        string ownerId, 
        ResourceType resourceType
        )
    {
        switch (resourceType)
        {
            case ResourceType.League:
                return await _leagueService.VerifyOwnershipAsync(resourceId, ownerId);
            
            case ResourceType.Team:
                return await _teamService.VerifyOwnershipAsync(resourceId, ownerId);
            
            case ResourceType.Stadium:
                return await _stadiumService.VerifyOwnershipAsync(resourceId, ownerId);
            
            case ResourceType.TimeSlot:
                return await _timeSlotService.VerifyOwnershipAsync(resourceId, ownerId);
            default:
                return false;
        }
    }
    
    
    
}