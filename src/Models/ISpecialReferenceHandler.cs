// --------------------------------------------------------------------------------
// <copyright file="ISpecialReferenceHandler.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the ISpecialReferenceHandler type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models
{
    internal interface ISpecialReferenceHandler
    {
        #region Public Methods and Operators

        string Expand(string value);

        #endregion
    }
}