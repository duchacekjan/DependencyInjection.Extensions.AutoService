namespace DependencyInjection.Extensions.AutoService;

public class NotImplementingServiceTypeException : Exception
{
    public NotImplementingServiceTypeException(Type serviceType, Type implementationType)
        : base(GetMessage(serviceType, implementationType))
    {
    }

    private static string GetMessage(Type serviceType, Type implementationType)
        => $"Service implementation type {implementationType} is not implementation of service type {serviceType}";
}