using Microsoft.Extensions.DependencyInjection;
using Rabbit.Extensions.DependencyInjection.Internal;
using System;

namespace Rabbit.Extensions.DependencyInjection
{
    public interface IServiceKey
    {
        Type ServiceType { get; }
    }

    public struct KeyedServiceKey : IServiceKey
    {
        public Type ServiceType { get; }
        public object Keyed { get; }

        public KeyedServiceKey(Type serviceType, object keyed)
        {
            ServiceType = serviceType;
            Keyed = keyed;
        }

        #region Overrides of ValueType

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>true if <paramref name="obj">obj</paramref> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is KeyedServiceKey serviceKey)
                return Equals(serviceKey);
            return base.Equals(obj);
        }

        public bool Equals(KeyedServiceKey other)
        {
            return ServiceType == other.ServiceType && Equals(Keyed, other.Keyed);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((ServiceType != null ? ServiceType.GetHashCode() : 0) * 397) ^ (Keyed != null ? Keyed.GetHashCode() : 0);
            }
        }

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>The fully qualified type name.</returns>
        public override string ToString()
        {
            return $"ServiceType:{ServiceType},Keyed:{Keyed}";
        }

        #endregion Overrides of ValueType
    }

    public struct ServiceKey : IServiceKey
    {
        public ServiceKey(Type serviceType)
        {
            ServiceType = serviceType;
        }

        #region Implementation of IServiceKey

        public Type ServiceType { get; }

        #endregion Implementation of IServiceKey

        #region Overrides of ValueType

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>true if <paramref name="obj">obj</paramref> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ServiceKey serviceKey)
                return Equals(serviceKey);
            return base.Equals(obj);
        }

        public bool Equals(ServiceKey other)
        {
            return ServiceType == other.ServiceType;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return ServiceType != null ? ServiceType.GetHashCode() : 0;
        }

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>The fully qualified type name.</returns>
        public override string ToString()
        {
            return $"ServiceType:{ServiceType}";
        }

        #endregion Overrides of ValueType
    }

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

        public IServiceKey ServiceKey { get; set; }

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
            //            rabbitServiceDescriptor.RealType = realType;
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