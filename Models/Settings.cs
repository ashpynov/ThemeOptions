using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Windows.Controls;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


namespace ThemeOptions.Models
{
    using ThemesSelectedPresets = Dictionary<string, List<string>>;

    public class Settings : ObservableObject
    {
        private ThemesSelectedPresets selectedPresets = new ThemesSelectedPresets();

        public ThemesSelectedPresets SelectedPresets { get => selectedPresets; set => SetValue(ref selectedPresets, value); }
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
        public List<string> ThemePresets(string themeId) => settings.SelectedPresets[themeId];

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
        }

        public Options SelectedThemeOptions = null;

        public Dictionary<string, Options> ThemesOptions = null;


        private void LoadCustomizableThemes()
        {
            CustomizableThemes = Theme.EnumThemes().Where(theme => theme?.Options != null).ToList();
            SelectedTheme = CustomizableThemes.FirstOrDefault(t => t.Id == ThemeOptions.PlayniteAPI.ApplicationSettings.FullscreenTheme) ??
                                    CustomizableThemes.FirstOrDefault(t => t.Id == ThemeOptions.PlayniteAPI.ApplicationSettings.DesktopTheme) ??
                                    CustomizableThemes.FirstOrDefault();

        }

        private void LoadFromSettings(ThemesSelectedPresets selectedPresets, List<Theme> themes )
        {
            foreach (var theme in themes)
            {
                foreach (var preset in theme.PresetList)
                {
                    preset.Selected = preset.OptionsList
                        .FirstOrDefault(
                            option => selectedPresets.ContainsKey(theme.Id)
                                   && selectedPresets[theme.Id].Contains(option.Id));
                }
            }
        }

        private void SaveToSettings(List<Theme> themes, ThemesSelectedPresets selectedPresets)
        {
            foreach (var theme in themes)
            {
                selectedPresets[theme.Id] = theme
                    .PresetList
                    .Where(p => p.Selected != null && !p.Selected.Id.ToLower().EndsWith("default"))
                    .Select(p => p.Selected.Id).ToList();
            }
        }


        public void BeginEdit()
        {
            LoadCustomizableThemes();
            LoadFromSettings(Settings.SelectedPresets, CustomizableThemes);
        }

        public void CancelEdit()
        {
        }

        public void EndEdit()
        {
            SaveToSettings(CustomizableThemes, Settings.SelectedPresets);
            plugin.SavePluginSettings(Settings);
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