using Authzed.Api.V1;
using STS.Resources.API.Attributes;
using STS.Resources.Application.Interfaces;

namespace STS.Resources.Infrastructure.Services;

public class LeaguePermissionService : ILeaguePermissionService
{
    private readonly PermissionsService.PermissionsServiceClient  _permissionsClient;

    public LeaguePermissionService(PermissionsService.PermissionsServiceClient permissionsClient)
    {
        _permissionsClient = permissionsClient;
    }
    public async Task<bool> HasAccessAsync(string resourceId, string userId, AccessLevel accessLevel)
    {
        var request = new CheckPermissionRequest
        {
            Resource = new ObjectReference 
            { 
                ObjectType = "league_resource", 
                ObjectId = resourceId
            },

            Permission = accessLevel.ToString().ToLower(), 

            Subject = new SubjectReference
            {
                Object = new ObjectReference 
                { 
                    ObjectType = "user", 
                    ObjectId = userId
                }
            },

            Consistency = new Consistency { MinimizeLatency = true }
        };

        var response = await _permissionsClient.CheckPermissionAsync(request);

        return response.Permissionship == CheckPermissionResponse.Types.Permissionship.HasPermission;

    }

    public async Task AddAccessAsync(string league, string userId, AccessLevel accessLevel)
    {
        var grantOwnerRequest = new WriteRelationshipsRequest();
        grantOwnerRequest.Updates.Add(new RelationshipUpdate
        {
            Operation = RelationshipUpdate.Types.Operation.Touch, // Use 'Touch' to create or update
            Relationship = new Relationship
            {
                Resource = new ObjectReference
                {
                    ObjectType = "league", 
                    ObjectId = league
                },
                Relation = accessLevel.ToString().ToLower(),
                Subject = new SubjectReference 
                { 
                    Object = new ObjectReference
                    {
                        
                        ObjectType = "user", 
                        ObjectId = userId
                    } 
                }
            }
        });

        await _permissionsClient.WriteRelationshipsAsync(grantOwnerRequest);
    }

    public async Task AddResourceAsync(string leagueId, string resourceId)
    {
        var addResourceRequest = new WriteRelationshipsRequest();
        addResourceRequest.Updates.Add(new RelationshipUpdate
        {
            Operation = RelationshipUpdate.Types.Operation.Touch,
            Relationship = new Relationship
            {
                Resource = new ObjectReference
                {
                    ObjectType = "league_resource", 
                    ObjectId = resourceId
                },
        
                Relation = "res_league",
        
                Subject = new SubjectReference 
                { 
                    Object = new ObjectReference
                    {
                        ObjectType = "league", 
                        ObjectId = leagueId
                    } 
                }
            }
        });

        await _permissionsClient.WriteRelationshipsAsync(addResourceRequest);
    }
    
    public async Task RemoveAccessAsync(string league, string userId)
    {
        var deleteRequest = new DeleteRelationshipsRequest
        {
            RelationshipFilter = new RelationshipFilter
            {
                ResourceType = "league",
                OptionalResourceId = league,
                
                OptionalSubjectFilter = new SubjectFilter
                {
                    SubjectType = "user",
                    OptionalSubjectId = userId
                }
            }
        };

        await _permissionsClient.DeleteRelationshipsAsync(deleteRequest);
    }

    public async Task DeleteLeagueAsync(string leagueId)
    {
        var deleteUserRelations = new DeleteRelationshipsRequest
        {
            RelationshipFilter = new RelationshipFilter
            {
                ResourceType = "league",
                OptionalResourceId = leagueId 
            }
        };

        await _permissionsClient.DeleteRelationshipsAsync(deleteUserRelations);
    }

    public async Task DeleteResourceAsync(string leagueId, string resourceId)
    {
        var request = new WriteRelationshipsRequest();
        request.Updates.Add(new RelationshipUpdate
        {
            Operation = RelationshipUpdate.Types.Operation.Delete,
            Relationship = new Relationship
            {
                
                Resource = new ObjectReference
                {
                    ObjectType = "league_resource", 
                    ObjectId = resourceId
                },
        
                Relation = "res_league",
        
                Subject = new SubjectReference 
                { 
                    Object = new ObjectReference
                    {
                        ObjectType = "league", 
                        ObjectId = leagueId
                    } 
                }
            }
        });

        await _permissionsClient.WriteRelationshipsAsync(request);
    }
}