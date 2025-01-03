# Theme Options Extension for Playnite
![DownloadCountTotal](https://img.shields.io/github/downloads/ashpynov/ThemeOptions/total?label=total%20downloads&style=plastic) ![DownloadCountLatest](https://img.shields.io/github/downloads/ashpynov/ThemeOptions/latest/total?style=plastic) ![LatestVersion](https://img.shields.io/github/v/tag/ashpynov/ThemeOptions?label=Latest%20version&style=plastic) ![License](https://img.shields.io/github/license/ashpynov/ThemeOptions?style=plastic)

Theme Options is an extension for Playnite game manager and launcher to give ability to Theme designers to provide a wide range options configurable by User. Additionaly it will add to Theme Localization support.

So theme may have:
- Presets - a set of theme styles to be enabled by user. This presets my be be complex and drasticaly modify them look'n'feel. User may choose one option for each preset.
- Settings or Variables - some flags or texts to be used by themes. This may be simple flag to enable/disable some theme features or on screen texts like user name.
- Localizations - Theme may provide a support of locales string. This will be automaticly loaded if playnite language differ from en_US (en_US locale had to be defined in native theme file to be full functional if theme options extension absent).

This Extension add theme configuration options menu in both - Desktop at usual extension configuration place, and in Fullscreen mode at Main menu -> Settings -> Theme Options.
Please note: as soon as implementation of custom settings/menus in Fullscreen modes is not expected by Playnite developers - it use some non-PlayniteSDK way to integrate with Playnite application. So this functionality depends from Playnite verson and may be affected by update. In this case it will remove Fullscreen Settings menu item, but don't worry - Desktop mode will allow you to configure your favourite fullscreen theme, why I will update integration code.

[Latest Release](https://github.com/ashpynov/ThemeOptions/releases/latest)

## If you feel like supporting
I do everything in my spare time for free, if you feel something aided you and you want to support me, you can always buy me a "koffie" as we say in dutch, no obligations whatsoever...

<a href='https://ko-fi.com/ashpynov' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://cdn.ko-fi.com/cdn/kofi2.png?v=3' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>

If you may not use Ko-Fi in you country, it should not stop you! On [boosty](https://boosty.to/ashpynov/donate) you may support me and other creators.


# Some technical stuff

Extension does not modify any existing theme files. Instead it load additional resources files on start and generate additional resources in memory. Thats if the extension will be disabled - all effects removed. This means that themes have to be designed the way to be fully functional in theit 'default' view if extension disabled - all used resources with default values should be declared in scope of native theme files like 'contants.xaml'. And modded resources should override existing ones by theme options extension.

## 'Live' theme options
Since version bindable options and life update are supported
So instead of reffer to xaml resource you may dirrectly bind you theme settionds using Indexable syntax and pluginSettings Bind:

```xml
<MultiDataTrigger>
    <MultiDataTrigger.Conditions>
        <Condition Binding="{PluginSettings Plugin=ThemeOptions, Path=Options[ShowHelpMenu]}" Value="Collapsed"/>
    </MultiDataTrigger.Conditions>
    <Setter Property="Visibility" Value="Collapsed" />
</MultiDataTrigger>

```

where 'ShoeHelpMenu' is sample of you variables or presets constants name.

Variables may be 'Two way' bind and actual values will be save on Playnite exit. Also you may 'create' variable even without theme options declare. For example to save some custom toggle button state.

And also generated resource during fullscreen editing reloads right after Theme Option settings are closed.

In case If some theme variables or resources are used as StaticResource thus it can not be updated in runtime - such variables (and preset options) may be marked as 'NeedRestart':

```yaml
Presets:
    Interface:
        Name: Interface Style
        LocKey: LOCInterfaceStyle
        Presets:
            Default:
                Name: Default
                LocKey: LOCDefault
                Preview: Themes Option\2.Interface\Preview\1-Default.png
            Ambiance:
                Name: Ambiance
                NeedRestart: True
                LocKey: LOCAmbiance
                Preview: Themes Option\2.Interface\Preview\3-Ambiance.png
                Files:
                    - Themes Option\2.Interface\ConstantsAmbiance.xaml
```
or
```yaml
Variables:
    AcceuilOrNot:
        Title: Login screen
        LocKey: LOCLoginScreen
        Type: Boolean
        Default: True
        NeedRestart: True
```
This variables and preset options will be marked in settings interface by asterisks, and ask request restart is value was changed.


## Localization
To support localization - no additional theme manifest required. Just add locales resources into Localization folder in your theme and it will be loaded automaticly (except of en_US - default theme version has to contain default language in native theme files. I recommend constants.xaml). Please refere to Playnite source code, or almost any (including this) extension code for localization resource samples.

## Plugin installation status

To detect installation status, extension exposed 'IsInstalled' path via PluginSettings with boolean value:

```xml
<Condition Binding="{PluginSettings Plugin=ThemeOptions, Path=IsInstalled}" Value="True" />
```

Also it may determine if minimal version is greater or equal to required:
```xml
<Condition Binding="{PluginSettings Plugin=ThemeOptions, Path=MinimalVersion[0.18]}" Value="True" />
```
also string value 'Version' is exposed.


## Structure of theme options manifest file
To add ability to configure variables and presets - additional theme manifest file is required.

Theme options looks up options.yaml file next to theme.yaml file with descriptions of presets of theme option.
Each theme may have multiple groups of presets, with set of Presets.
Each Preset has:
- Name to show
- Preview image
- List of additional xaml file need to be loaded to activate preset
- LocKey - key to localized Name of preset

Format is next:

```yaml

Presets:
    <preset_group_id_1>:
        Name: <Group 1 User friendly name>
        Presets:
            Default:
                Name: <Default preset name>
                LocKey: <localization key>
                Preview: <relative path to preview image(optional)>
            <preset_1_id>:
                Name: <Preset 1.1 name>
                LocKey: <localization key>
                Preview: <relative path to preview image(optional)>
                Files:
                    - <relative path to additional xaml file>
                    - <relative path to additional xaml file>
                Constants:
                    <constant_key_1>:
                        Type: <type String|Boolean|Double|Int32|Color|Thickness|SolidColorBrush>
                        Value: <value>
                    <constant_key_2>:
                        Type: <type String|Boolean|Double|Int32|Color|Thickness|SolidColorBrush>
                        Value: <value>
    <preset_group_id_2>:
        Name: <Group 2 User friendly name>
        LocKey: <localization key>
        Presets:
            Default:
                Name: <Default preset name>
                LocKey: <localization key>
                Preview: <relative path to preview image(optional)>
            <preset_1_id>:
                Name: <Preset 2.1 name>
                LocKey: <localization key>
                Preview: <relative path to preview image(optional)>
                Files:
                    - <relative path to additional xaml file>
                    - <relative path to additional xaml file>
Variables:
    <variable_key>:
        Title: <user friendly name>
        LocKey: <localization key>
        Type: <type String|Boolean>
        Default: <default value>
    <variable_key>:
        Title: <user friendly name>
        LocKey: <localization key>
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
constant_key:
    Type: constant_type
    Value: constant_value
```

This will generate resource record as
```xml
<ns:constant_type x:Key="constant_key">constant_value</ns:constant_type>
```
Please note that type should not include namespace part - it will be added automaticly if required.

E.g. this declaration:
```yaml
TopButtonAlways:
    Type: Boolean
    Value: False
```
will generate xaml definition:

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
- Duration
- TimeSpan
- CornerRadius
- SolidColorBrush (color)

### Custom user settings

Also It is possible to define user customizable 'Theme settings' or Variables
Specification contains field:
- Title - for user friendly name
- LocKey - Key for localized string for title (optional)
- Type - type of variable
- Default - default value to be shown in settings

User will be able to specify valuse on user settings page.
Supported list of variables Types:
- String (via TextBox)
- Boolean (via CheckBox)
- Double, Int32, Duration, TimeSpan (via Slider)

### Slider customization
As soon as Slider more complex control to specify Double, Int32, Duration, CornerRadius or TimeSpan It is require some additional specification in Variables.
Thus it is require to specify:
- Min - minimum value
- Max - maximum value
- Step - step of values, amount of minimal changes by Left/Right
- SmallChange (optional) - same as step either Step or Small change had to be defined.
- LargeChange (optional) - amount of big change by PageUp/Down or LB/RB, by default it will try to calculate automatically as 1/10 of range.

Value/format of this parameters had to corresponds to specified type:

```yaml
SampleDoubleValue:
    Title: Double value Sample
    Type: Double
    Default: 0.5
    Slider:
        Min: 0
        Max: 2
        Step: 0.1
```
```yaml
SampleDurationValue:
    Title: Duration value Sample
    Type: Duration
    Default: "00:00:00.5"
    Slider:
        Min: "00:00:00.1"
        Max: "00:01:00"
        Step: "00.1"
        LargeChange: "00:00:01"
```
```yaml
SampleInt32Value:
    Title: Int value Sample
    Type: Int32
    Default: 5
    Slider:
        Min: 0
        Max: 10
        Step: 1
```


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

## Fullscreen setting menu integration
Plugin support integration of Theme Options settings into Main menu->Settings in fullscreen.
As soon as various themes has very wide customisations of default controls - ability to shange styles was introduced with next keys and types. Tune this stlyle according to your theme and settings. The best location is native `Views\SettingsMenus.xaml` file.

### Theme options menu item
```xml
<DataTemplate x:Key="SettingsMenuThemeOptionsButtonTemplate">
    <StackPanel Orientation="Horizontal">
        <TextBlock Text="&#xef4b;" FontFamily="{DynamicResource FontIcoFont}" FontSize="24"
                    VerticalAlignment="Center" HorizontalAlignment="Center" Margin="0,0,10,0"/>
        <TextBlock Text="{DynamicResource LOCThemeOptionsSettingsMenu}" VerticalAlignment="Center" />
    </StackPanel>
</DataTemplate>
```
Specify look and feel of Theme Options menu item in settings menu.

### Preview image location

Next style define location of preview image relative to main settings dialog window
```xml
    <Style TargetType="Grid" x:Key="ThemeOptionsPreviewImageGridStyle">
        <Setter Property="Margin" Value="0,420,-440,-600" />
        <Setter Property="Width" Value="360" />
        <Setter Property="Height" Value="360" />
        <Setter Property="VerticalAlignment" Value="Top" />
        <Setter Property="HorizontalAlignment" Value="Right" />
    </Style>
```

Additionaly to specify image location withing placeholder use
```xml
    <Style TargetType="Image" x:Key="ThemeOptionsPreviewImageStyle">
        <Setter Property="VerticalAlignment" Value="Top" />
        <Setter Property="HorizontalAlignment" Value="Left" />
    </Style>
```

### Text Input Button/box
As soon as Fullscreen mode is aimed to be used with control, typing is tricky. Instead of using native TextBox field, it is using button to open On-screen keyboard to type text.
So if you theme use sreing variables - you may need to customize look and feel of this 'TextBox button"

Style to configure button style (: (mostly for location and margins)
```xml
    <Style x:Key="SettingsSectionInputBoxStyle" TargetType="{x:Type ButtonEx}" BasedOn="{StaticResource {x:Type ButtonEx}}">
        <Setter Property="BorderThickness" Value="2" />
        <Setter Property="MinWidth" Value="300" />
        <Setter Property="Margin"  Value="10,0,10,10" />
    </Style>
```

and data template to specify content template:
```xml
    <DataTemplate x:Key="SettingsSectionInputBoxTemplate">
        <DockPanel Margin="0,-10,0,-10" MinWidth="{Binding MinWidth, RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type ButtonEx}}}">
            <Viewbox DockPanel.Dock="Right" Height="32" Margin="20,0,0,0">
                <TextBlock Text="&#xec55;" FontFamily="{StaticResource FontIcoFont}"
                           Foreground="{DynamicResource TextBrush}"
                           FontSize="{DynamicResource FontSize}"
                           VerticalAlignment="Center"/>
            </Viewbox>
            <TextBlock  DockPanel.Dock="Left"
                        HorizontalAlignment="Left"
                        VerticalAlignment="Center"
                        FontFamily="Segoe UI Light">
                <ContentPresenter Content="{Binding}" />
            </TextBlock>
        </DockPanel>
    </DataTemplate>
```

### Variables grouping

You may visually group variables options. To do so Add 'Header' type variable in theme options:

```yaml
Variables:
    UserName:
        Title: User name
        Type: String
        Default:
    SomeHeader:
        Title: This is header
        Type: Header
        TitleStyle: SettingsSectionHeaderText
        Style: SettingsSectionHeaderDockPanelStyle
    TopButtonAlwaysVisible:
        Title: Topbar is always visible
        Type: Boolean
        Default: True
        Style: SettingsSectionOffsetDockPanelStyle
```

Also you may specify name of Style to be used for DockPanel hosted each option as 'Style' parameter and/or Stile of TextBlock to specify style of header title.
This styles will help you to organize items offsets

```xml
    <Style x:Key="SettingsSectionHeaderText" TargetType="TextBlock" BasedOn="{StaticResource TextBlockBaseStyle}">
        <Setter Property="VerticalAlignment" Value="Center" />
        <Setter Property="HorizontalAlignment" Value="Center" />
        <Setter Property="FontSize" Value="40" />
    </Style>

    <Style x:Key="SettingsSectionHeaderDockPanelStyle" TargetType="DockPanel">
        <Setter Property="VerticalAlignment" Value="Center" />
        <Setter Property="HorizontalAlignment" Value="Center" />
        <Setter Property="Margin" Value="0,0,0,0" />
    </Style>

    <Style x:Key="SettingsSectionOffsetDockPanelStyle" TargetType="DockPanel">
        <Setter Property="VerticalAlignment" Value="Center" />
        <Setter Property="Margin" Value="30,0,0,0" />
    </Style>
```

Note: please be aware of TargetType value.

