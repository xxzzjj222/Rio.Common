using Rio.Common;
using Rio.Common.Helpers;
using Rio.Extensions;
using System.Reflection;
using System.Security.Cryptography;

namespace Microsoft.Extensions.DependencyInjection;


public interface IServiceModule
{
    void ConfigureService(IServiceCollection services);
}

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterAssemblyTypes(this IServiceCollection services,Func<Type,bool>? typeFilter,ServiceLifetime serviceLifetime, params Assembly[] assemblies)
    {
        if(Guard.NotNull(assemblies,nameof(assemblies)).Length==0) 
        {
            assemblies = ReflectHelper.GetAssemblies();
        }

        var types = assemblies
                .Select(x => x.GetTypes())
                .SelectMany(t => t)
                .Where(t => !t.IsAbstract);

        if(typeFilter!=null)
        {
            types = types.Where(typeFilter);
        }

        foreach(var type in types)
        {
            services.Add(new ServiceDescriptor(type, type, serviceLifetime));
        }

        return services;
    }

    public static IServiceCollection RegisterAssemblyTypesAdImplementedInterfaces(this IServiceCollection services,Func<Type,bool>? typeFilter,Func<Type,bool> interfaceTypeFilter,ServiceLifetime serviceLifetime,params Assembly[] assemblies)
    {
        if(Guard.NotNull(assemblies,nameof(assemblies)).Length==0)
        {
            assemblies = ReflectHelper.GetAssemblies();
        }

        var types=assemblies
                .Select(t=>t.GetTypes())
                .SelectMany(t => t)
                .Where(t=> !t.IsAbstract);

        if(typeFilter!=null)
        {
            types = types.Where(typeFilter);
        }

        foreach(var type in types)
        {
            foreach(var implementedInterface in type.GetInterfaces())
            {
                if(interfaceTypeFilter?.Invoke(implementedInterface)!=true)
                {
                    services.Add(new ServiceDescriptor(implementedInterface,type,serviceLifetime));
                }
            }
        }

        return services;
    }

    public static IServiceCollection RegisterModule<TServiceModule>(this IServiceCollection services,TServiceModule module)
        where TServiceModule:IServiceModule
    {
        module.ConfigureService(services);
        return services;
    }

    public static IServiceCollection RegisterAssemblyModules(IServiceCollection services, Assembly[] assemblies)
    {
        if(Guard.NotNull(assemblies.Length)==0)
        {
            assemblies = ReflectHelper.GetAssemblies();
        }

        var types = assemblies
                .SelectMany(t => t.GetTypes())
                .Where(t => !t.IsAbstract && t.IsClass && typeof(IServiceModule).IsAssignableFrom(t));

        foreach (var type in types)
        {
            try
            {
                if(Activator.CreateInstance(type) is IServiceModule module)
                {
                    module.ConfigureService(services);
                }
            }
            catch(Exception ex)
            {
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }
        }

        return services;
    }

    public static IServiceCollection Decorate(this IServiceCollection services,Type serviceType,Type decorateType)
    {
        var service = services.LastOrDefault(x => x.ServiceType == serviceType);
        if (service == null)
        {
            throw new InvalidOperationException("The service is not registered, service need to be registered before decorating");
        }

        var objectFactory = ActivatorUtilities.CreateFactory(decorateType, new[] { serviceType });
        var decoratorService=new ServiceDescriptor(serviceType, sp => objectFactory(sp, new[]
        {
            sp.CreateInstance(service)
        }),service.Lifetime);

        return services;
    }

    private static object CreateInstance(this IServiceProvider services,ServiceDescriptor descriptor)
    {
        if(descriptor.ImplementationInstance!=null)
            return descriptor.ImplementationInstance;

        if (descriptor.ImplementationFactory != null)
            return descriptor.ImplementationFactory.Invoke(services);

        return ActivatorUtilities.GetServiceOrCreateInstance(services, descriptor.ServiceType);
    }
}
