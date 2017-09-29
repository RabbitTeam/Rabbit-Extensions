﻿using System;

namespace Rabbit.Extensions.DependencyInjection
{
    public static class ServiceProviderServiceExtensions
    {
        public static TService GetKeyedService<TService>(this IServiceProvider serviceProvider, object keyed)
        {
            return (TService)serviceProvider.GetKeyedService(typeof(TService), keyed);
        }

        public static object GetKeyedService(this IServiceProvider serviceProvider, Type serviceType, object keyed)
        {
            if (serviceProvider is ISupportKeyedService supportKeyedService)
                return supportKeyedService.GetKeyedService(serviceType, keyed);
            throw new NotSupportedException("not support keyedService.");
        }

        public static object GetRequiredKeyedService(this IServiceProvider serviceProvider, Type serviceType, object keyed)
        {
            var service = serviceProvider.GetKeyedService(serviceType, keyed);
            if (service == null)
                throw new InvalidOperationException("NoServiceRegistered.");
            return service;
        }

        public static TService GetRequiredKeyedService<TService>(this IServiceProvider serviceProvider, object keyed)
        {
            return (TService)serviceProvider.GetRequiredKeyedService(typeof(TService), keyed);
        }

        public static TService GetNamedService<TService>(this IServiceProvider serviceProvider, string named)
        {
            return (TService)serviceProvider.GetNamedService(typeof(TService), named);
        }

        public static object GetNamedService(this IServiceProvider serviceProvider, Type serviceType, string named)
        {
            return serviceProvider.GetKeyedService(serviceType, named);
        }

        public static object GetRequiredNamedService(this IServiceProvider serviceProvider, Type serviceType, string named)
        {
            var service = serviceProvider.GetNamedService(serviceType, named);
            if (service == null)
                throw new InvalidOperationException("NoServiceRegistered.");
            return service;
        }

        public static TService GetRequiredNamedService<TService>(this IServiceProvider serviceProvider, string named)
        {
            return (TService)serviceProvider.GetRequiredNamedService(typeof(TService), named);
        }
    }
}