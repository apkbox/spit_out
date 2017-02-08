// --------------------------------------------------------------------------------
// <copyright file="VStackGroupLayoutNode.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the VStackGroupLayoutNode type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models.Layout
{
    public class VStackGroupLayoutNode : HStackLayoutNode, ILayoutGroup
    {
        #region Public Properties

        public bool IsExpanded { get; set; }

        public string Label { get; set; }

        #endregion
    }
}