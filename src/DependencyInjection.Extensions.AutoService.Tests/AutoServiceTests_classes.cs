using System.Reflection;
using DependencyInjection.Extensions.AutoService.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ClassNeverInstantiated.Local

namespace DependencyInjection.Extensions.AutoService.Tests;

public partial class AutoServiceTests
{
    [AutoService(typeof(ITestA))]
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

    [AutoService]
    [AutoService(typeof(ITestA))]
    [AutoService(typeof(ITestB))]
    private class MultipleImplementation : ITestA, ITestB
    {
    }

    [AutoService]
    [AutoService(typeof(IBaseA))]
    [AutoService(typeof(IBaseB))]
    private class ChildImplementation : BaseImplementation
    {
    }

    [AutoService(typeof(BaseImplementation))]
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