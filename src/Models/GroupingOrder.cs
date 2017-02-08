// --------------------------------------------------------------------------------
// <copyright file="GroupingOrder.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the ExpansionOrder type.
// </summary>
// --------------------------------------------------------------------------------
namespace SpitOut.Models
{
    public enum GroupingOrder
    {
        /// <summary>
        /// Group by templates, then files.
        /// </summary>
        Template, 

        /// <summary>
        /// Group by files, then templates.
        /// </summary>
        File
    }
}
