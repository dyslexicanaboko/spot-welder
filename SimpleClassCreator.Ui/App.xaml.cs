using Microsoft.Extensions.DependencyInjection;
using SimpleClassCreator.Lib;
using SimpleClassCreator.Ui.Profile;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace SimpleClassCreator.Ui
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
      var asm = Assembly.Load("SimpleClassCreator.Ui");

      //Namespaces that must be excluded from the DI scan
      var excludeNamespaces = asm.GetTypes()
        .Where(t =>
          t.Namespace != null &&
          t.Namespace.Contains("SimpleClassCreator.Ui.ViewModels"))
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
      var asm = Assembly.Load("SimpleClassCreator.Lib");

      //Namespaces that must be excluded from the DI scan
      var excludeNamespaces = asm.GetTypes()
        .Where(t =>
          t.Namespace != null &&
          t.Namespace.Contains("SimpleClassCreator.Lib.Models"))
        .Select(t => t.Namespace)
        .Distinct()
        .ToArray();

      services.Scan(scan =>
      {
        scan.FromAssemblies(asm)
          .AddClasses(classes =>
            classes.NotInNamespaces(excludeNamespaces))
          // .WithoutAttribute<ExcludeFromDiScanAttribute>())
          .AsMatchingInterface()
          .WithScopedLifetime();
      });
    }
  }
}
