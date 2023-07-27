namespace DependencyInjection.Extensions.AutoService;

/// <summary>
/// Enumeration for handling implementing class as service
/// </summary>
public enum SelfImplementationUsage
{
    /// <summary>
    /// Adds implementing class as service.
    /// </summary>
    AddSelfImplementation,

    /// <summary>
    /// Adds implementing class as service only when no other service is auto registered on implementing class.
    /// </summary>
    WhenNoServicePresent,

    /// <summary>
    /// Implementing class is never added as service.
    /// </summary>
    Disabled
}