namespace Rabbit.Extensions.DependencyInjection.Builder
{
    public static class RabbitContainerBuilderExtensions
    {
        public static ServiceBuilder RegisterType<TImplementationType>(this RabbitContainerBuilder builder)
        {
            return builder.RegisterType(typeof(TImplementationType));
        }
    }
}