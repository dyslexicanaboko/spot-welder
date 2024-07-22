using System;
using System.Windows.Input;

namespace SpotWelder.Ui.ViewModels
{
  //Produced by CoPilot for the purpose of being able to close tabs in the UI
  public class RelayCommand<T> : ICommand
  {
    private readonly Action<T> _execute;
    private readonly Func<T, bool>? _canExecute;

    public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
    {
      _execute = execute ?? throw new ArgumentNullException(nameof(execute));
      
      _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
      if (parameter == null) return false;
      
      return _canExecute == null || _canExecute((T)parameter);
    }

    public void Execute(object? parameter)
    {
      if (parameter == null) return;

      _execute((T)parameter);
    }

    public event EventHandler? CanExecuteChanged
    {
      add => CommandManager.RequerySuggested += value;
      
      remove => CommandManager.RequerySuggested -= value;
    }
  }

}
