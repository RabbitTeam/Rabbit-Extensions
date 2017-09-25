using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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

        [Fact]
        public void LazyResolveTest()
        {
            var services = new ServiceCollection()
                .AddInterfaceDependency()
                .AddServiceExtensions()
                .BuildServiceProvider();

            Assert.NotEqual(services.GetRequiredService<Lazy<ITestService2>>(), services.GetRequiredService<Lazy<ITestService2>>());
            Assert.NotEqual(services.GetRequiredService<Lazy<ITestService2>>().Value, services.GetRequiredService<Lazy<ITestService2>>().Value);

            Assert.NotNull(services.GetRequiredService<Lazy<IEnumerable<ITestService2>>>());
        }

        [Fact]
        public void FuncResolveTest()
        {
            var services = new ServiceCollection()
                .AddInterfaceDependency()
                .AddServiceExtensions()
                .BuildServiceProvider();

            Assert.Equal(services.GetRequiredService<Func<ITestService1>>()(), services.GetRequiredService<Func<ITestService1>>()());
            Assert.Equal(services.GetRequiredService<Func<ITestService2>>(), services.GetRequiredService<Func<ITestService2>>());
            Assert.NotEqual(services.GetRequiredService<Func<ITestService2>>()(), services.GetRequiredService<Func<ITestService2>>()());
        }
    }
}