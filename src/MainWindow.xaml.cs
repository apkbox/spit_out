// --------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml.
// </summary>
// --------------------------------------------------------------------------------
namespace SpitOut
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors and Destructors

        public MainWindow()
        {
            this.DataContext = ((App)Application.Current).Model;
            this.InitializeComponent();
        }

        #endregion
    }
}
