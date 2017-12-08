using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Rabbit.Extensions.Boot
{
    public class RabbitBoot
    {
        public static async Task<IHostBuilder> BuildHostBuilderAsync(Action<IHostBuilder> configure = null, Func<AssemblyName, bool> assemblyPredicate = null, Func<TypeInfo, bool> typePredicate = null)
        {
            if (typePredicate == null)
                typePredicate = t => t.Name.EndsWith("Bootstrap");

            var types = GetAssemblies(assemblyPredicate).SelectMany(i => i.ExportedTypes.Select(z => z.GetTypeInfo())).Where(typePredicate);

            IHostBuilder hostBuilder = new HostBuilder();
            configure?.Invoke(hostBuilder);
            return await BuildHostBuilderAsync(hostBuilder, types);
        }

        public static async Task<IHostBuilder> BuildHostBuilderAsync(IHostBuilder hostBuilder, IEnumerable<TypeInfo> starterTypes)
        {
            if (hostBuilder == null)
                throw new ArgumentNullException(nameof(hostBuilder));
            if (starterTypes == null)
                throw new ArgumentNullException(nameof(starterTypes));

            foreach (var startMethod in starterTypes.OrderByDescending(i =>
            {
                var priorityProperty = i.GetProperty("Priority");
                if (priorityProperty == null)
                    return 20;
                return (int)priorityProperty.GetValue(null);
            }).SelectMany(GetStartMethods))
            {
                var parameters = startMethod.GetParameters().Any() ? new object[] { hostBuilder } : null;
                var result = startMethod.Invoke(null, parameters);
                if (result is Task task)
                    await task;
            }

            return hostBuilder;
        }

        private static IEnumerable<Assembly> GetAssemblies(Func<AssemblyName, bool> predicate = null)
        {
            var assemblyNames = DependencyContext.Default.RuntimeLibraries.SelectMany(i => i.GetDefaultAssemblyNames(DependencyContext.Default));
            if (predicate != null)
                assemblyNames = assemblyNames.Where(predicate).ToArray();
            var assemblies = assemblyNames.Select(i => Assembly.Load(new AssemblyName(i.Name))).ToArray();
            return assemblies;
        }

        private static IEnumerable<MethodInfo> GetStartMethods(TypeInfo starterType)
        {
            bool CheckParameters(MethodBase methodInfo)
            {
                var parameters = methodInfo.GetParameters();
                switch (parameters.Length)
                {
                    case 0:
                        return true;

                    case 1 when typeof(IHostBuilder).IsAssignableFrom(parameters[0].ParameterType):
                        return true;

                    default:
                        return false;
                }
            }

            MethodInfo GetStartMethod(string name)
            {
                var methodInfo = starterType.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
                if (methodInfo != null && CheckParameters(methodInfo))
                    return methodInfo;
                return null;
            }

            foreach (var methodInfo in new[] { "StartAsync", "Start" }.Select(GetStartMethod))
            {
                if (methodInfo != null)
                    yield return methodInfo;
            }
        }
    }
}