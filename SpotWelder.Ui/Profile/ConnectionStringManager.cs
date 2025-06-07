using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpotWelder.Ui.Profile
{
  //Make sure to only call this class's constructs with `Application.Current.Dispatcher.Invoke(() => ...);`
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
      /* If you get an exception here:
       * The exception occurs because the ConnectionStrings collection, an ObservableCollection, is being modified 
       * (Remove) from a thread that is not the Dispatcher thread. This violates WPF's threading model, which 
       * requires UI-bound collections to be accessed only from the Dispatcher thread.
         
         This is the fix:
         Application.Current.Dispatcher.Invoke(() => ConnectionStrings.Remove(target));

         If this keeps happening in the future I recommend putting in a fail safe that will tell 
         you if you are executing on the wrong thread explicitly. */
      ConnectionStrings.Remove(target);

      RaiseSaveEvent();
    }

    private void RaiseSaveEvent() => Save?.Invoke(this, EventArgs.Empty);
  }
}
