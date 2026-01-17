using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SpotWelder.Ui.Containers;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace SpotWelder.Ui
{
  /// <summary>
  ///   Interaction logic for App.xaml
  /// </summary>
  public partial class App : System.Windows.Application
  {

    [DllImport("kernel32.dll")]
    private static extern bool AttachConsole(int dwProcessId);

    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern bool FreeConsole();

    private const int AttachParentProcess = -1;

    [STAThread]
    public static void Main(string[] args)
    {
      //Debug_YamlBulkGen(ref args);

      // Run CLI mode - If arguments are provided it's CLI mode
      if (args.Length > 0)
      {
        // Attach to the parent console or allocate a new one
        if (!AttachConsole(AttachParentProcess))
        {
          AllocConsole();
        }

        var exitCode = RunCliMode(args);

        FreeConsole();

        Environment.Exit(exitCode);

        return;
      }

      // Run GUI mode
      var app = new App();
      app.InitializeComponent();
      app.Run();
    }

    private static int RunCliMode(string[] args)
    {
      var exitCode = 1; // Default to error

      CommonHarness(
        true,
        serviceProvider =>
        {
          var cli = serviceProvider.GetRequiredService<CliMode>();
          
          exitCode = cli.ProcessFile(args[0]);
        });

      return exitCode;
    }

    //GUI Mode: Called by app.Run();
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      CommonHarness(
        false,
        serviceProvider =>
        {
          var mainWindow = serviceProvider.GetRequiredService<MainWindow>();

          mainWindow.PositionWindowOnActiveMonitor();

          mainWindow.Show();

          //Debug_ResultWindow();
          //Debug_ParentResultWindow();
        });
    }

    /// <summary>
    /// Common actions that need to be taken before running the chosen mode.
    /// </summary>
    /// <param name="enableConsoleLogging"></param>
    /// <param name="modeActions">Chosen mode's execution logic</param>
    private static void CommonHarness(bool enableConsoleLogging, Action<ServiceProvider> modeActions)
    {
      var serviceCollection = ConfigureDependencyInjection.ConfigureServices(enableConsoleLogging);

      var serviceProvider = serviceCollection.BuildServiceProvider();

      Log.Logger = serviceProvider.GetRequiredService<ILogger>();

      try
      {
        modeActions(serviceProvider);
      }
      catch (Exception ex)
      {
        //Log the exception and exit
        Log.Error(ex, "Unhandled error");
      }
      finally
      {
        //TODO: I have no idea why the exe isn't quitting after finishing the CLI mode.
        // Therefore, I'm adding this hacky prompt to let the user know to press a key to exit.
        if (enableConsoleLogging) Log.Information("Press any key to exit");
        
        Log.CloseAndFlush();
      }
    }
  }
}
