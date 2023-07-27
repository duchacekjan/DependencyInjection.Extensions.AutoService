using System.Reflection;
using DependencyInjection.Extensions.AutoService.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ClassNeverInstantiated.Local

namespace DependencyInjection.Extensions.AutoService.Tests;

public partial class AutoServiceTests
{
    [AutoService]
    public class SingleImplementation : ITestA
    {
    }

    [AutoService]
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

    [AutoService(typeof(MultipleImplementation))]
    [AutoService(typeof(ITestA))]
    [AutoService(typeof(ITestB))]
    private class MultipleImplementation : ITestA, ITestB
    {
    }

    [AutoService(SelfImplementationUsage.AddSelfImplementation)]
    private class MultipleImplementationSimple : ITestA, ITestB
    {
    }

    [AutoService]
    private class ChildImplementation : BaseImplementation
    {
    }

    [AutoService(typeof(BaseImplementation))]
    private class InheritedImplementation : BaseImplementation
    {
    }

    [AutoService(ServiceLifetime.Transient, typeof(BaseImplementation))]
    private class InheritedTransientImplementation : BaseImplementation
    {
    }

    private static IServiceCollection ServicesFromExecutingAssembly()
    {
        var services = new ServiceCollection();
        services.AddAutoServices(Assembly.GetExecutingAssembly());
        return services;
    }
}