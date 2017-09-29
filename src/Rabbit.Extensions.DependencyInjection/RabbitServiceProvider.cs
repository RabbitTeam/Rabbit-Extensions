using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Extensions.DependencyInjection
{
    public class RabbitServiceProvider : IServiceProvider, ISupportKeyedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<ServiceKey, RabbitServiceDescriptor> _keyedServiceDescriptors = new Dictionary<ServiceKey, RabbitServiceDescriptor>();

        public RabbitServiceProvider(IServiceProvider serviceProvider, IEnumerable<ServiceDescriptor> serviceDescriptors)
        {
            _serviceProvider = serviceProvider;
            foreach (var rabbitServiceDescriptor in serviceDescriptors.OfType<RabbitServiceDescriptor>().Where(i => i.Keyeds.Any()))
            {
                foreach (var keyed in rabbitServiceDescriptor.Keyeds)
                {
                    var serviceKey = new ServiceKey(rabbitServiceDescriptor.RealType, keyed);
                    if (_keyedServiceDescriptors.ContainsKey(serviceKey))
                        continue;
                    _keyedServiceDescriptors.Add(serviceKey, rabbitServiceDescriptor);
                }
            }
        }

        #region Implementation of IServiceProvider

        /// <inheritdoc />
        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType">serviceType</paramref>.   -or-  null if there is no service object of type <paramref name="serviceType">serviceType</paramref>.</returns>
        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        #endregion Implementation of IServiceProvider

        #region Implementation of ISupportKeyedService

        public object GetKeyedService(Type serviceType, object keyed)
        {
            var key = new ServiceKey(serviceType, keyed);
            if (!_keyedServiceDescriptors.TryGetValue(key, out var keyedServiceDescriptor))
                return null;
            return keyedServiceDescriptor == null ? null : _serviceProvider.GetService(keyedServiceDescriptor.ServiceType);
        }

        #endregion Implementation of ISupportKeyedService

        #region Help Type

        internal struct ServiceKey
        {
            public Type ServiceType { get; }
            public object Keyed { get; }

            public ServiceKey(Type serviceType, object keyed)
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
                if (obj is ServiceKey serviceKey)
                    return Equals(serviceKey);
                return base.Equals(obj);
            }

            public bool Equals(ServiceKey other)
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

            #endregion Overrides of ValueType
        }

        #endregion Help Type
    }
}