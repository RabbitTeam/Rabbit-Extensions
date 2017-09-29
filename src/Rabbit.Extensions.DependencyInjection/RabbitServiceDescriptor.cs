using Microsoft.Extensions.DependencyInjection;
using Rabbit.Extensions.DependencyInjection.Internal;
using System;

namespace Rabbit.Extensions.DependencyInjection
{
    public class RabbitServiceDescriptor : ServiceDescriptor
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of <see cref="T:Microsoft.Extensions.DependencyInjection.ServiceDescriptor" /> with the specified <paramref name="implementationType" />.
        /// </summary>
        /// <param name="serviceType">The <see cref="T:System.Type" /> of the service.</param>
        /// <param name="implementationType">The <see cref="T:System.Type" /> implementing the service.</param>
        /// <param name="lifetime">The <see cref="T:Microsoft.Extensions.DependencyInjection.ServiceLifetime" /> of the service.</param>
        public RabbitServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime) : base(serviceType, implementationType, lifetime)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of <see cref="T:Microsoft.Extensions.DependencyInjection.ServiceDescriptor" /> with the specified <paramref name="instance" />
        /// as a <see cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton" />.
        /// </summary>
        /// <param name="serviceType">The <see cref="T:System.Type" /> of the service.</param>
        /// <param name="instance">The instance implementing the service.</param>
        public RabbitServiceDescriptor(Type serviceType, object instance) : base(serviceType, instance)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of <see cref="T:Microsoft.Extensions.DependencyInjection.ServiceDescriptor" /> with the specified <paramref name="factory" />.
        /// </summary>
        /// <param name="serviceType">The <see cref="T:System.Type" /> of the service.</param>
        /// <param name="factory">A factory used for creating service instances.</param>
        /// <param name="lifetime">The <see cref="T:Microsoft.Extensions.DependencyInjection.ServiceLifetime" /> of the service.</param>
        public RabbitServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime) : base(serviceType, factory, lifetime)
        {
        }

        public object[] Keyeds { get; set; }
        internal Type RealType { get; set; }

        public static RabbitServiceDescriptor Create(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            return Guarantee(serviceType, new RabbitServiceDescriptor(TypeKeyedUtilities.GetTypeKey(), implementationType, lifetime));
        }

        public static RabbitServiceDescriptor Create(Type serviceType, object instance)
        {
            return Guarantee(serviceType, new RabbitServiceDescriptor(TypeKeyedUtilities.GetTypeKey(), instance));
        }

        public static RabbitServiceDescriptor Create(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime)
        {
            return Guarantee(serviceType, new RabbitServiceDescriptor(TypeKeyedUtilities.GetTypeKey(), factory, lifetime));
        }

        private static RabbitServiceDescriptor Guarantee(Type realType, RabbitServiceDescriptor rabbitServiceDescriptor)
        {
            rabbitServiceDescriptor.RealType = realType;
            return rabbitServiceDescriptor;
        }

        public static RabbitServiceDescriptor Create(ServiceDescriptor descriptor)
        {
            RabbitServiceDescriptor newDescriptor;

            if (descriptor.ImplementationFactory != null)
                newDescriptor = Create(descriptor.ServiceType, descriptor.ImplementationFactory, descriptor.Lifetime);
            else if (descriptor.ImplementationType != null)
                newDescriptor = Create(descriptor.ServiceType, descriptor.ImplementationType, descriptor.Lifetime);
            else
                newDescriptor = Create(descriptor.ServiceType, descriptor.ImplementationInstance);

            return newDescriptor;
        }
    }
}