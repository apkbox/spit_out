// --------------------------------------------------------------------------------
// <copyright file="UniformGridLayoutNode.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the UniformGridLayoutNode type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models.Layout
{
    public class UniformGridLayoutNode : LayoutContainerNode
    {
        #region Public Properties

        public int Columns { get; internal set; }

        public int Rows { get; internal set; }

        #endregion
    }
}