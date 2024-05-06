using System;
using System.Windows.Input;

namespace OrderReaderUI.Helpers;

public class RelayCommand(Action action) : ICommand
{
    // A relay command can always execute
    public bool CanExecute(object? parameter) => true;

    // Execute the events action
    public void Execute(object? parameter)
    {
        action();
    }
    
    // The event that's fired when CanExecute() value has changed
    public event EventHandler? CanExecuteChanged = (sender, args) => { };
}