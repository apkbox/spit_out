// --------------------------------------------------------------------------------
// <copyright file="SelectorLayoutNode.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the SelectorLayoutNode type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models.Layout
{
    public class SelectorLayoutNode : LayoutItemNode
    {
        #region Public Properties

        public SelectorModel Selector { get; set; }

        public string SelectorName { get; set; }

        #endregion
    }
}