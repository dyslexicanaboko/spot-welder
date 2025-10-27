using Microsoft.Extensions.DependencyInjection;
using SpotWelder.Lib.Services.Generators;
using System;
using System.Reflection;

namespace SpotWelder.IntegrationTests.Common
{
  internal class TestContainer
  {
    public IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    ///   This is a container for integration tests. It can be used to store shared state or configuration
    ///   that is needed across multiple tests.
    /// </summary>
    public TestContainer()
    {
      var asm = Assembly.Load("SpotWelder.Lib");

      var serviceCollection = new ServiceCollection();

      //Targeting classes that all extend the `GeneratorBase` abstract class
      //This will load IEnumerable<GeneratorBase> into the `CodeGenerationFactory` constructor
      serviceCollection.Scan(scan =>
      {
        scan.FromAssemblies(asm)
          .AddClasses(classes =>
            classes.AssignableTo<GeneratorBase>())
          .As<GeneratorBase>()
          .WithScopedLifetime();
      });

      ServiceProvider = serviceCollection.BuildServiceProvider();
    }
  }
}
