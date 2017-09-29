using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using System;

namespace Rabbit.Extensions.DependencyInjection.Internal
{
    public static class TypeKeyedUtilities
    {
        private static readonly INamingScope NamingScope = new NamingScope();
        private static readonly ProxyGenerationOptions ProxyGenerationOptions = new ProxyGenerationOptions();

        public static Type GetTypeKey()
        {
            var moduleScope = new ModuleScope(false, false, NamingScope, ModuleScope.DEFAULT_ASSEMBLY_NAME, ModuleScope.DEFAULT_FILE_NAME, ModuleScope.DEFAULT_ASSEMBLY_NAME, ModuleScope.DEFAULT_FILE_NAME);
            var proxyBuilder = new DefaultProxyBuilder(moduleScope);
            return proxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(typeof(ISupportKeyedService), null, ProxyGenerationOptions);
        }
    }
}