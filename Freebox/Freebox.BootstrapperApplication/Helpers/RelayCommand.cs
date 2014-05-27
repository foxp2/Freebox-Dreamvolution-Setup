using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Freebox.BootstrapperApplication
{
    /// <summary>
    /// Implements the ICommand interface.
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Private Members

        // Action - a method that has a single parameter and does not return a value.
        private readonly Action<object> _execute;

        // Predicate - a method that has a single parameter and returns a bool.
        private readonly Predicate<object> _canExecute;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a relay command for the given action. 
        /// The command will be allways executed.
        /// </summary>
        /// <param name="executeDelegate"></param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a command for the given an action and predicate.
        /// The action 'execute' will be executed, if the predicate 'canExecute' is true.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="canExecute"></param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region Implementation of the ICommand Interface.

        /// <summary>
        /// Occurs when the CanExecute has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Gets a value indicating, whether the action of this command can be executed.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            // If there was no predicate specified for this command, then we allways are allowed to execute it.
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        /// <summary>
        /// Executes the action of this command.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        #endregion
    }

}