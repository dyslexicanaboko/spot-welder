using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.DataAccess.SqlClients;
using SpotWelder.Lib.Services;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.TableQueryFormats;
using SpotWelder.Ui.Helpers;
using SpotWelder.Ui.Profile;
using SpotWelder.Ui.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace SpotWelder.Ui
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    : Window
  {
    private readonly List<IUsesResultWindow> _hasResultWindows;

    public MainWindow(
      ITableQueryFormatFactory tableQueryFormatFactory,
      IQueryToClassService queryToClassService,
      IQueryToMockDataService queryToMockDataService,
      IGeneralDatabaseQueries repository,
      IProfileManager profileManager,
      IDtoGenerator dtoGenerator,
      IMetaViewModelService metaViewModelService,
      ICSharpCompilerService compilerService,
      IConnectionStringBuilderService builderService)
    {
      InitializeComponent();

      //I am probably doing this wrong, but I don't care right now. I will have to circle back and try to do it right later.
      //The MVVM model seems like a lot of extra unnecessary work.
      CtrlQueryToClass.Dependencies(
        tableQueryFormatFactory,
        queryToClassService,
        repository,
        profileManager,
        builderService);

      CtrlQueryToMockData.Dependencies(
        tableQueryFormatFactory,
        queryToMockDataService,
        repository,
        profileManager,
        builderService);

      CtrlDtoMaker.Dependencies(
        dtoGenerator,
        metaViewModelService,
        queryToClassService,
        compilerService);

      _hasResultWindows = new List<IUsesResultWindow> { CtrlQueryToMockData, CtrlDtoMaker, CtrlQueryToClass };
    }

    private void Window_Closing(object sender, CancelEventArgs e)
      => _hasResultWindows.ForEach(x => x.CloseResultWindows());

    private void btnAbout_Click(object sender, RoutedEventArgs e)
    {
      var obj = new AboutForm();

      obj.ShowDialog();
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);

      Application.Current.Shutdown();
    }
  }
}
