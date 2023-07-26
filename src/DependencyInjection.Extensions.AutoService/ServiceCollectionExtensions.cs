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
    /// <param name="services">Collection of services</param>
    /// <param name="assemblies">Assemblies containing services implementations</param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">When Abstract class is decorated with <see cref="AutoServiceAttribute"/></exception>
    /// <exception cref="NotImplementingServiceTypeException">When decorated class does not implement service type</exception>
    public static IServiceCollection AddAutoServices(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var servicesImplementations = assemblies.Distinct().SelectMany(s => s.DefinedTypes)
            .Where(w => Attribute.IsDefined(w, typeof(AutoServiceAttribute)));
        foreach (var serviceImplementation in servicesImplementations)
        {
            foreach (var autoServiceAttribute in serviceImplementation.GetCustomAttributes<AutoServiceAttribute>())
            {
                var serviceImplementationType = serviceImplementation.AsType();
                var serviceType = autoServiceAttribute.ServiceType ?? serviceImplementationType;
                if (IsServiceImplementation(serviceType, serviceImplementation))
                {
                    if (serviceImplementation.IsAbstract)
                    {
                        throw new NotSupportedException();
                    }

                    services.Add(new ServiceDescriptor(serviceType, serviceImplementationType, autoServiceAttribute.ServiceLifetime));
                }
                else
                {
                    throw new NotImplementingServiceTypeException(serviceType, serviceImplementationType);
                }
            }
        }

        return services;
    }

    /// <summary>
    /// Determines if implementation can be used as service type
    /// </summary>
    /// <param name="service">Type of service</param>
    /// <param name="implementation">Type info about implementation</param>
    /// <returns></returns>
    private static bool IsServiceImplementation(Type service, TypeInfo implementation)
    {
        return service == implementation.AsType()
               || implementation.ImplementedInterfaces.Contains(service)
               || implementation.IsAssignableTo(service);
    }
}