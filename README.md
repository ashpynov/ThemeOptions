## Structure of theme options manifest file
Theme options looks up options.yaml file with descriptions of presets of theme option.
Each theme may have multiple groups of presets, with set of Presets.
Each Preset has:
- Name to show
- Preview image
- List of additional xaml file need to be loaded to activate preset

Format is next:

```yaml

Presets:
    <preset_group_id_1>:
        Name: <Group 1 User friendly name>
        Presets:
            Default:
                Name: <Default preset name>
                Preview: <relative path to preview image(optional)>
            <preset_1_id>:
                Name: <Preset 1.1 name>
                Preview: <relative path to preview image(optional)>
                Files:
                    - <relative path to additional xaml file>
                    - <relative path to additional xaml file>
                Constants:
                    <constant_ke_1_>:
                        Type: <type String|Boolean|Double|Int32|Color|Thickness>
                        Value: <value>
                    <constant_key_2>:
                        Type: <type String|Boolean|Double|Int32|Color|Thickness>
                        Value: <value>
    <preset_group_id_2>:
        Name: <Group 2 User friendly name>
        Presets:
            Default:
                Name: <Default preset name>
                Preview: <relative path to preview image(optional)>
            <preset_1_id>:
                Name: <Preset 2.1 name>
                Preview: <relative path to preview image(optional)>
                Files:
                    - <relative path to additional xaml file>
                    - <relative path to additional xaml file>
Variables:
    <variable_key>:
        Title: <user friendly name>
        Type: <type String|Boolean>
        Default: <default value>
    <variable_key>:
        Title: <user friendly name>
        Type: <type String|Boolean>
        Default: <default value>


```

In UI it will be shown as:

- Group 1
  - default
  - preset 1.1
  - preset 1.2
- Group 2
  - default
  - preset 2.1
  - preset 2.2

User may select one variant of preset for each group.

Presets option with id equal to 'default' will be shown as selected options in settings UI if nothing specified. Obviously it does not need any Files - as soon as 'Default' theme composition showd be done using Playnite build-in theme files only. If no default section is found for preset in options - it will be added automaticly with empty preview and files.

Also preset may contain one or many constants redifinition - this may be alternative if it is no reasonable to have dedicated xaml file. Constants definition is next:
```yaml
TopButtonAlways:
    Type: Boolean
    Value: False
```
this will generate xaml definition:

```xml
<sys:Boolean x:Key="TopButtonAlways">False</sys:Boolean>
```

Currently types are supported:
- Boolean
- String
- Double
- Int32
- Color
- Thickness

### Custom user settings

Also It is possible to define user customizable 'Theme settings' or Variables
Specification contains field:
- Title - for user friendly name
- Type - type of variable
- Default - default value to be shown in settings

User will be able to specify valuse on user settings page.
Suported list of variables Types:
- String (via TextBox)
- Boolean (via CheckBox)


### Priority of files to be loaded
It is possible that 'preset files' contain resources with same name - and final value is depend on order of files was loaded: last one wins. The order of preset loaded is same as in options files from top to bottom.

Constants of presets are loaded after theme preset xamls

Variables are loaded latest.

So order is next:

1. Default Theme xaml resources
2. User selected theme xaml resources
3. Selected theme presets xaml resources
4. Theme Localization resources
5. Selected theme presets constants
6. User theme settings

