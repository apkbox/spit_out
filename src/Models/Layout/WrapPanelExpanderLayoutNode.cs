// --------------------------------------------------------------------------------
// <copyright file="WrapPanelExpanderLayoutNode.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the WrapPanelExpanderLayoutNode type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models.Layout
{
    public class WrapPanelExpanderLayoutNode : WrapPanelLayoutNode, ILayoutGroup
    {
        #region Public Properties

        public bool IsExpanded { get; set; }

        public string Label { get; set; }

        #endregion
    }
}