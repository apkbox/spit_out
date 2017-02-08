// --------------------------------------------------------------------------------
// <copyright file="ILayoutGroup.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the LayoutGroupContainerNode type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models.Layout
{
    public interface ILayoutGroup
    {
        #region Public Properties

        bool IsExpanded { get; set; }

        string Label { get; set; }

        #endregion
    }
}