// --------------------------------------------------------------------------------
// <copyright file="SelectorTemplateSelector.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the SelectorTemplateSelector type.
// </summary>
// --------------------------------------------------------------------------------
namespace SpitOut
{
    using System.Windows;
    using System.Windows.Controls;

    using SpitOut.Models;

    public class SelectorTemplateSelector : DataTemplateSelector
    {
        #region Public Methods and Operators

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            if (element == null)
            {
                return null;
            }

            var selectorModel = item as SelectorModel;
            if (selectorModel == null)
            {
                return null;
            }

            return element.TryFindResource(selectorModel.SelectorType.ToString() + "SelectorTemplate") as
                   DataTemplate;
        }

        #endregion
    }
}
