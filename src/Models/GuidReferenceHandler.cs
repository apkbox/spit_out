// --------------------------------------------------------------------------------
// <copyright file="GuidReferenceHandler.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the GuidReferenceHandler type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models
{
    using System;

    internal class GuidReferenceHandler : ISpecialReferenceHandler
    {
        #region Constructors and Destructors

        public GuidReferenceHandler()
        {
        }

        #endregion

        #region Public Methods and Operators

        public string Expand(string format)
        {
            return Guid.NewGuid().ToString(format);
        }

        #endregion
    }
}