// --------------------------------------------------------------------------------
// <copyright file="SelectorModel.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the SelectorModel type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Media;

    using SpitOut.Annotations;

    public class SelectorModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly ConfigModel config;

        private string cachedTextValueKey;

        private bool isActive = true;

        private bool isVisible = true;

        private string label;

        private ChoiceModel selectedChoice;

        #endregion

        #region Constructors and Destructors

        public SelectorModel(ConfigModel config)
        {
            this.config = config;
            this.Choices = new List<ChoiceModel>();
            this.SelectorType = SelectorType.ListBox;
            this.IsActiveExpr = "true";
            this.ControlWidth = double.NaN;
            this.ControlHeight = double.NaN;
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public Brush BorderColor { get; set; }

        public List<ChoiceModel> Choices { get; private set; }

        public Brush Color { get; set; }

        public double ControlHeight { get; set; }

        public double ControlWidth { get; set; }

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
                this.IsVisible = value;
                this.OnPropertyChanged();
            }
        }

        public string IsActiveExpr { get; set; }

        public bool? IsChecked
        {
            get
            {
                if (this.SelectedChoice == null)
                {
                    return null;
                }

                return bool.Parse(this.SelectedChoice.Name);
            }

            set
            {
                this.SelectedChoice = this.Choices.Find(o => bool.Parse(o.Name) == value);
            }
        }

        public bool IsHidden { get; set; }

        public bool IsInLayout { get; set; }

        public bool IsVisible
        {
            get
            {
                return this.isVisible && !this.IsHidden;
            }

            internal set
            {
                if (value.Equals(this.isVisible))
                {
                    return;
                }

                this.isVisible = value;
                this.OnPropertyChanged();
            }
        }

        public string Label
        {
            get
            {
                return this.label ?? this.Name;
            }

            set
            {
                this.label = value;
            }
        }

        public string Name { get; set; }

        public ChoiceModel SelectedChoice
        {
            get
            {
                return this.selectedChoice;
            }

            set
            {
                if (value == this.selectedChoice)
                {
                    return;
                }

                this.selectedChoice = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("IsChecked");
                this.config.NotifySelectionChanged();
            }
        }

        public SelectorType SelectorType { get; set; }

        public string TextValue
        {
            get
            {
                var varModel = this.GetSingleVariable();
                return varModel == null ? null : varModel.Value;
            }

            set
            {
                var varModel = this.GetSingleVariable();
                if (varModel != null)
                {
                    varModel.Value = value;
                }

                this.OnPropertyChanged();
                this.config.NotifySelectionChanged();
            }
        }

        #endregion

        #region Methods

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private VarModel GetSingleVariable()
        {
            if (this.cachedTextValueKey == null)
            {
                this.cachedTextValueKey = this.selectedChoice.Variables.Keys.FirstOrDefault();
            }

            return this.cachedTextValueKey == null ? null : this.selectedChoice.Variables[this.cachedTextValueKey];
        }

        #endregion
    }
}