// --------------------------------------------------------------------------------
// <copyright file="VarReferenceHandler.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the VarReferenceHandler type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models
{
    using System;

    internal class VarReferenceHandler : ISpecialReferenceHandler
    {
        #region Public Methods and Operators

        public string Expand(string name)
        {
            return Environment.GetEnvironmentVariable(name) ?? "${" + name + "}";
        }

        #endregion
    }
}