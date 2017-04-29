// --------------------------------------------------------------------------------
// <copyright file="DateReferenceHandler.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the DateReferenceHandler type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models
{
    using System;
    using System.Globalization;

    internal class DateReferenceHandler : ISpecialReferenceHandler
    {
        #region Fields

        private readonly bool isUtc;

        #endregion

        #region Constructors and Destructors

        public DateReferenceHandler(bool isUtc)
        {
            this.isUtc = isUtc;
        }

        #endregion

        #region Public Methods and Operators

        public string Expand(string format)
        {
            var time = this.isUtc ? DateTime.UtcNow : DateTime.Now;
            return string.IsNullOrEmpty(format) ? time.ToString(CultureInfo.CurrentCulture) : time.ToString(format);
        }

        #endregion
    }
}