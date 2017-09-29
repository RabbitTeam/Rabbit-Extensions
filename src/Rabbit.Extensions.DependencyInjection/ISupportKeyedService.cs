using System;

namespace Rabbit.Extensions.DependencyInjection
{
    public interface ISupportKeyedService
    {
        object GetKeyedService(Type serviceType, object keyed);
    }
}