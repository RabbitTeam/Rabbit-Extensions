using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Extensions.DependencyInjection.Builder
{
    public class ServiceBuilder
    {
        public ServiceBuilder()
        {
            Keyeds = new List<object>();
            ServiceTypes = new List<Type>();
        }

        internal Type ImplementationType { get; set; }
        internal ICollection<Type> ServiceTypes { get; set; }
        internal ServiceLifetime Lifetime { get; set; }
        internal object ImplementationInstance { get; set; }
        internal Func<IServiceProvider, object> ImplementationFactory { get; set; }
        internal ICollection<object> Keyeds { get; set; }

        internal void Build(IServiceCollection services)
        {
            var descriptors = ServiceTypes.Select(serviceType =>
              {
                  RabbitServiceDescriptor descriptor;

                  if (ImplementationFactory != null)
                      descriptor = RabbitServiceDescriptor.Create(serviceType, ImplementationFactory, Lifetime);
                  else if (ImplementationType != null)
                      descriptor = RabbitServiceDescriptor.Create(serviceType, ImplementationType, Lifetime);
                  else
                      descriptor = RabbitServiceDescriptor.Create(serviceType, ImplementationInstance);

                  descriptor.Keyeds = Keyeds.ToArray();

                  return descriptor;
              });

            foreach (var descriptor in descriptors)
            {
                services.Add(descriptor);
            }
        }
    }
}