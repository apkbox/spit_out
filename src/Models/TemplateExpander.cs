// --------------------------------------------------------------------------------
// <copyright file="TemplateExpander.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the TemplateExpander type.
// </summary>
// --------------------------------------------------------------------------------
namespace SpitOut.Models
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal static class TemplateExpander
    {
        #region Methods

        internal static string ExpandTemplate(
            Dictionary<string, string> variables, 
            string template, 
            HashSet<string> unresolvedTags)
        {
            foreach (var variable in variables)
            {
                template = template.Replace("${" + variable.Key + "}", variable.Value);
            }

            if (unresolvedTags != null)
            {
                var rx = new Regex(@"\$\{([\w_-[\d]][\w_]*)\}");
                var matches = rx.Matches(template);
                foreach (Match match in matches)
                {
                    unresolvedTags.Add(match.Groups[1].Value);
                }
            }

            return template;
        }

        internal static string ReplaceUnresolved(string template, string value)
        {
            var rx = new Regex(@"\$\{([\w_-[\d]][\w_]*)\}");
            return rx.Replace(template, value);
        }

        #endregion
    }
}
