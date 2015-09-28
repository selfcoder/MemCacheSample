using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MemCache.WpfClient
{
    public class RelayAsyncCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Func<Task> _execute;
        private bool _isRunning;

        public RelayAsyncCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            private set
            {
                if (_isRunning == value)
                    return;
                _isRunning = value;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public virtual bool CanExecute(object parameter)
        {
            if (_isRunning)
                return false;
            return _canExecute == null ? true : _canExecute();
        }

        public virtual void Execute(object parameter)
        {
            IsRunning = true;
            _execute().ContinueWith(t =>
            {
                IsRunning = false;
            });
        }
    }
}