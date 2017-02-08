// --------------------------------------------------------------------------------
// <copyright file="LayoutContainerNode.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the LayoutContainerNode type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models.Layout
{
    using System.Collections.Generic;

    public abstract class LayoutContainerNode : LayoutNode
    {
        #region Constructors and Destructors

        protected LayoutContainerNode()
        {
            this.Children = new List<LayoutNode>();
        }

        #endregion

        #region Public Properties

        public List<LayoutNode> Children { get; private set; }

        #endregion
    }
}