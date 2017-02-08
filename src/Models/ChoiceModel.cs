// --------------------------------------------------------------------------------
// <copyright file="ChoiceModel.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the ChoiceModel type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models
{
    using System.Collections.Generic;

    public class ChoiceModel
    {
        #region Fields

        private string label;

        #endregion

        #region Constructors and Destructors

        public ChoiceModel()
        {
            this.Variables = new Dictionary<string, VarModel>();
        }

        #endregion

        #region Public Properties

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

        public Dictionary<string, VarModel> Variables { get; private set; }

        #endregion
    }
}