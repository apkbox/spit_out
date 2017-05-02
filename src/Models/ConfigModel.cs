// --------------------------------------------------------------------------------
// <copyright file="ConfigModel.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the ConfigModel type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;

    using SpitOut.Annotations;
    using SpitOut.Models.Layout;

    public class ConfigModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly DelegateCommand<object> runCommand;

        private readonly DelegateCommand<object> saveAllCommand;

        private QuickpickModel selectedQuickpick;

        private FileTemplate selectedTemplate;

        private SelectorsVisibility selectorsUi;

        private string title;

        #endregion

        #region Constructors and Destructors

        public ConfigModel()
        {
            this.saveAllCommand = new DelegateCommand<object>(this.ExecuteSaveAll, this.CanExecuteSaveAll);
            this.runCommand = new DelegateCommand<object>(this.ExecuteRun, this.CanExecuteRun);
            this.Quickpicks = new List<QuickpickModel>();
            this.Selectors = new List<SelectorModel>();
            this.Filesets = new List<Fileset>();
            this.Templates = new List<FileTemplate>();
            this.WindowWidth = 800;
            this.WindowHeight = 600;
            this.SelectorsUi = SelectorsVisibility.Visible;

            this.QuickpicksWidth = double.NaN;

            this.AutoLayoutSelectors = new List<SelectorModel>();
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public List<SelectorModel> AutoLayoutSelectors { get; private set; }

        public string ConfigName { get; internal set; }

        public List<Fileset> Filesets { get; private set; }

        public GroupingOrder GroupBy { get; internal set; }

        public bool HasQuickpicksLabel
        {
            get
            {
                return !string.IsNullOrEmpty(this.QuickpicksLabel);
            }
        }

        public bool IsCurrentTemplateRunnable
        {
            get
            {
                return this.SelectedTemplate != null && this.SelectedTemplate.IsRunnable;
            }
        }

        public LayoutNode Layout { get; internal set; }

        public List<QuickpickModel> Quickpicks { get; private set; }

        public string QuickpicksLabel { get; internal set; }

        public double QuickpicksWidth { get; internal set; }

        public string RunButtonCaption
        {
            get
            {
                return this.SelectedTemplate == null ? null : this.SelectedTemplate.RunCommandCaption;
            }
        }

        public ICommand RunCommand
        {
            get
            {
                return this.runCommand;
            }
        }

        public ICommand SaveAllCommand
        {
            get
            {
                return this.saveAllCommand;
            }
        }

        public QuickpickModel SelectedQuickpick
        {
            get
            {
                return this.selectedQuickpick;
            }

            set
            {
                if (value == this.selectedQuickpick)
                {
                    return;
                }

                this.selectedQuickpick = value;
                this.OnQuickpickChanged();
                this.OnPropertyChanged();
            }
        }

        public FileTemplate SelectedTemplate
        {
            get
            {
                return this.selectedTemplate;
            }

            set
            {
                if (value == this.selectedTemplate)
                {
                    return;
                }

                this.selectedTemplate = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("RunButtonCaption");
                this.OnPropertyChanged("IsCurrentTemplateRunnable");
                this.runCommand.RaiseCanExecuteChanged();
            }
        }

        public List<SelectorModel> Selectors { get; private set; }

        public bool SelectorsPanelExpanded { get; set; }

        public SelectorsVisibility SelectorsUi
        {
            get
            {
                return this.selectorsUi;
            }

            internal set
            {
                this.selectorsUi = value;
                this.SelectorsPanelExpanded = this.SelectorsUi == SelectorsVisibility.Expanded
                                              || this.selectorsUi == SelectorsVisibility.Visible;
            }
        }

        public bool ShowQuickpicks
        {
            get
            {
                return this.Quickpicks.Count > 0;
            }
        }

        public bool ShowSelectors
        {
            get
            {
                return this.SelectorsUi != SelectorsVisibility.Hidden;
            }
        }

        public bool ShowSelectorsExpander
        {
            get
            {
                return this.ShowQuickpicks
                       && (this.selectorsUi == SelectorsVisibility.Collapsed
                           || this.selectorsUi == SelectorsVisibility.Expanded);
            }
        }

        public List<FileTemplate> Templates { get; private set; }

        public string Title
        {
            get
            {
                return this.title ?? this.ConfigName;
            }

            internal set
            {
                this.title = value;
            }
        }

        public double WindowHeight { get; internal set; }

        public double WindowWidth { get; internal set; }

        #endregion

        #region Methods

        internal void FinalizeLoading()
        {
            this.CompleteLayoutGeneration();
            this.GenerateResultingTemplateSet();
            
            // Force initial template expansion
            this.NotifySelectionChanged();
        }

        internal void NotifySelectionChanged()
        {
            // The first round is used to determine which selectors are active now.
            var variables = new Dictionary<string, string>();

            foreach (var selector in this.Selectors)
            {
                // Update selector active status. Note that the first selector cannot use variables (there are none),
                // and each subsequent selector can only reference variables from previous selectors.
                selector.IsActive = this.CalculateIsActiveStatus(variables, selector.IsActiveExpr ?? "false");

                // Inactive selector cannot influence variables.
                if (!selector.IsActive)
                {
                    continue;
                }

                var v = selector.SelectedChoice;
                if (v != null)
                {
                    foreach (var variable in v.Variables)
                    {
                        if (variable.Value.IsExpression)
                        {
                            variables[variable.Key] = TemplateExpander.ExpandTemplate(
                                variables,
                                variable.Value.Value,
                                null);
                        }
                        else
                        {
                            variables[variable.Key] = variable.Value.Value;
                        }
                    }
                }
            }

            foreach (var tpl in this.Templates)
            {
                tpl.IsActive = this.CalculateIsActiveStatus(variables, tpl.IsActiveExpr);
                tpl.Expand(variables);
            }

            if (this.SelectedTemplate == null ||!this.SelectedTemplate.IsActive)
            { 
                this.SelectedTemplate = this.Templates.FirstOrDefault();
            }

            this.saveAllCommand.RaiseCanExecuteChanged();
            this.runCommand.RaiseCanExecuteChanged();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool CalculateIsActiveStatus(Dictionary<string, string> variables, string expr)
        {
            // Update selector active status. Note that the first selector cannot use variables (there are none),
            // and each subsequent selector can only reference variables from previous selectors.
            var normVars = this.CreateBooleanNormalizedList(variables);
            var expandedExpr = TemplateExpander.ExpandTemplate(normVars, expr, null);
            expandedExpr = TemplateExpander.ReplaceUnresolved(expandedExpr, "false");
            return ExpressionEvaluator.Evaluate(expandedExpr);
        }

        private bool CanExecuteRun(object arg)
        {
            if (this.SelectedTemplate == null)
            {
                return false;
            }

            if (!this.SelectedTemplate.IsRunnable)
            {
                return false;
            }

            if (this.SelectedTemplate.IsIncomplete)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(this.SelectedTemplate.FileName))
            {
                return false;
            }

            if (!File.Exists(this.SelectedTemplate.FileName))
            {
                return false;
            }

            return true;
        }

        private bool CanExecuteSaveAll(object arg)
        {
            return this.Templates.Where(o => o.IsActive).All(o => o.IsResolved && !string.IsNullOrWhiteSpace(o.FileName));
        }

        private void CompleteLayoutGeneration()
        {
            if (this.Layout == null)
            {
                this.AutoLayoutSelectors.AddRange(this.Selectors);
            }
            else
            {
                this.ResolveSelectorReferencesRecursively(this.Layout as LayoutContainerNode);
                this.AutoLayoutSelectors.AddRange(this.Selectors.Where(o => o.IsInLayout == false));
            }
        }

        private Dictionary<string, string> CreateBooleanNormalizedList(Dictionary<string, string> variables)
        {
            var normVars = new Dictionary<string, string>();
            foreach (var v in variables)
            {
                bool result;
                if (bool.TryParse(v.Value, out result))
                {
                    normVars.Add(v.Key, v.Value);
                }
                else if (!string.IsNullOrEmpty(v.Value))
                {
                    normVars.Add(v.Key, "true");
                }
                else
                {
                    normVars.Add(v.Key, "false");
                }
            }

            return normVars;
        }

        private void ExecuteRun(object obj)
        {
            if (!this.CanExecuteRun(obj))
            {
                return;
            }

            this.SelectedTemplate.Run(Directory.GetCurrentDirectory());
        }

        private void ExecuteSaveAll(object obj)
        {
            if (!this.CanExecuteSaveAll(obj))
            {
                return;
            }

            foreach (var template in this.Templates)
            {
                if (!template.IsActive)
                {
                    continue;
                }

                try
                {
                    template.SaveFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            this.runCommand.RaiseCanExecuteChanged();
        }

        private void GenerateResultingTemplateSet()
        {
            // Now if we have any filesets, generate new templates.
            if (this.Filesets.Count > 0)
            {
                var filesetTemplates = new List<FileTemplate>();
                foreach (var fileset in this.Filesets)
                {
                    var matchingTemplates = new List<FileTemplate>();
                    var templateName = fileset.TemplateName;
                    if (templateName != null)
                    {
                        matchingTemplates = this.Templates.Where(o => o.Name == templateName).ToList();
                        if (matchingTemplates.Count == 0)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        matchingTemplates.AddRange(this.Templates);
                    }

                    if (this.GroupBy == GroupingOrder.Template)
                    {
                        foreach (var matchingTemplate in matchingTemplates)
                        {
                            foreach (var file in fileset.Files)
                            {
                                var newTemplate = matchingTemplate.ExpandAsNew(file.Variables);
                                if (!string.IsNullOrWhiteSpace(file.Name))
                                {
                                    newTemplate.Name = file.Name;
                                }

                                newTemplate.IsActiveExpr = "(" + fileset.IsActiveExpr + ") and ("
                                    + file.IsActiveExpr + ") and (" + newTemplate.IsActiveExpr + ")";

                                filesetTemplates.Add(newTemplate);
                            }
                        }
                    }
                    else
                    {
                        foreach (var file in fileset.Files)
                        {
                            foreach (var matchingTemplate in matchingTemplates)
                            {
                                var newTemplate = matchingTemplate.ExpandAsNew(file.Variables);
                                if (!string.IsNullOrWhiteSpace(file.Name))
                                {
                                    newTemplate.Name = file.Name;
                                }

                                newTemplate.IsActiveExpr = "(" + fileset.IsActiveExpr + ") and ("
                                    + file.IsActiveExpr + ") and (" + newTemplate.IsActiveExpr + ")";

                                filesetTemplates.Add(newTemplate);
                            }
                        }
                    }
                }

                this.Templates.Clear();
                filesetTemplates.ForEach(t => this.Templates.Add(t));
            }
        }

        private void OnQuickpickChanged()
        {
            var qp = this.SelectedQuickpick;
            if (qp == null)
            {
                return;
            }

            foreach (var preset in qp.Presets)
            {
                var selector = this.Selectors.Find(o => o.Name == preset.Key);
                if (selector != null)
                {
                    selector.SelectedChoice = selector.Choices.Find(o => o.Name == preset.Value);
                }
            }
        }

        private void ResolveSelectorReferencesRecursively(LayoutContainerNode layoutNode)
        {
            if (layoutNode == null)
            {
                throw new ArgumentNullException("layoutNode", "error: Unknown layout container type.");
            }

            foreach (var child in layoutNode.Children)
            {
                var itemNode = child as LayoutItemNode;
                if (itemNode != null)
                {
                    var selectorNode = itemNode as SelectorLayoutNode;
                    if (selectorNode != null)
                    {
                        var selector = this.Selectors.Find(o => o.Name == selectorNode.SelectorName);
                        if (selector == null)
                        {
                            throw new Exception(
                                string.Format(
                                    "error: Undefined selector {0} is referenced in the layout",
                                    selectorNode.SelectorName));
                        }

                        if (selector.IsInLayout)
                        {
                            throw new Exception(
                                string.Format(
                                    "error: Selector {0} already used in other part of layout",
                                    selectorNode.SelectorName));
                        }

                        selectorNode.Selector = this.Selectors.Find(o => o.Name == selectorNode.SelectorName);
                        selectorNode.Selector.IsInLayout = true;
                    }
                }
                else
                {
                    this.ResolveSelectorReferencesRecursively(child as LayoutContainerNode);
                }
            }
        }

        #endregion
    }
}