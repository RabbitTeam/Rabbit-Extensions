using Microsoft.Extensions.DependencyInjection;

namespace Rabbit.Extensions.DependencyInjection
{
    public interface IServiceRegister
    {
        void Register(IServiceCollection services);
    }
}