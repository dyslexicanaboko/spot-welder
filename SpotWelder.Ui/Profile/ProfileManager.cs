using System;
using SpotWelder.Lib;

namespace SpotWelder.Ui.Profile
{
  [ExcludeFromDiScan]
  public class ProfileManager
    : IProfileManager
  {
    public delegate void SaveHandler(object sender, EventArgs e);

    public ConnectionStringManager ConnectionStringManager { get; set; }

    private event SaveHandler? Save;

    public void RegisterSaveDelegate(SaveHandler saveHandler)
    {
      Save += saveHandler;

      ConnectionStringManager.Save += RaiseSaveEvent;
    }

    private void RaiseSaveEvent(object sender, EventArgs e)
    {
      Save?.Invoke(this, EventArgs.Empty);
    }
  }
}
