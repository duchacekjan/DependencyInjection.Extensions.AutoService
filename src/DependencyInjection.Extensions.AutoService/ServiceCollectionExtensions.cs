using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Extensions.AutoService;

/// <summary>
/// Extensions for registering auto services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all services implementations decorated with <see cref="AutoServiceAttribute"/> from
    /// calling assembly.
    /// </summary>
    /// <param name="services">Collection of services</param>
    /// <returns></returns>
    public static IServiceCollection AddAutoServices(this IServiceCollection services)
        => services.AddAutoServices(new[] { Assembly.GetCallingAssembly() });

    /// <summary>
    /// Registers all services implementations decorated with <see cref="AutoServiceAttribute"/> from
    /// given assembly.
    /// </summary>
    /// <param name="services">Collection of services</param>
    /// <param name="assembly">Assembly containing services implementations</param>
    /// <returns></returns>
    public static IServiceCollection AddAutoServices(this IServiceCollection services, Assembly assembly)
        => services.AddAutoServices(new[] { assembly });

    /// <summary>
    /// Registers all services implementations decorated with <see cref="AutoServiceAttribute"/> from
    /// given assemblies.
    /// </summary>
    /// <param name="services">Collection of services.</param>
    /// <param name="assemblies">Assemblies containing services implementations.</param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">When Abstract class is decorated with <see cref="AutoServiceAttribute"/>.</exception>
    /// <exception cref="NotImplementingServiceTypeException">When decorated class does not implement service type.</exception>
    public static IServiceCollection AddAutoServices(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        => services.AddAutoServices(assemblies, null);

    /// <summary>
    /// Registers all services implementations decorated with <see cref="AutoServiceAttribute"/> from
    /// given assemblies.
    /// </summary>
    /// <param name="services">Collection of services.</param>
    /// <param name="assemblies">Assemblies containing services implementations.</param>
    /// <param name="excludeCondition">Exclude condition for testing purposes.</param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">When Abstract class is decorated with <see cref="AutoServiceAttribute"/>.</exception>
    /// <exception cref="NotImplementingServiceTypeException">When decorated class does not implement service type.</exception>
    internal static IServiceCollection AddAutoServices(this IServiceCollection services, IEnumerable<Assembly> assemblies, Func<TypeInfo, bool>? excludeCondition)
    {
        var servicesImplementations = assemblies.Distinct().SelectMany(s => s.DefinedTypes)
            .Where(w => Attribute.IsDefined(w, typeof(AutoServiceAttribute)))
            .Where(w => excludeCondition?.Invoke(w) != true);
        foreach (var serviceImplementation in servicesImplementations)
        {
            var serviceDescriptors = serviceImplementation
                .GetCustomAttributes<AutoServiceAttribute>()
                .SelectMany(s => s.GetServiceDescriptors(serviceImplementation));
            foreach (var serviceDescriptor in serviceDescriptors)
            {
                services.Add(serviceDescriptor);
            }
        }

        return services;
    }
}