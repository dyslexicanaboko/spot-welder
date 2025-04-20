using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpotWelder.Ui.Profile
{
  public class ConnectionStringManager
  {
    public delegate void SaveHandler(object sender, EventArgs e);

    public int MaxConnectionStrings { get; set; }

    public ObservableCollection<UserConnectionString> ConnectionStrings { get; set; } = new ();

    public event SaveHandler? Save;

    public void Upsert(UserConnectionString target)
    {
      var inList = ConnectionStrings.SingleOrDefault(x => x == target);

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
        Remove(inList);
      }
    }

    public void Remove(UserConnectionString target)
    {
      //TODO: When a connection string fails the connection test, this code is triggered and it is failing.
      //Easiest way to reproduce is to just shut down the database server. So in the case of Postgres, turn off the container.
      //And then press the "Test" button after running the application.
      ConnectionStrings.Remove(target);

      RaiseSaveEvent();
    }

    private void RaiseSaveEvent() => Save?.Invoke(this, EventArgs.Empty);
  }
}
