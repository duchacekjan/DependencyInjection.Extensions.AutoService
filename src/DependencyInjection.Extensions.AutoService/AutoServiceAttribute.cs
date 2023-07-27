using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Extensions.AutoService;

/// <summary>
/// Attribute allows auto register service implementation.
/// Decorated class should not be abstract class.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class AutoServiceAttribute : Attribute
{
    private readonly bool _allImplementedInterfaces;
    private readonly SelfImplementationUsage _selfImplementation;
    private readonly ServiceLifetime _serviceLifetime;
    private readonly List<Type> _serviceTypes;

    /// <summary>
    /// Registers decorated class as service implementation.
    /// </summary>
    /// <param name="selfImplementation">Handling decorated class as service. <see cref="SelfImplementationUsage"/></param>
    /// <param name="serviceLifetime">Services lifetime.</param>
    public AutoServiceAttribute(SelfImplementationUsage selfImplementation = SelfImplementationUsage.WhenNoServicePresent, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        : this(allImplementedInterfaces: true, selfImplementation, serviceLifetime, null)
    {
    }

    /// <summary>
    /// Registers decorated class as service implementation of <paramref name="serviceType"/>
    /// and <paramref name="servicesTypes"/> with <see cref="ServiceLifetime.Scoped"/>
    /// </summary>
    /// <param name="serviceType">Service type.</param>
    /// <param name="servicesTypes">Another service types.</param>
    public AutoServiceAttribute(Type serviceType, params Type[] servicesTypes)
        : this(ServiceLifetime.Scoped, serviceType, servicesTypes)
    {
    }

    /// <summary>
    /// Registers decorated class as service implementation of <paramref name="serviceType"/>
    /// and <paramref name="servicesTypes"/> with <paramref name="serviceLifetime"/>
    /// </summary>
    /// <param name="serviceLifetime">Services lifetime</param>
    /// <param name="serviceType">Service type.</param>
    /// <param name="servicesTypes">Another service types.</param>
    public AutoServiceAttribute(ServiceLifetime serviceLifetime, Type serviceType, params Type[] servicesTypes)
        : this(false, SelfImplementationUsage.Disabled, serviceLifetime, servicesTypes.Concat(new[] { serviceType }))
    {
    }

    /// <summary>
    /// Common constructor for auto registering services.
    /// </summary>
    /// <param name="allImplementedInterfaces">If <see langword="true"/> it registers all implemented interfaces</param>
    /// <param name="selfImplementation">Describes how to handle decorated class type for service registration</param>
    /// <param name="serviceLifetime">Services lifetime</param>
    /// <param name="servicesTypes">Types of services</param>
    private AutoServiceAttribute(bool allImplementedInterfaces, SelfImplementationUsage selfImplementation, ServiceLifetime serviceLifetime, IEnumerable<Type>? servicesTypes)
    {
        _serviceTypes = servicesTypes?.ToList() ?? new List<Type>();
        _allImplementedInterfaces = allImplementedInterfaces;
        _selfImplementation = selfImplementation;
        _serviceLifetime = serviceLifetime;
    }

    /// <summary>
    /// Returns list of service registrations.
    /// </summary>
    /// <param name="decoratedTypeInfo">Decorated type.</param>
    /// <returns></returns>
    public IEnumerable<ServiceDescriptor> GetServiceDescriptors(TypeInfo decoratedTypeInfo)
    {
        var result = new List<ServiceDescriptor>();


        if (_allImplementedInterfaces)
        {
            result.AddRange(GetServiceDescriptors(decoratedTypeInfo, _serviceLifetime));
        }

        if (_serviceTypes.Any())
        {
            result.AddRange(GetServiceDescriptors(decoratedTypeInfo, _serviceTypes, _serviceLifetime));
        }

        var addSelfImplementation = _selfImplementation == SelfImplementationUsage.AddSelfImplementation;
        var addSelfNoServicePresent = _selfImplementation == SelfImplementationUsage.WhenNoServicePresent && !result.Any();
        if (addSelfImplementation || addSelfNoServicePresent)
        {
            result.Add(GetSelfDescriptor(decoratedTypeInfo.AsType(), _serviceLifetime));
        }

        return result;
    }

    /// <summary>
    /// Returns list of service registrations from implemented interfaces.
    /// </summary>
    /// <param name="decoratedTypeInfo"></param>
    /// <param name="serviceLifetime"></param>
    /// <returns></returns>
    private static IEnumerable<ServiceDescriptor> GetServiceDescriptors(TypeInfo decoratedTypeInfo, ServiceLifetime serviceLifetime)
    {
        var result = decoratedTypeInfo.ImplementedInterfaces
            .Select(s => new ServiceDescriptor(s, decoratedTypeInfo.AsType(), serviceLifetime))
            .ToList();
        if (result.Count == 0)
        {
            result.Add(GetSelfDescriptor(decoratedTypeInfo.AsType(), serviceLifetime));
        }

        return result;
    }

    /// <summary>
    /// Returns decorated class as service.
    /// </summary>
    /// <param name="implementationType">Decorated class type</param>
    /// <param name="serviceLifetime">Services lifetime</param>
    /// <returns></returns>
    private static ServiceDescriptor GetSelfDescriptor(Type implementationType, ServiceLifetime serviceLifetime)
        => new(implementationType, implementationType, serviceLifetime);

    /// <summary>
    /// list of service registrations from explicitly given types.
    /// </summary>
    /// <param name="decoratedTypeInfo">Decorated class type info.</param>
    /// <param name="serviceTypes">Types of services.</param>
    /// <param name="serviceLifetime">Services lifetime.</param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">Decorated class is abstract.</exception>
    /// <exception cref="NotImplementingServiceTypeException">Decorated class cannot be cast to service type.</exception>
    private static IEnumerable<ServiceDescriptor> GetServiceDescriptors(TypeInfo decoratedTypeInfo, List<Type> serviceTypes, ServiceLifetime serviceLifetime)
    {
        var result = new List<ServiceDescriptor>();
        var implementationType = decoratedTypeInfo.AsType();
        foreach (var serviceType in serviceTypes)
        {
            if (IsServiceImplementation(serviceType, decoratedTypeInfo))
            {
                if (decoratedTypeInfo.IsAbstract)
                {
                    throw new NotSupportedException();
                }

                result.Add(new ServiceDescriptor(serviceType, implementationType, serviceLifetime));
            }
            else
            {
                throw new NotImplementingServiceTypeException(serviceType, implementationType);
            }
        }

        return result;
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