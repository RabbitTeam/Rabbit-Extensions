using Microsoft.Extensions.DependencyInjection;

namespace Rabbit.Extensions.DependencyInjection
{
    public static class ServiceCollectionContainerBuilderExtensions
    {
        public static RabbitServiceProvider BuildRabbitServiceProvider(this IServiceCollection services)
        {
            return new RabbitServiceProvider(services.BuildServiceProvider(), services);
        }
    }
}