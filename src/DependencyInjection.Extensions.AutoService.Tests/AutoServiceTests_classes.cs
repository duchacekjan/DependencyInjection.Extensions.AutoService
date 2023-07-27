using System.Reflection;
using DependencyInjection.Extensions.AutoService.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ClassNeverInstantiated.Local

namespace DependencyInjection.Extensions.AutoService.Tests;

public partial class AutoServiceTests
{
    [AutoService(ServiceLifetime.Transient, typeof(ITestA))]
    public class SingleImplementation : ITestA
    {
    }

    [AutoService(selfAsService: true)]
    private class SingleSelfImplementation
    {
    }

    [AutoService(serviceLifetime: ServiceLifetime.Transient)]
    private class SingleTransient
    {
    }

    [AutoService(serviceLifetime: ServiceLifetime.Singleton)]
    private class SingleSingleton
    {
    }

    [AutoService(serviceLifetime: ServiceLifetime.Scoped)]
    private class SingleScoped
    {
    }

    [AutoService(selfAsService: true)]
    [AutoService(ServiceLifetime.Transient, typeof(ITestA), typeof(ITestB))]
    private class MultipleImplementation : ITestA, ITestB
    {
    }

    [AutoService(selfAsService: true)]
    [AutoService(ServiceLifetime.Transient, typeof(IBaseA), typeof(IBaseB))]
    private class ChildImplementation : BaseImplementation
    {
    }

    [AutoService(ServiceLifetime.Transient, typeof(BaseImplementation))]
    private class InheritedImplementation : BaseImplementation
    {
    }

    private static IServiceCollection ServicesFromExecutingAssembly()
    {
        var services = new ServiceCollection();
        services.AddAutoServices(Assembly.GetExecutingAssembly());
        return services;
    }
}