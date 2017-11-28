// --------------------------------------------------------------------------------
// <copyright file="BrowseFileCommand.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the BrowseFileCommand type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using Microsoft.Win32;

    public class BrowseFileCommand : ICommand
    {
        #region Fields

        private readonly SelectorModel selectorModel;

        #endregion

        #region Constructors and Destructors

        public BrowseFileCommand(SelectorModel selectorModel)
        {
            this.selectorModel = selectorModel;
        }

        #endregion

        #region Public Events

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        #endregion

        #region Public Methods and Operators

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var v = new OpenFileDialog();
            v.ShowDialog(Application.Current.MainWindow);
            this.selectorModel.TextValue = v.FileName;
        }

        #endregion
    }
}