using SpotWelder.Ui.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SpotWelder.Ui.Services
{
  public class VersionInfoService
  {
    private readonly Assembly _assembly;

    public ObservableCollection<VersionInfoViewModel> Properties { get; set; } = new ();

    public const string Website = "http://obscureproblemsandgotchas.com/dyslexicapps";
    public const string GitHub = "https://github.com/dyslexicanaboko/spot-welder";

    public VersionInfoService()
    {
       _assembly = Assembly.GetExecutingAssembly();
    }

    public void LoadVersionInfo()
    {
      Add("Product", GetAssemblyAttribute<AssemblyProductAttribute>("Product"));
      Add("Version", _assembly.GetName().Version.ToString());
      Add("Company", GetAssemblyAttribute<AssemblyCompanyAttribute>("Company"));
      Add("Copyright", GetAssemblyAttribute<AssemblyCopyrightAttribute>("Copyright"));
      Add("Assembly", GetAssemblyTitleAttribute());
      Add("Description", GetAssemblyAttribute<AssemblyDescriptionAttribute>("Description"));
    }

    private void Add(string key, string value)
      => Properties.Add(new VersionInfoViewModel(key, value));

    private string GetAssemblyTitleAttribute()
    {
      var title = GetAssemblyAttribute<AssemblyTitleAttribute>("Title");

      return title != string.Empty ? 
        title : 
        Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
    }

    private string GetAssemblyAttribute<T>(string propertyName)
    {
      var attributes = _assembly.GetCustomAttributes(typeof(T), false);

      if (!attributes.Any()) return string.Empty;

      var t = ((T)attributes.First());

      return Convert.ToString(t!.GetType().GetProperty(propertyName)?.GetValue(t)) ?? string.Empty;
    }
  }
}
