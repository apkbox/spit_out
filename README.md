# spitout
Allows quickly generate file from template and set of parameters choosen in the UI.

# Important note
This app could be star destroyer, but I like it to be a small swiss army knife.

# Variables
Variables defined through `var` elements and referenced in template with
`${name}`, where `name` is the variable name.

Variable references can be used in the following places:
* Selector's `active` attribute
* Variable value (see `Double evaluation`)
* Template `filename` attribute
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

```xml

<layouts>
  <layout type="grid">
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
