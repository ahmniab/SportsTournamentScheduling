namespace STS.Resources.API.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class SecureResourceAttribute : Attribute
{
    public AccessLevel Type { get; }
    public SecureResourceAttribute(AccessLevel type)
    {
        Type = type;
    }
}