using System;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace MyHub.Commands
{
    public class RelayCommand : BaseCommand, ICommand
    {
        private readonly Action _action;

        public RelayCommand(Action action)
            : this(action, null)
        {

        }

        public RelayCommand(Action action, Func<bool> canExecute)
            : base(canExecute)
        {
            if(action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            _action = action;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }

    public class RelayCommand<T> : BaseCommand, ICommand
    {
        private readonly Action<T> _action;

        public RelayCommand(Action<T> action)
            : this(action, null)
        {

        }

        public RelayCommand(Action<T> action, Func<bool> canExecute)
            : base(canExecute)
        {
            if(action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            _action = action;
        }

        public void Execute(object parameter)
        {
            if(parameter is ItemClickEventArgs)
            {
                parameter = ((ItemClickEventArgs)parameter).ClickedItem;
            }
            if(parameter is T)
            {
                _action((T)parameter);
            }
        }
    }
}
