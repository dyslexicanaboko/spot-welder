using System;
using System.Collections.Generic;

namespace SpotWelder.Ui.Profile
{
  public class ConnectionStringManager
  {
    public delegate void SaveHandler(object sender, EventArgs e);

    public int MaxConnectionStrings { get; set; }

    public List<UserConnectionString> ConnectionStrings { get; set; } = new();

    public event SaveHandler? Save;

    public void Upsert(UserConnectionString target)
    {
      var inList = ConnectionStrings.Find(x => x.ConnectionString == target.ConnectionString);

      if (inList == null && target.Verified)
      {
        //If the maximum amount of connections has been reached
        if (ConnectionStrings.Count == MaxConnectionStrings)
          ConnectionStrings.RemoveAt(ConnectionStrings.Count - 1); //Then remove the last item

        //Add the new connection to the top of the list
        ConnectionStrings.Insert(0, target);

        RaiseSaveEvent();
      }
      else if (inList != null && !target.Verified)
      {
        ConnectionStrings.Remove(inList);

        RaiseSaveEvent();
      }
    }

    private void RaiseSaveEvent() => Save?.Invoke(this, EventArgs.Empty);
  }
}
