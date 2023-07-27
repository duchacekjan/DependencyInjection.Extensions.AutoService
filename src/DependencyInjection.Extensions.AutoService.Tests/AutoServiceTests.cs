using System.Reflection;
using DependencyInjection.Extensions.AutoService.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Extensions.AutoService.Tests;

public partial class AutoServiceTests
{
    [Fact]
    public void Should_register_interface()
    {
        var services = ServicesFromExecutingAssembly();
        var sut = services.BuildServiceProvider();

        var actual = sut.GetService<ITestA>();
        actual.Should().NotBeNull();
    }

    [Fact]
    public void Should_register_self()
    {
        var services = ServicesFromExecutingAssembly();
        var sut = services.BuildServiceProvider();

        var actual = sut.GetService<SingleSelfImplementation>();
        actual.Should().NotBeNull();
    }

    [Fact]
    public void Should_register_multiple_auto_services()
    {
        var services = ServicesFromExecutingAssembly();
        services.Where(w => w.ImplementationType == typeof(MultipleImplementation)).Should().HaveCount(3);
        services.Where(w => w.ImplementationType == typeof(MultipleImplementationSimple)).Should().HaveCount(3);
    }


    [Fact]
    public void Should_register_child_auto_services()
    {
        var services = ServicesFromExecutingAssembly();
        services.Where(w => w.ImplementationType == typeof(ChildImplementation)).Should().HaveCount(2);
    }

    [Fact]
    public void Should_register_auto_services_as_inherited_implementation()
    {
        var services = ServicesFromExecutingAssembly();
        var sut = services.BuildServiceProvider();

        var actual = sut.GetService<BaseImplementation>();
        actual.Should().NotBeNull();
        
        services.Where(w => w.ServiceType == typeof(BaseImplementation)).Should().HaveCount(2);
    }

    [Fact]
    public void Should_register_from_calling_assembly()
    {
        var services = new ServiceCollection();
        services.AddAutoServices();
        var executingAssemblyServices = ServicesFromExecutingAssembly();
        services.Should().HaveSameCount(executingAssemblyServices);
    }
    
    [Fact]
    public void Should_throw_on_adding_services()
    {
        var services = new ServiceCollection();
        services.Invoking(i => i.AddAutoServices(new[] { Assembly.GetExecutingAssembly() }, true))
            .Should().Throw<NotImplementingServiceTypeException>();
    }

    [Theory]
    [InlineData(typeof(SingleSingleton), ServiceLifetime.Singleton)]
    [InlineData(typeof(SingleScoped), ServiceLifetime.Scoped)]
    [InlineData(typeof(SingleTransient), ServiceLifetime.Transient)]
    [InlineData(typeof(BaseImplementation), ServiceLifetime.Transient)]
    public void Should_register_correct_lifetime(Type expectedType, ServiceLifetime expectedLifetime)
    {
        var services = ServicesFromExecutingAssembly();

        services.Should().ContainSingle(s => s.Lifetime == expectedLifetime && s.ServiceType == expectedType);
    }
}