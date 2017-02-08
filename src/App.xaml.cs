// --------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Interaction logic for App.xaml.
// </summary>
// --------------------------------------------------------------------------------
namespace SpitOut
{
    using System;
    using System.IO;
    using System.Windows;

    using Microsoft.Win32;

    using SpitOut.Models;

    /// <summary>
    /// Interaction logic for App.
    /// </summary>
    public partial class App : Application
    {
        #region Public Properties

        public ConfigModel Model { get; private set; }

        #endregion

        #region Methods

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var configFile = "SpitOut.xml";

            if (e.Args.Length == 0)
            {
                if (!File.Exists(configFile))
                {
                    var ofd = new OpenFileDialog();
                    ofd.Multiselect = false;
                    ofd.DefaultExt = "xml";
                    ofd.Filter = "XML Document (*.xml)|*.xml|All Files|*.*";
                    ofd.InitialDirectory = Directory.GetCurrentDirectory();
                    if (ofd.ShowDialog() != true)
                    {
                        this.Shutdown(1);
                        return;
                    }

                    configFile = ofd.FileName;
                }
            }
            else
            {
                configFile = e.Args[0];
            }

            try
            {
                this.Model = XmlConfigLoader.LoadFromFile(configFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Shutdown(1);
                return;
            }

            this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
        }

        #endregion
    }
}
