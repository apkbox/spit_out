// --------------------------------------------------------------------------------
// <copyright file="BrowseFolderCommand.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the BrowseFolderCommand type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;

    public class BrowseFolderCommand : ICommand
    {
        #region Fields

        private readonly SelectorModel selectorModel;

        #endregion

        #region Constructors and Destructors

        public BrowseFolderCommand(SelectorModel selectorModel)
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
            var mainWindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            var dialog = new BrowseForFolderHelper();
            var result = dialog.SelectFolder(this.selectorModel.Label, this.selectorModel.TextValue, mainWindowHandle);
            if (result != null)
            {
                this.selectorModel.TextValue = result;
            }
        }

        #endregion
    }
}