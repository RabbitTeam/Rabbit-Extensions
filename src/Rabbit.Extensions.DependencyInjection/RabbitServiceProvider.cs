using Microsoft.Extensions.DependencyInjection;
using Rabbit.Extensions.DependencyInjection.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Extensions.DependencyInjection
{
    public class RabbitServiceProvider : IServiceProvider, ISupportKeyedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IReadOnlyCollection<RabbitServiceDescriptor> _rabbitServiceDescriptors;
        private readonly Lazy<IServiceScopeFactory> _serviceScopeFactoryLazy;

        public RabbitServiceProvider(IServiceProvider serviceProvider, IEnumerable<ServiceDescriptor> serviceDescriptors)
        {
            _serviceProvider = serviceProvider;
            _rabbitServiceDescriptors = serviceDescriptors.OfType<RabbitServiceDescriptor>().ToArray();
            _serviceScopeFactoryLazy = new Lazy<IServiceScopeFactory>(() => new RabbitServiceScopeFactory(this, _rabbitServiceDescriptors));
        }

        #region Implementation of IServiceProvider

        /// <inheritdoc />
        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType">serviceType</paramref>.   -or-  null if there is no service object of type <paramref name="serviceType">serviceType</paramref>.</returns>
        public object GetService(Type serviceType)
        {
            var serviceKey = new ServiceKey(serviceType);
            return GetService(serviceKey);
        }

        #endregion Implementation of IServiceProvider

        #region Implementation of ISupportKeyedService

        public object GetKeyedService(Type serviceType, object keyed)
        {
            var serviceKey = new KeyedServiceKey(serviceType, keyed);
            return GetService(serviceKey);
        }

        #endregion Implementation of ISupportKeyedService

        private object GetService(IServiceKey serviceKey)
        {
            var identityType = GetIdentityType(serviceKey);

            if (identityType == typeof(IServiceScopeFactory))
                return _serviceScopeFactoryLazy.Value;

            return identityType == null ? null : _serviceProvider.GetService(identityType);
        }

        #region Private Method

        private Type GetIdentityType(IServiceKey serviceKey)
        {
            if (serviceKey is KeyedServiceKey)
            {
                var descriptor = _rabbitServiceDescriptors.LastOrDefault(i => i.ServiceKey.Equals(serviceKey));
                return descriptor?.ServiceType;
            }
            return serviceKey.ServiceType;
        }

        #endregion Private Method
    }
}