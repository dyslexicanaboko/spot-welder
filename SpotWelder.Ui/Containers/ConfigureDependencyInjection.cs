using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SpotWelder.Lib;
using SpotWelder.Lib.Services.Generators;
using SpotWelder.Lib.Services.TableQueryFormats;
using SpotWelder.Ui.Profile;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SpotWelder.Ui.Containers
{
  public static class ConfigureDependencyInjection
  {
    private static readonly ProfileSaver ProfileSaver = new();

    public static IServiceCollection ConfigureServices(bool enableConsoleLogging)
    {
      var services = new ServiceCollection();

      //Top level services
      services.AddSerilog(configureLogger =>
        {
          var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Log.log");

          configureLogger
            .MinimumLevel.Debug()
            .WriteTo.File(path, rollingInterval: RollingInterval.Day);

          //Only need this when CLI is involved
          if (enableConsoleLogging) configureLogger.WriteTo.Console();

          #if DEBUG
          configureLogger.WriteTo.Seq("http://localhost:5341");
          #endif
        })
        .AddLogging();

      //Library based services
      ConfigureLibServices(services);

      //WPF Forms and UI based services
      services.AddTransient<MainWindow>();
      //CLI mode
      services.AddTransient<CliMode>();

      //Scanned services
      var asm = Assembly.Load("SpotWelder.Ui");

      //Namespaces that must be excluded from the DI scan
      var excludeNamespaces = asm.GetTypes()
        .Where(t =>
          t.Namespace != null &&
          t.Namespace.Contains("SpotWelder.Ui.ViewModels"))
        .Select(t => t.Namespace)
        .Distinct()
        .ToArray();

      services.Scan(scan =>
      {
        scan.FromAssemblies(asm)
          .AddClasses(classes =>
            classes.NotInNamespaces(excludeNamespaces)
              .WithoutAttribute<ExcludeFromDiScanAttribute>())
          .AsMatchingInterface()
          .WithScopedLifetime();
      });

      //This is to initialize all of the "Dependencies" objects that are used for child controls
      services.Scan(scan =>
      {
        scan.FromAssemblies(asm)
          .AddClasses(classes =>
            classes.InNamespaces("SpotWelder.Ui.Controls"))
          .AsSelf()
          .WithScopedLifetime();
      });

      return services;
    }

    private static void ConfigureLibServices(IServiceCollection services)
    {
      //Common services
      services.AddSingleton<IProfileManager>(_ => ProfileSaver.Load());

      //Scanned services
      var asm = Assembly.Load("SpotWelder.Lib");

      //Namespaces that must be excluded from the DI scan
      var excludeNamespaces = asm.GetTypes()
        .Where(t =>
          t.Namespace != null &&
          t.Namespace.Contains("SpotWelder.Lib.Models"))
        .Select(t => t.Namespace)
        .Distinct()
        .ToArray();

      //Targeting classes with an exactly one matching interface - as in: IClassName -> ClassName
      services.Scan(scan =>
      {
        scan.FromAssemblies(asm)
          .AddClasses(classes =>
            classes.NotInNamespaces(excludeNamespaces))

          // .WithoutAttribute<ExcludeFromDiScanAttribute>())
          .AsMatchingInterface()
          .WithScopedLifetime();
      });

      //Targeting classes that all extend the `GeneratorBase` abstract class
      //This will load IEnumerable<GeneratorBase> into the `CodeGenerationFactory` constructor
      services.Scan(scan =>
      {
        scan.FromAssemblies(asm)
          .AddClasses(classes =>
            classes.AssignableTo<GeneratorBase>())
          .As<GeneratorBase>()
          .WithScopedLifetime();
      });

      //Targeting classes that all implement the `ITableQueryFormatStrategy` interface
      //This will load IEnumerable<ITableQueryFormatStrategy> into the `TableQueryFormatFactory` constructor
      services.Scan(scan =>
      {
        scan.FromAssemblies(asm)
          .AddClasses(classes =>
            classes.AssignableTo<ITableQueryFormatStrategy>())
          .As<ITableQueryFormatStrategy>()
          .WithScopedLifetime();
      });
    }
  }
}
