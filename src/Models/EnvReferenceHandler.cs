// --------------------------------------------------------------------------------
// <copyright file="EnvReferenceHandler.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the EnvReferenceHandler type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models
{
    using System;

    internal class EnvReferenceHandler : ISpecialReferenceHandler
    {
        #region Constructors and Destructors

        public EnvReferenceHandler()
        {
        }

        #endregion

        #region Public Methods and Operators

        public string Expand(string name)
        {
            return Environment.GetEnvironmentVariable(name) ?? string.Empty;
        }

        #endregion
    }
}