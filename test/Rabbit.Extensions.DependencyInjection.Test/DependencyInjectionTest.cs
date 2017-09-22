using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Rabbit.Extensions.DependencyInjection.Test
{
    public class TestServiceRegister : IServiceRegister
    {
        #region Implementation of IServiceRegister

        public void Register(IServiceCollection services)
        {
            services.AddSingleton("test");
        }

        #endregion Implementation of IServiceRegister
    }

    public interface ITestService1 : IDependency
    {
    }

    public interface ITestService2 : ITransientDependency
    {
    }

    public interface ITestService3 : ISingletonDependency
    {
    }

    public class TestService : ITestService1, ITestService2, ITestService3
    {
    }

    public class DependencyInjectionTest
    {
        [Fact]
        public void ServiceRegisterTest()
        {
            var services = new ServiceCollection()
                .AddServiceRegister()
                .BuildServiceProvider();

            Assert.Equal("test", services.GetRequiredService<string>());
        }

        [Fact]
        public void InterfaceDependencyTest()
        {
            var services = new ServiceCollection()
                .AddInterfaceDependency()
                .BuildServiceProvider();

            Assert.Equal(services.GetRequiredService<ITestService1>().GetHashCode(), services.GetRequiredService<ITestService1>().GetHashCode());
            Assert.NotEqual(services.GetRequiredService<ITestService2>().GetHashCode(), services.GetRequiredService<ITestService2>().GetHashCode());
            Assert.Equal(services.GetRequiredService<ITestService3>().GetHashCode(), services.GetRequiredService<ITestService3>().GetHashCode());

            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                Assert.NotEqual(scope.ServiceProvider.GetRequiredService<ITestService1>().GetHashCode(), services.GetRequiredService<ITestService1>().GetHashCode());
                Assert.NotEqual(scope.ServiceProvider.GetRequiredService<ITestService2>().GetHashCode(), services.GetRequiredService<ITestService2>().GetHashCode());
                Assert.Equal(scope.ServiceProvider.GetRequiredService<ITestService3>().GetHashCode(), services.GetRequiredService<ITestService3>().GetHashCode());
            }
        }
    }
}