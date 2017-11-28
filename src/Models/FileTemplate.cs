// --------------------------------------------------------------------------------
// <copyright file="FileTemplate.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the FileTemplate type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using SpitOut.Annotations;

    public class FileTemplate : INotifyPropertyChanged
    {
        #region Fields

        private readonly DelegateCommand<object> showFileInWindowsExplorerCommand;

        private readonly HashSet<string> unresolvedVariables = new HashSet<string>();

        private string content;

        private string fileName;

        private bool isActive = true;

        private string name;

        #endregion

        #region Constructors and Destructors

        public FileTemplate()
        {
            this.showFileInWindowsExplorerCommand = new DelegateCommand<object>(
                this.ExecuteShowInWindowsExplorer,
                this.CanExecuteShowInWindowsExplorer);
            this.IsActiveExpr = "true";
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public string Content
        {
            get
            {
                return this.content;
            }

            set
            {
                if (value == this.content)
                {
                    return;
                }

                this.content = value;
                this.OnPropertyChanged("Content");
            }
        }

        public string ContentTemplate { get; set; }

        public bool CreateDir { get; set; }

        public string FileName
        {
            get
            {
                return this.fileName;
            }

            set
            {
                if (value == this.fileName)
                {
                    return;
                }

                this.fileName = value;
                this.OnPropertyChanged("FileName");
                if (string.IsNullOrEmpty(this.name))
                {
                    this.OnPropertyChanged("Name");
                }
            }
        }

        public string FileNameTemplate { get; set; }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                if (value.Equals(this.isActive))
                {
                    return;
                }

                this.isActive = value;

                this.OnPropertyChanged("IsActive");
            }
        }

        public string IsActiveExpr { get; set; }

        public bool IsIncomplete
        {
            get
            {
                return !this.IsResolved;
            }
        }

        public bool IsResolved
        {
            get
            {
                return this.unresolvedVariables.Count == 0;
            }
        }

        public bool IsRunnable { get; set; }

        public string Name
        {
            get
            {
                return string.IsNullOrEmpty(this.name) ? this.FileName : this.name;
            }

            set
            {
                if (value == this.name)
                {
                    return;
                }

                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        public string RunCommandCaption { get; set; }

        public string RunCommandLine { get; set; }

        public string RunExecutable { get; set; }

        public ICommand ShowFileInWindowsExplorerCommand
        {
            get
            {
                return this.showFileInWindowsExplorerCommand;
            }
        }

        public HashSet<string> UnresolvedVariables
        {
            get
            {
                return this.unresolvedVariables;
            }
        }

        public string UnresolvedVariablesList
        {
            get
            {
                return string.Join(", ", this.UnresolvedVariables);
            }
        }

        #endregion

        #region Methods

        internal void Expand(Dictionary<string, string> variables)
        {
            this.unresolvedVariables.Clear();
            this.Content = TemplateExpander.ExpandTemplate(variables, this.ContentTemplate, this.unresolvedVariables);
            this.FileName = TemplateExpander.ExpandTemplate(variables, this.FileNameTemplate, this.unresolvedVariables);
            this.OnPropertyChanged("UnresolvedVariables");
            this.OnPropertyChanged("UnresolvedVariablesList");
            this.OnPropertyChanged("IsResolved");
            this.OnPropertyChanged("IsIncomplete");
        }

        internal FileTemplate ExpandAsNew(Dictionary<string, string> variables)
        {
            var newTemplate = new FileTemplate();
            newTemplate.name = this.name;
            newTemplate.ContentTemplate = this.ContentTemplate;
            newTemplate.FileNameTemplate = this.FileNameTemplate;
            newTemplate.CreateDir = this.CreateDir;
            newTemplate.IsRunnable = this.IsRunnable;
            newTemplate.RunCommandLine = this.RunCommandLine;
            newTemplate.RunExecutable = this.RunExecutable;
            newTemplate.RunCommandCaption = this.RunCommandCaption;
            newTemplate.Expand(variables);
            newTemplate.ContentTemplate = newTemplate.content;
            newTemplate.FileNameTemplate = newTemplate.fileName;
            return newTemplate;
        }

        internal void Run(string directory)
        {
            var fullPath = Path.Combine(directory, this.FileName);
            var commandLine = string.IsNullOrWhiteSpace(this.RunCommandLine)
                                  ? fullPath
                                  : string.Format(this.RunCommandLine, fullPath);
            if (string.IsNullOrEmpty(this.RunExecutable))
            {
                Process.Start(commandLine);
            }
            else
            {
                Process.Start(this.RunExecutable, commandLine);
            }
        }

        internal void SaveFile()
        {
            if (this.FileName == null)
            {
                return;
            }

            if (this.Content == null)
            {
                return;
            }

            if (this.CreateDir)
            {
                var dirName = Path.GetDirectoryName(this.FileName);
                if (!string.IsNullOrEmpty(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
            }

            var writer = File.CreateText(this.FileName);
            writer.Write(this.Content);
            writer.Close();
            this.showFileInWindowsExplorerCommand.RaiseCanExecuteChanged();
        }

        [NotifyPropertyChangedInvocator]
#if DOTNET_45
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
#else
        protected virtual void OnPropertyChanged(string propertyName = null)
#endif
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool CanExecuteShowInWindowsExplorer(object arg)
        {
            return File.Exists(this.FileName);
        }

        private void ExecuteShowInWindowsExplorer(object obj)
        {
            if (!this.CanExecuteShowInWindowsExplorer(obj))
            {
                return;
            }

            Process.Start("explorer.exe", "/select," + Path.Combine(Directory.GetCurrentDirectory(), this.FileName));
        }

        #endregion
    }
}
