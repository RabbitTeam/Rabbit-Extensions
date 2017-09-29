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
            ServiceKeys = new List<IServiceKey>();
        }

        internal Type ImplementationType { get; set; }
        internal ICollection<IServiceKey> ServiceKeys { get; set; }
        internal ServiceLifetime Lifetime { get; set; }
        internal object ImplementationInstance { get; set; }
        internal Func<IServiceProvider, object> ImplementationFactory { get; set; }

        internal void Build(IServiceCollection services)
        {
            var descriptors = ServiceKeys.Distinct().Select(serviceKey =>
            {
                var serviceType = serviceKey.ServiceType;
                ServiceDescriptor descriptor;
                if (serviceKey is KeyedServiceKey)
                {
                    if (ImplementationFactory != null)
                        descriptor = RabbitServiceDescriptor.Create(serviceType, ImplementationFactory, Lifetime);
                    else if (ImplementationType != null)
                        descriptor = RabbitServiceDescriptor.Create(serviceType, ImplementationType, Lifetime);
                    else
                        descriptor = RabbitServiceDescriptor.Create(serviceType, ImplementationInstance);

                    ((RabbitServiceDescriptor)descriptor).ServiceKey = serviceKey;
                }
                else
                {
                    if (ImplementationFactory != null)
                        descriptor = new ServiceDescriptor(serviceType, ImplementationFactory, Lifetime);
                    else if (ImplementationType != null)
                        descriptor = new ServiceDescriptor(serviceType, ImplementationType, Lifetime);
                    else
                        descriptor = new ServiceDescriptor(serviceType, ImplementationInstance);
                }

                return descriptor;
            }).ToArray();

            foreach (var descriptor in descriptors)
            {
                services.Add(descriptor);
            }
        }
    }
}