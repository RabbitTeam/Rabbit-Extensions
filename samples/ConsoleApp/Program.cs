using Microsoft.Extensions.Configuration;
using Rabbit.Extensions.Configuration;
using System;

namespace ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build()
                .EnableTemplateSupport();

            foreach (var section in configuration.GetSection("ServiceUrls").GetChildren())
            {
                Console.WriteLine(section.Value);
            }
        }
    }
}