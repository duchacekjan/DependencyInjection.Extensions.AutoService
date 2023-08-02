# DependencyInjection.Extensions.AutoService
![master workflow](https://github.com/duchacekjan/DependencyInjection.Extensions.AutoService/actions/workflows/ci.yml/badge.svg?branch=master)
[![NuGet](https://img.shields.io/nuget/vpre/DependencyInjection.Extensions.AutoService.svg)](https://www.nuget.org/packages/DependencyInjection.Extensions.AutoService)
[![NuGet Downloads](https://img.shields.io/nuget/dt/DependencyInjection.Extensions.AutoService.svg)](https://www.nuget.org/packages/DependencyInjection.Extensions.AutoService/)

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

# Usage
How to use package is described in section [usage](./src/DependencyInjection.Extensions.AutoService/USAGE.md).