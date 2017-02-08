// --------------------------------------------------------------------------------
// <copyright file="VarModel.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the VarModel type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models
{
    public class VarModel
    {
        #region Public Properties

        public bool IsExpression { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        #endregion
    }
}