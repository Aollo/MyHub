using System;

namespace MyHub.Commands
{
    public class BaseCommand
    {
        private readonly Func<bool> _canExecute;

        protected BaseCommand(Func<bool> canExecute)
        {
            _canExecute = canExecute;
        }
        
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
