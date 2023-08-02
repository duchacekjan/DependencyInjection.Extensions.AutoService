Auto register service in Microsoft.DI using attribute.
```csharp

// Service registration without attribute
services.AddScoped<IServiceA, Service>()
        .AddScoped<IServiceB, Service>();

// Service registration alternative using attribute
[AutoService]
public class Service : IServiceA, IServiceB
```
# Setup

## Decorate classes

```csharp
// Decorate class as auto service
[AutoService]
public class Service : IServiceA, IServiceB
```
## Add configuration in startup.cs
```csharp

private static void ConfigureServices(IServiceCollection services)
{   
    var executingAssembly = Assembly.GetExecutingAssembly()
    // Add AutoServices
    services.AddAutoServices(executingAssembly);

    // optionally register all auto services from calling assembly
    // services.AddAutoServices();
        
    // Add other registration
}

```

# Sample usages

## Description SelfImplementationUsage
Enum class describing how to handle self registration, which means that service implementation serves as service.
1. **AddSelfImplementation** -  Adds implementing class as service.
2. **WhenNoServicePresent** - Adds implementing class as service only when no other service is auto registered on implementing class.
3. **Disabled** - Implementing class is never added as service.

## Using all implemented interfaces
By default attribute registers all implemented interfaces with Scoped lifetime and add self implementation only when no interface is implemented.</br>
Other self implementation options are described [here](#description-selfimplementationusage).
```csharp
public AutoServiceAttribute(SelfImplementationUsage selfImplementation = SelfImplementationUsage.WhenNoServicePresent, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
```
### Examples
#### Register all implemented interfaces with lifetime as scoped.
##### Implementation
```csharp
abstract class BaseService: IServiceBase
[AutoService]
class Service : IServiceA, IServiceB
[AutoService]
class ChildService : BaseService
```
##### Registered services

| ServiceType  | Implementation | Lifetime |
|--------------|----------------|----------|
| IServiceA    | Service        | Scoped   |
| IServiceB    | Service        | Scoped   |
| IServiceBase | ChildService   | Scoped   |

#### Register self implementation with lifetime as scoped.
##### Implementation
```csharp
// SelfImplementationUsage.WhenNoServicePresent
[AutoService]
class Service
[AutoService(SelfImplementationUsage.AddSelfImplementation)]
class SelfService : IServiceA, IServiceB
[AutoService(SelfImplementationUsage.Disabled)]
class NoSelfService
```
##### Registered services

| ServiceType | Implementation | Lifetime |
|-------------|----------------|----------|
| Service     | Service        | Scoped   |
| IServiceA   | SelfService    | Scoped   |
| IServiceB   | SelfService    | Scoped   |
| SelfService | SelfService    | Scoped   |

#### Register services with different lifetimes
##### Implementation
```csharp
[AutoService(serviceLifetime: ServiceLifetime.Transient)]
class Service : IServiceA, IServiceB
```
##### Registered services

| ServiceType | Implementation | Lifetime  |
|-------------|----------------|-----------|
| IServiceA   | Service        | Transient |
| IServiceB   | Service        | Transient |

## Using explicit types
By default attribute registers with lifetime scoped.
```csharp
public AutoServiceAttribute(Type serviceType, params Type[] servicesTypes)
public AutoServiceAttribute(ServiceLifetime serviceLifetime, Type serviceType, params Type[] servicesTypes)

```
### Examples
#### Register explicitly defined service types
##### Implementation
```csharp
[AutoService(typeof(IServiceA)]
class Service : IServiceA, IServiceB
[AutoService(typeof(IServiceB), typeof(ServiceB)]
class ServiceB : IServiceA, IServiceB
```
##### Registered services

| ServiceType | Implementation | Lifetime |
|-------------|----------------|----------|
| IServiceA   | Service        | Scoped   |
| IServiceB   | ServiceB       | Scoped   |
| ServiceB    | ServiceB       | Scoped   |

#### Register explicitly defined with different lifetimes
##### Implementation
```csharp
[AutoService(ServiceLifetime.Transient, typeof(IServiceA), typeof(Service)]
[AutoService(ServiceLifetime.Singleton, typeof(IServiceB)]
class Service : IServiceA, IServiceB
```
##### Registered services

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