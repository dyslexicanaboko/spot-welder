using SpotWelder.Ui.Services;
using SpotWelder.Ui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpotWelder.Ui
{
  /// <summary>
  /// Interaction logic for AboutForm.xaml
  /// </summary>
  public partial class AboutForm : Window
  {
    private VersionInfoService _versionInfoService;

    public AboutForm()
    {
      InitializeComponent();

      _versionInfoService = new VersionInfoService();
      _versionInfoService.LoadVersionInfo();

      DataContext = _versionInfoService;
    }
  }
}
