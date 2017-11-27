using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rabbit.Extensions.DependencyInjection
{
    public static class ServiceDependencyInjectionExtensions
    {
        public static IServiceCollection AddServiceRegister(this IServiceCollection services, Func<AssemblyName, bool> predicate = null)
        {
            var assemblies = GetAssemblies(predicate);
            return services.AddServiceRegister(assemblies);
        }

        public static IServiceCollection AddServiceRegister(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            //获取程序集中所有实现IServiceRegister接口的类型
            var types = assemblies.SelectMany(i => i.GetExportedTypes())
                .Where(i =>
                {
                    var info = i.GetTypeInfo();
                    return info.IsClass && !info.IsAbstract && typeof(IServiceRegister).IsAssignableFrom(i);
                });

            foreach (var type in types)
            {
                var constructor = type.GetConstructor(new Type[0]);
                var instance = (IServiceRegister)constructor.Invoke(new object[0]);
                instance.Register(services);
            }

            return services;
        }

        public static IServiceCollection AddInterfaceDependency(this IServiceCollection services, Func<AssemblyName, bool> predicate = null)
        {
            var assemblies = GetAssemblies(predicate);
            return services.AddInterfaceDependency(assemblies);
        }

       

        public static IServiceCollection AddInterfaceDependency(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            //获取程序集中所有实现IDependency接口的类型
            var types = assemblies.SelectMany(i => i.GetExportedTypes())
                .Where(i =>
                {
                    var info = i.GetTypeInfo();
                    return info.IsClass && !info.IsAbstract && typeof(IDependency).IsAssignableFrom(i);
                });

            return services.AddInterfaceDependency(types);
        }

        public static IServiceCollection AddInterfaceDependency(this IServiceCollection services, IEnumerable<Type> types)
        {
            foreach (var type in types)
                RegisterDependency(type, services);

            return services;
        }

        public static IServiceCollection AddServiceExtensions(this IServiceCollection services)
        {
            var method = typeof(ServiceDependencyInjectionExtensions).GetMethod(nameof(GetServiceExtensions), BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var service in services.ToArray().GroupBy(i => i.ServiceType).Select(i => i.First()).Where(i => !i.ServiceType.ContainsGenericParameters))
            {
                var extensionsDescriptors = (IEnumerable<ServiceDescriptor>)method.MakeGenericMethod(service.ServiceType).Invoke(null, null);
                foreach (var serviceDescriptor in extensionsDescriptors)
                {
                    services.Add(serviceDescriptor);
                }
            }

            return services;
        }

        #region Private Method

        private static IEnumerable<ServiceDescriptor> GetServiceExtensions<T>()
        {
            yield return ServiceDescriptor.Transient(typeof(Lazy<T>), provider => new Lazy<T>(provider.GetRequiredService<T>));
            yield return ServiceDescriptor.Transient(typeof(Lazy<IEnumerable<T>>), provider => new Lazy<IEnumerable<T>>(provider.GetRequiredService<IEnumerable<T>>));
            yield return ServiceDescriptor.Singleton(typeof(Func<T>), provider => new Func<T>(provider.GetRequiredService<T>));
            yield return ServiceDescriptor.Singleton(typeof(Func<IEnumerable<T>>), provider => new Func<IEnumerable<T>>(provider.GetRequiredService<IEnumerable<T>>));
        }

        private static IEnumerable<Assembly> GetAssemblies(Func<AssemblyName, bool> predicate = null)
        {
            var assemblyNames = DependencyContext.Default.RuntimeLibraries.SelectMany(i => i.GetDefaultAssemblyNames(DependencyContext.Default));
            if (predicate != null)
                assemblyNames = assemblyNames.Where(predicate).ToArray();
            var assemblies = assemblyNames.Select(i => Assembly.Load(new AssemblyName(i.Name))).ToArray();
            return assemblies;
        }

        private static void RegisterDependency(Type type, IServiceCollection services)
        {
            foreach (var serviceDescriptor in GetServiceDescriptors(type))
            {
                if (serviceDescriptor != null)
                    services.Add(serviceDescriptor);
            }
        }

        private static IEnumerable<ServiceDescriptor> GetServiceDescriptors(Type implementationType)
        {
            var interfaces = implementationType
                .GetInterfaces()
                .Where(i => typeof(IDependency).IsAssignableFrom(i))
                .ToArray();

            //没有任何标识依赖的接口
            if (!interfaces.Any())
                yield break;

            //处理类本身实现的接口
            var baseDependency = interfaces.FirstOrDefault();
            var defaultDependencys = new[] { typeof(IDependency), typeof(ISingletonDependency), typeof(ITransientDependency) };
            if (defaultDependencys.Contains(baseDependency))
                yield return GetServiceDescriptor(implementationType, implementationType);

            //处理其它服务类型
            foreach (var interfaceType in interfaces.Where(i => !defaultDependencys.Contains(i)))
            {
                yield return GetServiceDescriptor(interfaceType, implementationType);
            }
        }

        private static ServiceDescriptor GetServiceDescriptor(Type serviceType, Type implementationType)
        {
            ServiceDescriptor descriptor = null;
            if (typeof(ISingletonDependency).IsAssignableFrom(serviceType))
            {
                descriptor = ServiceDescriptor.Singleton(serviceType, implementationType);
            }
            else if (typeof(ITransientDependency).IsAssignableFrom(serviceType))
            {
                descriptor = ServiceDescriptor.Transient(serviceType, implementationType);
            }
            else if (typeof(IDependency).IsAssignableFrom(serviceType))
            {
                descriptor = ServiceDescriptor.Scoped(serviceType, implementationType);
            }
            return descriptor;
        }

        #endregion Private Method



        #region add metadata dependency by only attribute

        public static IServiceCollection AddMetadataDependency(this IServiceCollection services, Func<AssemblyName, bool> predicate = null)
        {
            var assemblies = GetAssemblies(predicate);

            var types = assemblies.SelectMany(i => i.GetExportedTypes())
                .Where(t=>t.GetTypeInfo()
                           .IsDefined(typeof(ServiceDescriptorAttribute),inherit: true)
                );

            foreach (var type in types)
            {
                var typeInfo = type.GetTypeInfo();

                var attributes = typeInfo.GetCustomAttributes<ServiceDescriptorAttribute>().ToArray();

                // Check if the type has multiple attributes with same ServiceType.
                var duplicates = attributes
                    .GroupBy(s => s.ServiceType)
                    .SelectMany(grp => grp.Skip(1));

                if (duplicates.Any())
                {
                    throw new InvalidOperationException($@"Type ""{type.FullName}"" has multiple ServiceDescriptor attributes with the same service type.");
                }

                foreach (var attribute in attributes)
                {
                    var serviceTypes = getRequireMetadataServiceTypes(type, attribute);

                    foreach (var serviceType in serviceTypes)
                    {
                        //
                        var descriptor = new ServiceDescriptor(serviceType, type, attribute.Lifetime);

                        services.Add(descriptor);

                        if (serviceType.IsInterface)
                        {
                            var extensionDescriptor = (ServiceDescriptor)getMetadataServiceDescriptorMethodInfo
                                                        .MakeGenericMethod(descriptor.ServiceType)
                                                        .Invoke(null, new object[] { descriptor.ImplementationType });

                            services.Add(extensionDescriptor);

                            ServiceTypeMetadataExtensions.AddMetadata(descriptor.ImplementationType, attribute.Name);

                        }

                    }
                }
            }

            return services;
        }

        /// <summary>
        /// get support metadata required interface and class .
        /// such as a class:interface, required types as class and interface.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private static IEnumerable<Type> getRequireMetadataServiceTypes(Type type, ServiceDescriptorAttribute attribute)
        {
            var typeInfo = type.GetTypeInfo();

            var serviceType = attribute.ServiceType;

            if (serviceType == null)
            {
                yield return type;

                foreach (var implementedInterface in typeInfo.ImplementedInterfaces)
                {
                    yield return implementedInterface;
                }

                if (typeInfo.BaseType != null && typeInfo.BaseType != typeof(object))
                {
                    yield return typeInfo.BaseType;
                }

                yield break;
            }

            var serviceTypeInfo = serviceType.GetTypeInfo();

            if (!serviceTypeInfo.IsAssignableFrom(typeInfo))
            {
                throw new InvalidOperationException($@"Type ""{typeInfo.FullName}"" is not assignable to ""${serviceTypeInfo.FullName}"".");
            }

            yield return serviceType;
            yield return type;

        }

        static MethodInfo getMetadataServiceDescriptorMethodInfo = typeof(ServiceDependencyInjectionExtensions).GetMethod(nameof(GetMetadataServiceDescriptor), BindingFlags.NonPublic | BindingFlags.Static);
        private static ServiceDescriptor GetMetadataServiceDescriptor<T>(Type ImplementationType)
        {

            return ServiceDescriptor.Transient(typeof(Lazy<T, ServiceTypeMetadata>),
               provider =>
               new Lazy<T, ServiceTypeMetadata>(
                   () =>
                   (T)provider.GetRequiredService(ImplementationType),
                   ServiceTypeMetadataExtensions.GetServiceTypeMetadata(ImplementationType)
               ));

        }

        #endregion


    }
}