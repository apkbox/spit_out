// --------------------------------------------------------------------------------
// <copyright file="LayoutNode.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the LayoutNode type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models.Layout
{
    using System.Windows.Media;

    public abstract class LayoutNode
    {
        #region Public Properties

        public Brush BorderColor { get; set; }

        public Brush Color { get; set; }

        #endregion
    }
}