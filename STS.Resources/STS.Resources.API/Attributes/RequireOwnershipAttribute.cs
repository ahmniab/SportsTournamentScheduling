namespace STS.Resources.API.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RequireOwnershipAttribute : Attribute
{
    public ResourceType Type { get; }
    public RequireOwnershipAttribute(ResourceType type)
    {
        Type = type;
    }
}