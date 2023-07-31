# DependencyInjection.Extensions.AutoService
![master workflow](https://github.com/duchacekjan/DependencyInjection.Extensions.AutoService/actions/workflows/ci.yml/badge.svg?branch=master)
![master workflow](https://github.com/duchacekjan/DependencyInjection.Extensions.AutoService/actions/workflows/publish.yml/badge.svg?branch=master)
[![NuGet](https://img.shields.io/nuget/vpre/DependencyInjection.Extensions.AutoService.svg)](https://www.nuget.org/packages/DependencyInjection.Extensions.AutoService)

Auto register service in Microsoft.DI using attribute.
```csharp

// Service registration
services.AddScoped<IServiceA, Service>()
        .AddScoped<IServiceB, Service>();

// Service registration using attribute
[AutoService]
public class Service : IServiceA, IServiceB
```

# Install

## Install with Package Manager Console

```
Install-Package DependencyInjection.Extensions.AutoService
```

## Install with .NET CLI
```
dotnet add package DependencyInjection.Extensions.AutoService
```

# How to use

## Decorate classes

```csharp
// Decorate class as auto service and then you will have registration on one place with its implementation 
// and you will see that class is registered as service implementation
[AutoService]
public class Service : IServiceA, IServiceB
```
## Setup - Add configuration in startup.cs
```csharp

private static IServiceCollection ConfigureServices(IServiceCollection services)
{   
    var executingAssembly = Assembly.GetExecutingAssembly()
    // Add AutoServices
    services.AddAutoServices(executingAssembly);

    // optionally register all auto services from calling assembly
    // services.AddAutoServices();
        
    //Add other registration
    ...
    return services;
}

```

# Use cases

## Using all implemented interfaces
By default attribute registers all implemented interfaces with Scoped lifetime and add self implementation only when no interface is implemented
```csharp
public AutoServiceAttribute(SelfImplementationUsage selfImplementation = SelfImplementationUsage.WhenNoServicePresent, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
```
### Examples
```csharp
[AutoServiceAttribute]
class Service
```
Registered services:

| ServiceType | Implementation | Lifetime |
|-------------|----------------|----------|
| Service     | Service        | Scoped   |

```csharp
[AutoServiceAttribute]
class Service : IServiceA
```
Registered services:

| ServiceType | Implementation | Lifetime |
|-------------|----------------|----------|
| IServiceA   | Service        | Scoped   |

```csharp
[AutoServiceAttribute]
class Service : IServiceA, IServiceB
```
Registered services:

| ServiceType | Implementation | Lifetime |
|-------------|----------------|----------|
| IServiceA   | Service        | Scoped   |
| IServiceB   | Service        | Scoped   |

```csharp
[AutoServiceAttribute(SelfImplementationUsage.AddSelfImplementation)]
class Service : IServiceA, IServiceB
```
Registered services:

| ServiceType | Implementation | Lifetime |
|-------------|----------------|----------|
| IServiceA   | Service        | Scoped   |
| IServiceB   | Service        | Scoped   |
| Service     | Service        | Scoped   |

```csharp
[AutoService(serviceLifetime: ServiceLifetime.Transient)]
class Service : IServiceA, IServiceB
```
Registered services:

| ServiceType | Implementation | Lifetime  |
|-------------|----------------|-----------|
| IServiceA   | Service        | Transient |
| IServiceB   | Service        | Transient |

## Using explicit types
By default attribute registers with Scoped lifetime
```csharp
public AutoServiceAttribute(Type serviceType, params Type[] servicesTypes)
public AutoServiceAttribute(ServiceLifetime serviceLifetime, Type serviceType, params Type[] servicesTypes)

```
### Examples
```csharp
[AutoService(typeof(IServiceA)]
class Service : IServiceA, IServiceB
```
Registered services:

| ServiceType | Implementation | Lifetime |
|-------------|----------------|----------|
| IServiceA   | Service        | Scoped   |

```csharp
[AutoService(typeof(IServiceA), typeof(Service)]
class Service : IServiceA, IServiceB
```
Registered services:

| ServiceType | Implementation | Lifetime |
|-------------|----------------|----------|
| IServiceA   | Service        | Scoped   |
| Service     | Service        | Scoped   |

```csharp
[AutoService(ServiceLifetime.Transient, typeof(IServiceA), typeof(Service)]
class Service : IServiceA, IServiceB
```
Registered services:

| ServiceType | Implementation | Lifetime  |
|-------------|----------------|-----------|
| IServiceA   | Service        | Transient |
| Service     | Service        | Transient |


```csharp
[AutoService(ServiceLifetime.Transient, typeof(IServiceA), typeof(Service)]
[AutoService(ServiceLifetime.Singleton, typeof(IServiceB)]
class Service : IServiceA, IServiceB
```
Registered services:

| ServiceType | Implementation | Lifetime  |
|-------------|----------------|-----------|
| IServiceA   | Service        | Transient |
| Service     | Service        | Transient |
| IServiceB   | Service        | Singleton |


## Wrong usages
Attribute cannot be used on abstract class.</br>
Using explicitly named types has to be assignable from decorated type.

### Abstract class example
```csharp
[AutoService]
abstract class Service
```

### Not implementing service example
```csharp
[AutoService(typeof(IServiceA)]
class Service
```