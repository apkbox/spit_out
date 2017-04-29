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
        #region Constants

        private const string SpecialRefPattern = @"\$\{([\w_-[\d]][\w_]*)(?:\:([^}]+))?\}";

        private const string VariableRefPattern = @"\$\{([\w_-[\d]][\w_]*)\}";

        #endregion

        #region Static Fields

        private static readonly Dictionary<string, ISpecialReferenceHandler> SpecialRefHandlers =
            new Dictionary<string, ISpecialReferenceHandler>
                {
                    { "guid", new GuidReferenceHandler() },
                    { "env", new EnvReferenceHandler() },
                    { "now", new DateReferenceHandler(false) },
                    { "utcnow", new DateReferenceHandler(true) },
                    { "var", new VarReferenceHandler() }
                };

        #endregion

        #region Methods

        internal static string ExpandTemplate(
            Dictionary<string, string> variables,
            string template,
            HashSet<string> unresolvedTags)
        {
            var rx = new Regex(SpecialRefPattern);
            var matches = rx.Matches(template);
            var offset = 0;
            foreach (Match match in matches)
            {
                var spec = match.Groups[1].Value;
                var arguments = match.Groups[2].Value;

                ISpecialReferenceHandler handler;
                if (SpecialRefHandlers.TryGetValue(spec, out handler))
                {
                    var value = handler.Expand(arguments);
                    template = template.Remove(match.Index + offset, match.Length);
                    template = template.Insert(match.Index + offset, value);
                    offset += value.Length - match.Length;
                }
            }

            foreach (var variable in variables)
            {
                template = template.Replace("${" + variable.Key + "}", variable.Value);
            }

            if (unresolvedTags != null)
            {
                rx = new Regex(VariableRefPattern);
                matches = rx.Matches(template);
                foreach (Match match in matches)
                {
                    unresolvedTags.Add(match.Groups[1].Value);
                }
            }

            return template;
        }

        internal static string ReplaceUnresolved(string template, string value)
        {
            var rx = new Regex(VariableRefPattern);
            return rx.Replace(template, value);
        }

        #endregion
    }
}