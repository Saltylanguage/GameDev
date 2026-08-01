using System;
using System.Windows.Input;

public class DelegateCommand : ICommand
{
    private readonly Action _Execute;

    public DelegateCommand(Action execute, Func<bool> canExcute = null)
    {
        _Execute = execute;
    }

    public void Execute (object parameter)
    {
        _Execute();
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute (object paramter)
    {
        return true;
    }

    
}
