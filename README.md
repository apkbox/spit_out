# spitout
Allows quickly generate file from template and set of parameters choosen in the UI.

# Important note
This app could be star destroyer, but I like it to be a small swiss army knife.

# Selectors
A selector represents a user interface element that can be used to set variables.

The following selectors are available
  * `listbox` List box
  * `combobox` Combo box
  * `checkbox` Check box
  * `textbox` Text box
  * `directory` Directory selector
  * `file` File selector
  
Each selector is identified by name attribute which is used to reference it in quickpick
or layout sections. Name is alse acts as a UI label, unless label attribute is explicitly
specified.

Some selector types have restrictions on syntax, like a text box that can set only one
variable.

The following attributes are common for all selectors:
  * `label` Specifies a display label of the control. If not specified, `name` is used.
  * `name` Specifies internal name of the selector if it needs to be referenced in
    quickpick or layout.
  * `type` Specifies selector type. Default is `listbox`.
  * `width` Width of the selector in pixels.
  * `height` Height of the selector in pixels.
  * `active` A boolean value specifying whether selector is enabled. This can be a variable reference.
  * `hidden` A boolean value specifying whether selector is hidden. This can be a variable reference.
     This is a common way to define constants.
  * `color` Specifies a color name or RGB value of the control foreground.
  * `bordercolor` Specifies a color name or RGB value of control's border.
  
## ListBox
The selector defines a single selection list box. If selector type is not explicitly
specified this is a default selector type.

The `default` attribute identifies default selected `choice` element by name.

One or more `choice` elements specify items of the list box. Each choice element can specify
`name` attribute that is used in selector\`s `default` attribute and `label` can be used to
specify UI text of the choice. If `label` is not specified then `name` is used instead.

Each choice can define zero or more variables.

```xml
    <selector name="Platform" default="x64" type="listbox">
        <choice name="Win32">
            <var name="Platform">Win32</var>
        </choice>
        <choice name="x64">
            <var name="Platform">x64</var>
        </choice>
    </selector>
```

## ComboBox
The selector defines a combo box. And functionally equivalent to list box.

## CheckBox
The selector is similar to list box, but can contain no more than two choices.
If more than two choices specified it defaults to list box.

Two choices must be named exactly `true` and `false`.
```xml
    <selector name="Log into file" default="true" type="checkbox">
        <choice name="true">
            <var name="LOG_SWITCH">/log:logfile.log</var>
        </choice>
        <choice name="false">
            <var name="LOG_SWITCH">/log-to-console</var>
        </choice>
    </selector>
```

## TextBox, Directory, File
Three selector types that present a text box. In case of Directory and
File an additional button is shown beside the text box to select directory
or file.

This selector does not have choices but rather defines a single variable.
The `default` attribute is meaningless and ignored.

```xml
    <selector name="Log location" type="directory">
        <var name="LOG_DIR">C:\Logs</var>
    </selector>
```


# Variables
Variables defined through `var` elements and referenced in template with
`${name}`, where `name` is the variable name.

Variable references can be used in the following places:
* Selector's `active` attribute
* As another variable's value (see `Double evaluation`)
* Template's `filename` attribute
* Template content

# Special variables
In addition to regular variables, also predefined variables available.
In the following list the square brackets `[]` denote optional parameter.

    ${guid[:format]}
Each occurence expands to new GUID. Optional format specifies the GUID string
format.

The following formats available:

    N       00000000000000000000000000000000
    D       00000000-0000-0000-0000-000000000000
    B       {00000000-0000-0000-0000-000000000000}
    P       (00000000-0000-0000-0000-000000000000)
    X       {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}

    ${env:name}
Expands to an OS environment variable value or empty string if environment
variable is not defined.

    ${now[:format]}
Each occurence expands to current date and time string. The optional format
parameter can be used to specify custom formatting.

    ${utcnow[:format]}
Same as `now`, except that time is UTC.

    ${var:name}
Expands to an OS environmet variable value if environment variable is defined,
otherwise expands to a normal variable reference. This construct allows to use
normal variable with ability to override it with an environment variable.

# Quick pick
Quick picks allow quickly select specified selectors.
Each quick pick specifies selector and choice names to set.
You can hide all selectors when using quick pick or hide them behind expander.
Quickpicks occupy separate panel above selectors and are not affected by layout.

```xml
<quickpicks width="auto" label="Presets" selectorspanel="collapsed"> <!-- visible, hidden, collapsed, expanded -->
  <quickpick label="Preset 1">
    <set selector="selector1" choice="choicename1"/>
    <set selector="selector2" choice="choicename2"/>
  <quickpick>
  <quickpick label="Preset 2">
    <set selector="selector1" choice="choicename3"/>
    <set selector="selector2" choice="choicename4"/>
  <quickpick>
