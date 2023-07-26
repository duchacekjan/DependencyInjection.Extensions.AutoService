using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Extensions.AutoService;

/// <summary>
/// Attribute allows auto register service implementation.
/// Decorated class has to implement <see cref="ServiceType"/> and should not be used on abstract classes.
/// If not <see cref="ServiceType"/> defined, it is used decorated class as type of service.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class AutoServiceAttribute : Attribute
{
    /// <summary>
    /// Type of service.
    /// If not defined, it is used decorated class as type of service.
    /// </summary>
    public Type? ServiceType { get; }

    /// <summary>
    /// Service lifetime.
    /// </summary>
    public ServiceLifetime ServiceLifetime { get; }

    /// <summary>
    /// Attribute allows auto register service implementation.
    /// Decorated class has to implement <paramref name="serviceType"/> and should not be used on abstract classes.
    /// If <paramref name="serviceType"/> is <see langword="null"/>, it is used decorated class as type of service.
    /// </summary>
    /// <param name="serviceType">Type of service. Default <see langword="null"/></param>
    /// <param name="serviceLifetime">Service lifetime. Default <see cref="ServiceLifetime.Transient"/></param>
    public AutoServiceAttribute(Type? serviceType = null, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        ServiceType = serviceType;
        ServiceLifetime = serviceLifetime;
    }
}