using System;
using System.Windows.Input;

namespace OrderReader.Helpers;

public class RelayParameterizedCommand(Action<object> action) : ICommand
{
    // A relay command can always execute
    public bool CanExecute(object? parameter) => true;

    // Execute the events action
    public void Execute(object? parameter)
    {
        if (parameter is null) return;
        action(parameter);
    }

    // The event that's fired when CanExecute() value has changed
    public event EventHandler? CanExecuteChanged = (sender, args) => { };
}