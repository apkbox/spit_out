// --------------------------------------------------------------------------------
// <copyright file="MainView.xaml.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Interaction logic for AppView.xaml.
// </summary>
// --------------------------------------------------------------------------------
namespace SpitOut.Views
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for AppView.
    /// </summary>
    public partial class MainView : UserControl
    {
        #region Constructors and Destructors

        public MainView()
        {
            this.InitializeComponent();
            this.DataContext = ((App)Application.Current).Model;
        }

        #endregion
    }
}
