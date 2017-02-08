// --------------------------------------------------------------------------------
// <copyright file="DelegateCommand.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the DelegateCommand type.
// </summary>
// --------------------------------------------------------------------------------
namespace SpitOut.Models
{
    using System;
    using System.Windows.Input;

    public class DelegateCommand<T> : ICommand
    {
        #region Fields

        private readonly Func<T, bool> canExecuteMethod;

        private readonly Action<T> executeMethod;

        #endregion

        #region Constructors and Destructors

        public DelegateCommand(Action<T> executeMethod)
        {
            this.executeMethod = executeMethod;
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        #endregion

        #region Public Events

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Public Methods and Operators

        public bool CanExecute(object parameter)
        {
            var method = this.canExecuteMethod;
            return method == null || method((T)parameter);
        }

        public void Execute(object parameter)
        {
            var method = this.executeMethod;
            if (method != null)
            {
                method((T)parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged();
        }

        #endregion

        #region Methods

        private void OnCanExecuteChanged()
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
