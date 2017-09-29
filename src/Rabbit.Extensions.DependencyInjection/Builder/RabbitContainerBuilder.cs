using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Rabbit.Extensions.DependencyInjection.Builder
{
    public class RabbitContainerBuilder
    {
        private readonly ICollection<ServiceBuilder> _builders = new List<ServiceBuilder>();

        public ServiceBuilder RegisterType(Type implementationType)
        {
            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            var item = new ServiceBuilder
            {
                ImplementationType = implementationType
            };
            _builders.Add(item);
            return item;
        }

        public ServiceBuilder RegisterInstance(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var item = new ServiceBuilder
            {
                ImplementationInstance = instance
            };
            _builders.Add(item);
            return item;
        }

        public ServiceBuilder Register(Func<IServiceProvider, object> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var item = new ServiceBuilder
            {
                ImplementationFactory = factory
            };
            _builders.Add(item);
            return item;
        }

        public void Build(IServiceCollection services)
        {
            foreach (var builder in _builders)
            {
                builder.Build(services);
            }
        }
    }
}