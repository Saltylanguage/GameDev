using System;
using System.Windows.Input;

public class DelegateCommand : ICommand
{
    readonly Action execute;
    readonly Action<object> executeWithParameter;
    readonly Func<object, bool> canExecute;

    public DelegateCommand(Action execute, Func<bool> canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute == null
            ? null
            : new Func<object, bool>(_ => canExecute());
    }

    public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        executeWithParameter = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
        return canExecute == null || canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        if (!CanExecute(parameter))
        {
            return;
        }

        if (executeWithParameter != null)
        {
            executeWithParameter(parameter);
        }
        else
        {
            execute();
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