</quickpicks>
```

# Custom layout
The selector area is split in multiple areas. Each area corresponds to the layout.
There is also additional area, where selector not specified in the layout are placed. The layout for this area is WrapPanel.
You can override width and height of the selector in layout.

**Important:** The expander that may be shown when quick picks are used is not affected
by the custom layout. To control it use selectorspanel attribute of quickpicks.

The available layout types are
  * `wrappanel` - UI elements flow from left to right, top to bottom.
  * `uniformgrid` - Each element is placed in a grid, where each cell is equal sized
  * `vstack` - Elements are organized in a vertical stack
  * `hstack` - Elements are organized in a horizontal stack
  
Each layout element can additionally grouped into a labeled and expandable group
  * `none` - Ordinary group. `label` attribute is ignored
  * `simple` - A simple group that can have label specified with `label` attribute.
  * `expander` - An expander group that user can expand and collapse.
    The initial state of the expander can be specified with `expander` attribute that can be either
    `expanded` or `collapsed`. The default state is `expanded`.

```xml

<layouts>
  <layout type="uniformgrid">
    <selector row="0" column="0" name="name"/>
    <layout row="0" column="1" type="vstack">
      <selector name="a" height="100"/>
      <selector name="b"/>
    </layout>
  </layout>
  <layout type="expander" label="Optional parameters">
    <!-- expander implicitly creates a wrappanel layout if there is more than
        one child -->
    <selector name="Option1"/>
  </layout>
</layouts>

```


# Fileset
Fileset will create as many templates, based on one, as file entries. These templates are resolved with variables specified in file tag.

```xml
<filesets>
  <!-- Template name is optional. If not specified, fileset is generated for each valid template. -->
  <!-- Valid template is the one that is selected, when template selection is implemented.  -->
  <fileset template="template_name">
    <file name="File 1">
      <var name="file_no">1</var>
      <var name="color">Red</var>
    </file>
    <file>
      <var name="file_no">2</var>
      <var name="color">Green</var>
    </file>
    <file>
      <var name="file_no">3</var>
      <var name="color">Blue</var>
    </file>
</fileset>

<!-- ... then -->
<template filename="color_file_${file_no}.txt"><![CDATA[
My color in the file ${file_no} is ${color}
]]>
</template>

```

# Double evaluation
Double evaluation allows to specify variable value as a template.

Say **${other}** is defined as **C:\Program Files**.
```xml
<selector>
  <choice>
    <var name="exp" evaluate="true">${other}</var>
  </choice>
</selector>

<template>
  ${exp}
</template>
```
Then template expands to **C:\Program Files** instead of **${other}** when using
double evaluation.

Variables evaluated in the same order they appear in config, from the first
selector to the last and in the order the defined within selector's choice.
Variables in not active selectors and in non-selected choices not expanded.

This will not result in recursive expansion, because ```exp``` is not defined yet,
when it is used as content. But even if it was defined above, the evaluation
happens only once, when variable is encountered.
```xml
 <var name="exp" evaluate="true">a_${exp}</var>
```

This will not work for the same reason as in example above.
```xml
 <var name="exp" evaluate="true">${other}</var>
 <var name="other" evaluate="true">${exp}</var>
```

Note that variables used in fileset file definitions cannot be evaluated as
these expanded first and one time only at config loading time when producing
final set of templates.
