using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;

using Playnite.SDK;
using Playnite.SDK.Data;
using System.Windows.Controls;


namespace ThemeOptions.Models
{
    using ThemesSelectedPresets = Dictionary<string, List<string>>;
    using ThemesSettings = Dictionary<string, VariablesValues>;

    public static class DictionaryExtension
    {
        static public TValue Get<TKey,TValue>(this Dictionary<TKey,TValue>dict, TKey key, TValue def = default)
        {
            return dict != null && dict.ContainsKey(key) ? dict[key] : def;
        }
    }

    public class Settings : ObservableObject
    {
        private ThemesSelectedPresets selectedPresets = new ThemesSelectedPresets();

        private ThemesSettings userSettings = new ThemesSettings();

        public ThemesSelectedPresets SelectedPresets { get => selectedPresets; }

        public ThemesSettings UserSettings { get => userSettings; }

        [DontSerialize]
        public DynamicProperties Options { get; } = new DynamicProperties();

        [DontSerialize]
        public GamepadState GamepadState { get; } = new GamepadState();

        [DontSerialize]
        public MinimalVersion MinimalVersion { get; } = new MinimalVersion();

        [DontSerialize]
        public string Version { get => MinimalVersion.PluginVersion; }

        [DontSerialize]
        public bool IsInstalled { get => true; }


        public void ThemeToOptions(string themeId)
        {
            Options themeOptions = Theme.FromId(themeId)?.Options;

            Variables themeSettings = themeOptions?.Variables;
            VariablesValues presetSettings = themeOptions?.Presets?.GetConstants(SelectedPresets.Get(themeId));
            VariablesValues userSettings = UserSettings.Get(themeId);

            VariablesValues variableValues = new VariablesValues(themeSettings).Merge(presetSettings).Merge(userSettings);
            Options.Update(variableValues);
        }

        public void OptionsToTheme(string themeId)
        {
            VariablesValues themeValues = UserSettings.Get(themeId, new VariablesValues());
            VariablesValues userSettings = themeValues.FromDynamicProperties(Options);
            if (userSettings?.Count > 0)
            {
                UserSettings[themeId] = userSettings;
            }
        }
    }

    public class SettingsViewModel : ObservableObject, ISettings, INotifyPropertyChanged
    {
        private readonly ThemeOptions plugin;
        public Theme SelectedTheme { get; set; }

        public List<Theme> CustomizableThemes { get; set; }

        private string _PreviewImage;
        public string PreviewImage
        {
            get { return _PreviewImage; }
            set
            {
                _PreviewImage = value;
                OnPropertyChanged("PreviewImage");
            }
        }
        public void UpdatePreview(string path, bool show)
        {
            if (show)
            {
                PreviewImage = path ;
            }
            else if (PreviewImage == path)
            {
                PreviewImage = null;
            }
        }

        private Settings settings;
        public Settings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }
        public List<string> ThemePresets(string themeId) => settings.SelectedPresets.Get(themeId, new List<string>());
        public VariablesValues ThemeSettings(string themeId)
        {
            Theme theme = Theme.FromId(themeId);
            if (theme == null) return new VariablesValues();
            Variables themeSettings = Theme.FromId(themeId)?.Options?.Variables ?? new Variables();
            VariablesValues userSettings = settings.UserSettings.Get(themeId, new VariablesValues());
            VariablesValues filtered = new VariablesValues(userSettings.Where(
                s => s.Value.Value != null
                  && s.Value.Type == themeSettings.Get(s.Key)?.Type
                  && s.Value.Value != themeSettings.Get(s.Key)?.Default)
            );

            var result = theme.Options.Presets?.GetConstants(ThemePresets(themeId)) ?? new VariablesValues();
            return result.Merge(filtered);
        }

        public SettingsViewModel(ThemeOptions plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<Settings>();

            // LoadPluginSettings returns null if no saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new Settings();
            }

