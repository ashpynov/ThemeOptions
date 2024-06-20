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

### Priority of files to be loaded
It is possible that 'preset files' contain resources with same name - and final value is depend on order of files was loaded: last one wins. The order of preset loaded is same as in options files from top to bottom.



