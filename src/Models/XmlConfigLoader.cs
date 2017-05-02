// --------------------------------------------------------------------------------
// <copyright file="XmlConfigLoader.cs" company="SpitOut Project">
//   Copyright (c) 2016 Alex Kozlov
// </copyright>
// <summary>
//   Defines the XmlConfigLoader type.
// </summary>
// --------------------------------------------------------------------------------

namespace SpitOut.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    using SpitOut.Models.Layout;

    public class XmlConfigLoader
    {
        #region Fields

        private readonly ConfigModel config = new ConfigModel();

        private readonly XmlDocument doc = new XmlDocument();

        private readonly XmlNameTable nt = new NameTable();

        private XmlNamespaceManager ns;

        #endregion

        #region Public Methods and Operators

        public static ConfigModel LoadFromFile(string fileName)
        {
            var loader = new XmlConfigLoader();
            loader.LoadDocumentInternal(fileName);
            loader.LoadInternal();
            loader.config.ConfigName = fileName;
            loader.config.FinalizeLoading();
            return loader.config;
        }

        #endregion

        #region Methods

        private static void ForEachElementDo(XmlNode node, string xpath, Action<XmlElement> action)
        {
            var nodes = node.SelectNodes(xpath);
            if (nodes != null)
            {
                foreach (XmlElement element in nodes)
                {
                    action(element);
                }
            }
        }

        private static T ParseAsEnum<T>(string value, T defaultValue = default(T)) where T : struct
        {
            if (value != null)
            {
                return (T)Enum.Parse(typeof(T), value.Replace("-", string.Empty), true);
            }

            return defaultValue;
        }

        private static ChoiceModel ParseChoiceElement(XmlElement choiceElement)
        {
            var choiceModel = new ChoiceModel();
            WithAttributeDo(choiceElement, "label", label => choiceModel.Label = label);
            WithAttributeDo(choiceElement, "name", name => choiceModel.Name = name);
            ForEachElementDo(choiceElement, "var", v => ParseVarElement(v, choiceModel.Variables));
            return choiceModel;
        }

        private static Brush ParseColor(string s)
        {
            var value = new BrushConverter().ConvertFromInvariantString(s);
            if (value == null)
            {
                throw new Exception(string.Format("error: Invalid color '{0}", s));
            }

            return (Brush)value;
        }

        private static FilesetFile ParseFileElement(XmlElement f)
        {
            var filesetFile = new FilesetFile();
            WithAttributeDo(f, "name", n => filesetFile.Name = n);
            WithAttributeDo(f, "active", n => filesetFile.IsActiveExpr = n);
            ForEachElementDo(f, "var", varNode => ParseFilesetFileVarElement(varNode, filesetFile.Variables));
            return filesetFile;
        }

        private static Fileset ParseFilesetElement(XmlElement e)
        {
            var fileset = new Fileset();
            WithAttributeDo(e, "template", n => fileset.TemplateName = n);
            WithAttributeDo(e, "active", n => fileset.IsActiveExpr = n);
            ForEachElementDo(e, "file", f => fileset.Files.Add(ParseFileElement(f)));
            return fileset;
        }

        private static void ParseFilesetFileVarElement(XmlElement varElement, Dictionary<string, string> variables)
        {
            var name = varElement.GetAttribute("name");
            var value = varElement.InnerText;
            variables.Add(name, value);
        }

        private static QuickpickModel ParseQuickpick(XmlElement quickpickElement)
        {
            var quickpickModel = new QuickpickModel();
            WithAttributeDo(quickpickElement, "label", n => quickpickModel.Label = n);
            ForEachElementDo(quickpickElement, "set", setElement => ParseSetElement(setElement, quickpickModel.Presets));
            return quickpickModel;
        }

        private static void ParseSetElement(XmlElement setElement, Dictionary<string, string> presets)
        {
            presets.Add(setElement.GetAttribute("selector"), setElement.GetAttribute("choice"));
        }

        private static FileTemplate ParseTemplateElement(XmlElement e)
        {
            var fileTemplate = new FileTemplate();
            WithAttributeDo(e, "name", n => fileTemplate.Name = n);
            WithAttributeDo(e, "filename", n => fileTemplate.FileNameTemplate = n);
            WithAttributeDo(e, "active", n => fileTemplate.IsActiveExpr = n);
            WithAttributeDo(e, "createdir", n => fileTemplate.CreateDir = bool.Parse(n));
            WithAttributeDo(e, "runnable", n => fileTemplate.IsRunnable = bool.Parse(n));
            WithAttributeDo(e, "runexecutable", n => fileTemplate.RunExecutable = n);
            WithAttributeDo(e, "runcommandline", n => fileTemplate.RunCommandLine = n);
            WithAttributeDo(e, "runbuttontext", n => fileTemplate.RunCommandCaption = n);
            fileTemplate.ContentTemplate = e.InnerText;
            return fileTemplate;
        }

        private static void ParseVarElement(XmlElement varElement, Dictionary<string, VarModel> variables)
        {
            var isExpression = false;
            WithAttributeDo(varElement, "evaluate", s => isExpression = bool.Parse(s));

            var model = new VarModel
                            {
                                Name = varElement.GetAttribute("name"),
                                Value = varElement.InnerText,
                                IsExpression = isExpression
                            };
            variables.Add(model.Name, model);
        }

        private static double StringToLength(string s)
        {
            var value = new LengthConverter().ConvertFromInvariantString(s);
            if (value == null)
            {
                throw new Exception(string.Format("error: Invalid lenth '{0}", s));
            }

            return (double)value;
        }

        private static void WithAttributeDo(
            XmlElement element,
            string name,
            Action<string> action,
            Action falseAaction = null)
        {
            if (element.HasAttribute(name))
            {
                var value = element.GetAttribute(name);
                action(value);
            }
            else if (falseAaction != null)
            {
                falseAaction();
            }
        }

        private void LoadDocumentInternal(string fileName)
        {
            using (var reader = new XmlTextReader(fileName, this.nt))
            {
                this.ns = new XmlNamespaceManager(this.nt);
                reader.Namespaces = false;
                this.doc.Load(reader);
            }
        }

        private void LoadInternal()
        {
            var configElement = this.doc.SelectSingleNode("config") as XmlElement;
            if (configElement == null)
            {
                return;
            }

            WithAttributeDo(configElement, "width", n => this.config.WindowWidth = StringToLength(n));
            WithAttributeDo(configElement, "height", n => this.config.WindowHeight = StringToLength(n));
            WithAttributeDo(configElement, "title", n => this.config.Title = n);

            var quickpicksElement = this.doc.SelectSingleNode("/config/quickpicks") as XmlElement;
            if (quickpicksElement != null)
            {
                WithAttributeDo(quickpicksElement, "label", o => this.config.QuickpicksLabel = o);
                WithAttributeDo(quickpicksElement, "width", o => this.config.QuickpicksWidth = StringToLength(o));
                WithAttributeDo(
                    quickpicksElement,
                    "selectorspanel",
                    o => this.config.SelectorsUi = ParseAsEnum(o, SelectorsVisibility.Visible));
                ForEachElementDo(quickpicksElement, "quickpick", e => this.config.Quickpicks.Add(ParseQuickpick(e)));
            }

            this.config.Layout = this.ParseLayoutElement(
                this.doc.SelectSingleNode("/config/layout") as XmlElement,
                false);

            ForEachElementDo(this.doc, "/config/selector", e => this.config.Selectors.Add(this.ParseSelector(e)));

            ForEachElementDo(
                this.doc,
                "/config/templates/template",
                e => this.config.Templates.Add(ParseTemplateElement(e)));

            var groupByAttrib = this.doc.SelectSingleNode("/config/filesets/@groupby") as XmlAttribute;
            this.config.GroupBy = GroupingOrder.Template;
            if (groupByAttrib != null)
            {
                this.config.GroupBy = ParseAsEnum<GroupingOrder>(groupByAttrib.Value);
            }

            ForEachElementDo(
                this.doc,
                "/config/filesets/fileset",
                e => this.config.Filesets.Add(ParseFilesetElement(e)));
        }

        private LayoutNode ParseLayoutElement(XmlElement layoutElement, bool allowSelectors)
        {
            if (layoutElement == null)
            {
                return null;
            }

            if (layoutElement.LocalName == "selector" && allowSelectors)
            {
                var node = new SelectorLayoutNode();
                WithAttributeDo(layoutElement, "name", n => node.SelectorName = n);
                WithAttributeDo(layoutElement, "color", n => node.Color = ParseColor(n));
                WithAttributeDo(layoutElement, "bordercolor", n => node.BorderColor = ParseColor(n));
                return node;
            }

            var layoutType = LayoutType.WrapPanel;
            WithAttributeDo(layoutElement, "type", v => layoutType = ParseAsEnum<LayoutType>(v));

            var groupType = LayoutGroupType.None;
            WithAttributeDo(layoutElement, "group", v => groupType = ParseAsEnum<LayoutGroupType>(v));

            LayoutContainerNode layoutNode;
            switch (layoutType)
            {
                case LayoutType.WrapPanel:
                    switch (groupType)
                    {
                        case LayoutGroupType.None:
                            layoutNode = new WrapPanelLayoutNode();
                            break;
                        case LayoutGroupType.Simple:
                            layoutNode = new WrapPanelGroupLayoutNode();
                            break;
                        case LayoutGroupType.Expander:
                            layoutNode = new WrapPanelExpanderLayoutNode();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case LayoutType.UniformGrid:
                    UniformGridLayoutNode ug;
                    switch (groupType)
                    {
                        case LayoutGroupType.None:
                            ug = new UniformGridLayoutNode();
                            break;
                        case LayoutGroupType.Simple:
                            ug = new UniformGridGroupLayoutNode();
                            break;
                        case LayoutGroupType.Expander:
                            ug = new UniformGridExpanderLayoutNode();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    WithAttributeDo(layoutElement, "rows", v => ug.Rows = int.Parse(v));
                    WithAttributeDo(layoutElement, "columns", v => ug.Columns = int.Parse(v));
                    layoutNode = ug;
                    break;
                case LayoutType.VStack:
                    switch (groupType)
                    {
                        case LayoutGroupType.None:
                            layoutNode = new VStackLayoutNode();
                            break;
                        case LayoutGroupType.Simple:
                            layoutNode = new VStackGroupLayoutNode();
                            break;
                        case LayoutGroupType.Expander:
                            layoutNode = new VStackExpanderLayoutNode();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case LayoutType.HStack:
                    switch (groupType)
                    {
                        case LayoutGroupType.None:
                            layoutNode = new HStackLayoutNode();
                            break;
                        case LayoutGroupType.Simple:
                            layoutNode = new HStackGroupLayoutNode();
                            break;
                        case LayoutGroupType.Expander:
                            layoutNode = new HStackExpanderLayoutNode();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var groupLayout = layoutNode as ILayoutGroup;
            if (groupLayout != null)
            {
                WithAttributeDo(layoutElement, "label", v => groupLayout.Label = v);
                var expanderState = ExpanderState.Expanded;
                WithAttributeDo(layoutElement, "expander", v => expanderState = ParseAsEnum<ExpanderState>(v));
                if (expanderState == ExpanderState.Collapsed)
                {
                    groupLayout.IsExpanded = false;
                }
                else if (expanderState == ExpanderState.Expanded)
                {
                    groupLayout.IsExpanded = true;
                }
            }

            WithAttributeDo(layoutElement, "color", n => layoutNode.Color = ParseColor(n));
            WithAttributeDo(layoutElement, "bordercolor", n => layoutNode.BorderColor = ParseColor(n));

            ForEachElementDo(layoutElement, "*", node => layoutNode.Children.Add(this.ParseLayoutElement(node, true)));

            return layoutNode;
        }

        private SelectorModel ParseSelector(XmlElement selectorElement)
        {
            var selectorModel = new SelectorModel(this.config);
            WithAttributeDo(selectorElement, "label", n => selectorModel.Label = n);
            WithAttributeDo(selectorElement, "name", n => selectorModel.Name = n);
            WithAttributeDo(selectorElement, "type", n => selectorModel.SelectorType = ParseAsEnum<SelectorType>(n));
            WithAttributeDo(selectorElement, "width", n => selectorModel.ControlWidth = StringToLength(n));
            WithAttributeDo(selectorElement, "height", n => selectorModel.ControlHeight = StringToLength(n));
            WithAttributeDo(selectorElement, "active", n => selectorModel.IsActiveExpr = n);
            WithAttributeDo(selectorElement, "hidden", n => selectorModel.IsHidden = bool.Parse(n));
            WithAttributeDo(selectorElement, "color", n => selectorModel.Color = ParseColor(n));
            WithAttributeDo(selectorElement, "bordercolor", n => selectorModel.BorderColor = ParseColor(n));

            // textbox is different from other controls as it can affect only one variable
            // and has no choices.
            if (selectorModel.SelectorType == SelectorType.TextBox
                || selectorModel.SelectorType == SelectorType.Directory
                || selectorModel.SelectorType == SelectorType.File)
            {
                var vars = new Dictionary<string, VarModel>();
                ParseVarElement(selectorElement.SelectSingleNode("var") as XmlElement, vars);
                if (vars.Count > 0)
                {
                    var onlyVar = vars.First().Value;
                    var onlyChoice = new ChoiceModel();
                    onlyChoice.Variables.Add(onlyVar.Name, onlyVar);
                    selectorModel.SelectedChoice = onlyChoice;
                }

                return selectorModel;
            }

            ForEachElementDo(
                selectorElement,
                "choice",
                choiceElement => selectorModel.Choices.Add(ParseChoiceElement(choiceElement)));

            // Check whether checkbox selector choices are booleans,
            // otherwise switch to listbox.
            if (selectorModel.SelectorType == SelectorType.CheckBox)
            {
                if (selectorModel.Choices.Count > 2)
                {
                    selectorModel.SelectorType = SelectorType.ListBox;
                }
                else
                {
                    bool hasTrue = false;
                    bool hasFalse = false;

                    // Should be exactly true and false, or one or these.
                    foreach (var v in selectorModel.Choices)
                    {
                        bool result;
                        if (bool.TryParse(v.Name, out result))
                        {
                            if (result)
                            {
                                if (hasTrue)
                                {
                                    throw new Exception(
                                        string.Format(
                                            "More than one true choice defined for selector {0}",
                                            selectorModel.Name));
                                }

                                hasTrue = true;
                            }
                            else
                            {
                                if (hasFalse)
                                {
                                    throw new Exception(
                                        string.Format(
                                            "More than one true choice defined for selector {0}",
                                            selectorModel.Name));
                                }

                                hasFalse = true;
                            }
                        }
                    }

                    if (!hasTrue)
                    {
                        selectorModel.Choices.Add(new ChoiceModel { Name = "true" });
                    }

                    if (!hasFalse)
                    {
                        selectorModel.Choices.Add(new ChoiceModel { Name = "false" });
                    }
                }
            }

            // Define undefined variables as empty strings
            var allKeys = new List<string>();

            // Collect all variable keys
            foreach (var val in selectorModel.Choices)
            {
                allKeys = allKeys.Union(val.Variables.Keys).ToList();
            }

            // Add empty string variable to each choice where it is missing.
            foreach (var choiceModel in selectorModel.Choices)
            {
                foreach (var key in allKeys)
                {
                    if (!choiceModel.Variables.ContainsKey(key))
                    {
                        choiceModel.Variables[key] = new VarModel() { Name = key, Value = string.Empty };
                    }
                }
            }

            // Set default
            WithAttributeDo(
                selectorElement,
                "default",
                n =>
                    {
                        if (!string.IsNullOrWhiteSpace(n))
                        {
                            selectorModel.SelectedChoice = selectorModel.Choices.FirstOrDefault(o => o.Name == n);
                        }
                    });
            return selectorModel;
        }

        #endregion
    }
}