            Settings.ThemeToOptions(plugin.CurrentThemeId);
            Settings.Options.PropertyChanged += (o, e) =>
            {
                Settings.OnPropertyChanged("Options");
            };
            Settings.GamepadState.PropertyChanged += (o, e) =>
            {
                Settings.OnPropertyChanged("ControllerState");
            };
        }



        public Options SelectedThemeOptions = null;

        public Dictionary<string, Options> ThemesOptions = null;


        private void LoadCustomizableThemes()
        {
            CustomizableThemes = Theme.EnumThemes().ToList();
            SelectedTheme = CustomizableThemes.FirstOrDefault(t => t.Id == ThemeOptions.PlayniteAPI.ApplicationSettings.FullscreenTheme) ??
                            CustomizableThemes.FirstOrDefault(t => t.Id == ThemeOptions.PlayniteAPI.ApplicationSettings.DesktopTheme) ??
                            CustomizableThemes.FirstOrDefault();

        }

        private void LoadFromSettings(Settings settings, List<Theme> themes )
        {
            foreach (var theme in themes)
            {
                theme.TranslateOptions();

                foreach (var variable in theme?.Options?.Variables ?? new Variables())
                {
                    if (variable.Value.NeedRestart)
                    {
                        variable.Value.Title += " *";
                    }
                }

                if (theme?.Options?.Presets?.Count > 0)
                {
                    theme.Options.Presets.Enumerate().ForEach(p => {
                        if (p.Value.NeedRestart)
                        {
                            p.Value.Name += " *";
                        }
                    });
                }

                foreach (var preset in theme.PresetList)
                {
                    preset.Selected = preset.OptionsList
                        .FirstOrDefault(
                            option => settings.SelectedPresets.Get(theme.Id)?.Contains(option.Id) == true);
                }

                if (theme.Options.Variables != null)
                {
                    var themeSettings = settings.UserSettings.Get(theme.Id);

                    foreach (var variable in theme.Options.Variables)
                    {
                        variable.Value.Value = (
                            themeSettings != null &&
                            variable.Value.Type != null &&
                            themeSettings.Get(variable.Key)?.Type == variable.Value.Type
                            ? themeSettings[variable.Key].Value : null)
                            ?? variable.Value.Default;
                    }
                }
            }
        }

        private bool SaveToSettings(List<Theme> themes, Settings settings)
        {
            Settings original = Serialization.GetClone(settings);

            foreach (var theme in themes)
            {
                var selectedPresets = theme
                    .PresetList
                    .Where(p => p.Selected != null && !p.Selected.Id.ToLower().EndsWith("default"))
                    .Select(p => p.Selected.Id).ToList();

                if (selectedPresets?.Count > 0)
                {
                    settings.SelectedPresets[theme.Id] = selectedPresets;
                }

                if (theme.Options.Variables != null)
                {
                    var variables = new VariablesValues(
                        theme.Options.Variables
                        .Where(v => v.Value.Value != v.Value.Default)
                    );

                    if (variables?.Count > 0)
                    {
                        settings.UserSettings[theme.Id] = variables;
                    }

                    List<string> defaultValues = original.UserSettings.Get(theme.Id, new VariablesValues())
                        .Where(v => !theme.Options.Variables.ContainsKey(v.Key) ||  v.Value.Value == theme.Options.Variables[v.Key]?.Default)
                        .Select(v => v.Key)
                        .ToList();

                    foreach( var key in defaultValues)
                    {
                        original.UserSettings[theme.Id].Remove(key);
                    }
                }
            }

            string currentTheme = plugin.CurrentThemeId;

            Settings updated = Serialization.GetClone(settings);

            List<string> needsRestart = Theme.FromId(currentTheme)?.Options?.Variables
                ?.Where(v=>v.Value.NeedRestart)
                ?.Select(v => v.Key)
                ?.ToList()
                ?? new List<string>();

            List<string> needsPresetsRestart = Theme.FromId(currentTheme)?.Options?.Presets
                ?.SelectMany(p => p.Value.Presets?.Where(sp => sp.Value.NeedRestart).Select(sp => $"{p.Key}.{sp.Key}"))
                ?.ToList();

            var originalNonDefaultPreset = original.SelectedPresets.Get(plugin.CurrentThemeId, new List<string>());

            var originalPreset = Theme.FromId(currentTheme)
                ?.PresetList
                ?.Select(
                    p => p.OptionsList.FirstOrDefault(o => originalNonDefaultPreset.Contains(o.Id))?.Id
                    ?? p.OptionsList.FirstOrDefault(o => o.Id.ToLower().EndsWith("default"))?.Id)
                ?.ToList() ?? new List<string>();

            var updatedNonDefaultPreset = updated.SelectedPresets.Get(plugin.CurrentThemeId, new List<string>());
            var updatedPreset = Theme.FromId(currentTheme)
                ?.PresetList
                ?.Select(
                    p => p.OptionsList.FirstOrDefault(o => updatedNonDefaultPreset.Contains(o.Id))?.Id
                    ?? p.OptionsList.FirstOrDefault(o => o.Id.ToLower().EndsWith("default"))?.Id)
                ?.ToList() ?? new List<string>();

            var presetsEqual = Serialization.AreObjectsEqual(
                originalPreset.Where(p => needsPresetsRestart.Contains(p)),
                updatedPreset.Where(p => needsPresetsRestart.Contains(p))
            );


            var variablesEqual = Serialization.AreObjectsEqual(
                original.UserSettings.Get(plugin.CurrentThemeId, new VariablesValues()).Where(v => needsRestart.Contains(v.Key)),
                updated.UserSettings.Get(plugin.CurrentThemeId, new VariablesValues()).Where(v => needsRestart.Contains(v.Key))
            );

            return  !presetsEqual || !variablesEqual;
       }

        public void BeginEdit()
        {
            LoadCustomizableThemes();
            Settings.OptionsToTheme(plugin.CurrentThemeId);
            LoadFromSettings(Settings, CustomizableThemes);
        }

        public void BeginFullscreenEdit(string themeId)
        {
            SelectedTheme = Theme.FromId(themeId);
            CustomizableThemes = new List<Theme> { SelectedTheme };

            Settings.OptionsToTheme(plugin.CurrentThemeId);
            LoadFromSettings(Settings, CustomizableThemes);
        }

        public void CancelEdit()
        {
        }

        public void EndEdit()
        {
            bool needReload = SaveToSettings(CustomizableThemes, Settings);
            plugin.SavePluginSettings(Settings);
            Settings.ThemeToOptions(plugin.CurrentThemeId);

            if (needReload)
            {

                dynamic ctx = Application.Current.MainWindow.DataContext;
                ctx.AppSettings.Fullscreen.OnPropertyChanged("Theme");
                ctx.AppSettings.OnPropertyChanged("Theme");
            }
            else
            {
                plugin.LoadThemeOption();
            }
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }
    }
}