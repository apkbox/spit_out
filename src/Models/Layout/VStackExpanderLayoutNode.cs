// --------------------------------------------------------------------------------
// <copyright file="VStackExpanderLayoutNode.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the VStackExpanderLayoutNode type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models.Layout
{
    public class VStackExpanderLayoutNode : HStackLayoutNode, ILayoutGroup
    {
        #region Public Properties

        public bool IsExpanded { get; set; }

        public string Label { get; set; }

        #endregion
    }
}