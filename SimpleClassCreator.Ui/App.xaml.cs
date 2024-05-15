using System;
using System.Windows;
using SimpleClassCreator.Lib.DataAccess;
using SimpleClassCreator.Lib.Services;
using SimpleClassCreator.Lib.Services.CodeFactory;
using SimpleClassCreator.Ui.Profile;
using SimpleClassCreator.Ui.Services;
using SimpleInjector;

namespace SimpleClassCreator.Ui
{
  /// <summary>
  ///   Interaction logic for App.xaml
  /// </summary>
  public partial class App : System.Windows.Application
  {
    private static ProfileSaver _profileSaver;

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      _profileSaver = new ProfileSaver();

      var container = Bootstrap();

      // Any additional other configuration, e.g. of your desired MVVM toolkit.

      try
      {
        var mainWindow = container.GetInstance<MainWindow>();

        mainWindow.Show();
      }
      catch (Exception ex)
      {
        //Log the exception and exit
        if (true) ;
      }
    }

    private static Container Bootstrap()
    {
      // Create the container as usual.
      var container = new Container();

      // Register your types, for instance:
      container.Register<IMetaViewModelService, MetaViewModelService>();
      container.Register<IGeneralDatabaseQueries, GeneralDatabaseQueries>();
      container.Register<IQueryToClassRepository, QueryToClassRepository>();
      container.Register<IDtoGenerator, DtoGenerator>();
      container.Register<INameFormatService, NameFormatService>();
      container.Register<IQueryToClassService, QueryToClassService>();
      container.Register<IQueryToMockDataService, QueryToMockDataService>();
      container.Register<IProfileManager>(GetProfileManager, Lifestyle.Singleton);
      container.Register<ICSharpCompilerService, CSharpCompilerService>();

      // Register your windows and view models:
      //container.Register<DtoMakerControl>();
      //container.Register<QueryToClassControl>();
      container.Register<MainWindow>();

      container.Verify();

      return container;
    }

    private static ProfileManager GetProfileManager()
    {
      var profileManager = _profileSaver.Load();

      return profileManager;
    }
  }
}
