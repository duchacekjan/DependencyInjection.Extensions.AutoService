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
    private readonly ServiceTypeOptions _options;
    private readonly ServiceLifetime _serviceLifetime;

    public AutoServiceAttribute(bool selfAsService = false, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        : this(selfAsService ? ServiceTypeOptions.Self : ServiceTypeOptions.AllImplementedInterfaces, serviceLifetime)
    {
    }

    public AutoServiceAttribute(ServiceLifetime serviceLifetime, params Type[] types)
        : this(ServiceTypeOptions.Types(types), serviceLifetime)
    {
    }

    private AutoServiceAttribute(ServiceTypeOptions options, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        _options = options;
        _serviceLifetime = serviceLifetime;
    }

    public IEnumerable<ServiceDescriptor> GetServiceDescriptors(TypeInfo decoratedTypeInfo)
    {
        var result = new List<ServiceDescriptor>();
        switch (_options.Kind)
        {
            case ServiceTypeKind.Self:
                result.Add(GetSelfDescriptor(decoratedTypeInfo.AsType(), _serviceLifetime));
                break;
            case ServiceTypeKind.AllImplementedInterfaces:
                result.AddRange(GetServiceDescriptors(decoratedTypeInfo, _serviceLifetime));
                break;
            case ServiceTypeKind.Type:
                result.AddRange(GetServiceDescriptors(decoratedTypeInfo, _options.ServiceTypes, _serviceLifetime));
                break;
        }

        return result;
    }

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

    private static ServiceDescriptor GetSelfDescriptor(Type implementationType, ServiceLifetime serviceLifetime)
        => new(implementationType, implementationType, serviceLifetime);

    private static IEnumerable<ServiceDescriptor> GetServiceDescriptors(TypeInfo decoratedTypeInfo, IEnumerable<Type>? types, ServiceLifetime serviceLifetime)
    {
        var result = new List<ServiceDescriptor>();
        var implementationType = decoratedTypeInfo.AsType();
        var serviceTypes = types?.ToList() ?? new List<Type> { implementationType };
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

public enum ServiceTypeKind
{
    Self,
    AllImplementedInterfaces,
    Type
}

internal class ServiceTypeOptions
{
    private ServiceTypeOptions(ServiceTypeKind kind, IEnumerable<Type>? serviceTypes)
    {
        Kind = kind;
        ServiceTypes = serviceTypes;
    }

    public ServiceTypeKind Kind { get; }
    public IEnumerable<Type>? ServiceTypes { get; }

    internal static ServiceTypeOptions Self => new(ServiceTypeKind.Self, null);
    internal static ServiceTypeOptions Types(IEnumerable<Type> types) => new(ServiceTypeKind.Type, types);
    internal static ServiceTypeOptions AllImplementedInterfaces => new(ServiceTypeKind.AllImplementedInterfaces, null);
}