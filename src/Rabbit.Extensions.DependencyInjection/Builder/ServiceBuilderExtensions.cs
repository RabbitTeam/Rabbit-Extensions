using Microsoft.Extensions.DependencyInjection;
using System;

namespace Rabbit.Extensions.DependencyInjection.Builder
{
    public static class ServiceBuilderExtensions
    {
        public static ServiceBuilder As<TService>(this ServiceBuilder builder)
        {
            return builder.As(typeof(TService));
        }

        public static ServiceBuilder As(this ServiceBuilder builder, Type serviceType)
        {
            builder.ServiceKeys.Add(new ServiceKey(serviceType));
            return builder;
        }

        public static ServiceBuilder Singleton(this ServiceBuilder builder)
        {
            return builder.Lifetime(ServiceLifetime.Singleton);
        }

        public static ServiceBuilder Scoped(this ServiceBuilder builder)
        {
            return builder.Lifetime(ServiceLifetime.Scoped);
        }

        public static ServiceBuilder Transient(this ServiceBuilder builder)
        {
            return builder.Lifetime(ServiceLifetime.Transient);
        }

        public static ServiceBuilder Lifetime(this ServiceBuilder builder, ServiceLifetime lifetime)
        {
            builder.Lifetime = lifetime;
            return builder;
        }

        public static ServiceBuilder Named<TService>(this ServiceBuilder builder, string named)
        {
            return builder.Named(typeof(TService), named);
        }

        public static ServiceBuilder Named(this ServiceBuilder builder, Type serviceType, string named)
        {
            return builder.Keyed(serviceType, named);
        }

        public static ServiceBuilder Keyed<TService>(this ServiceBuilder builder, object keyed)
        {
            return builder.Keyed(typeof(TService), keyed);
        }

        public static ServiceBuilder Keyed(this ServiceBuilder builder, Type serviceType, object keyed)
        {
            builder.ServiceKeys.Add(new KeyedServiceKey(serviceType, keyed));
            return builder;
        }
    }
}