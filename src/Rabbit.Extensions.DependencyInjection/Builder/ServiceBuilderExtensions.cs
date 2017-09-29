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
            builder.ServiceTypes.Add(serviceType);
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

        public static ServiceBuilder Named(this ServiceBuilder builder, string named)
        {
            return builder.Keyed(named);
        }

        public static ServiceBuilder Keyed(this ServiceBuilder builder, object keyed)
        {
            builder.Keyeds.Add(keyed);
            return builder;
        }
    }
}