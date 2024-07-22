using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using SpotWelder.Lib;
using SpotWelder.Lib.Services.Generators;
using SpotWelder.Ui.Profile;

namespace SpotWelder.Ui
{
  /// <summary>
  ///   Interaction logic for App.xaml
  /// </summary>
  public partial class App : System.Windows.Application
  {
    private static readonly ProfileSaver ProfileSaver = new ();

    public IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      
      var serviceCollection = new ServiceCollection();

      ConfigureServices(serviceCollection);

      ServiceProvider = serviceCollection.BuildServiceProvider();

      try
      {
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();

        mainWindow.Show();
      }
      catch (Exception ex)
      {
        //Log the exception and exit
        if (true) ;
      }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
      //Library based services
      ConfigureLibServices(services);

      //WPF Forms and UI based services
      services.AddTransient<MainWindow>();

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

      //This is specifically targeting classes with an interface
      services.Scan(scan =>
      {
        scan.FromAssemblies(asm)
          .AddClasses(classes =>
            classes.NotInNamespaces(excludeNamespaces))
          // .WithoutAttribute<ExcludeFromDiScanAttribute>())
          .AsMatchingInterface()
          .WithScopedLifetime();
      });

      //This is specifically targeting classes that all share the same abstract class
      services.Scan(scan =>
      {
        scan.FromAssemblies(asm)
          .AddClasses(classes =>
            classes.AssignableTo<GeneratorBase>())
          .As<GeneratorBase>()
          .WithScopedLifetime();
      });
    }
  }
}
